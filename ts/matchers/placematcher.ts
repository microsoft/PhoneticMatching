/**
 * @file
 * Place matcher.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { Speech } from "..";
import { WhitespaceTokenizer, EnPlacesPreProcessor } from "../nlp";
import { AcceleratedFuzzyMatcher } from "../matchers"
import { EnHybridDistance } from "../distance"

/**
 * Fields made available from the user defined Place object for pronunciation and distance functions.
 *
 * @export
 * @interface PlaceFields
 */
export interface PlaceFields {
    /**
     * The name of the place.
     *
     * @type {string}
     * @memberof PlaceFields
     */
    name?: string;

    /**
     * The address of the place.
     *
     * @type {string}
     * @memberof PlaceFields
     */
    address?: string;

    /**
     * The tags/categories defining the place.
     *
     * @type {string[]}
     * @memberof PlaceFields
     */
    types?: string[];
}

/**
 * Configurations to tweak the accuracy of the place matcher.
 *
 * @export
 * @class PlaceMatcherConfig
 */
export class PlaceMatcherConfig {
    public readonly phoneticWeightPercentage: number;
    public maxReturns: number;
    public findThreshold: number;
    public maxDistanceMarginReturns: number;
    public bestDistanceMultiplier: number;

    /**
     *Creates an instance of PlaceMatcherConfig.
     * @param {*} [{
     *         phoneticWeightPercentage = 0.7, Between 0 and 1. Weighting trade-off between the phonetic
     *  distance and the lexical distance scores. 1 meaning 100% phonetic score and 0% lexical score.
     *         maxReturns = 8, The maximum number of places the matcher can return.
     *         findThreshold = 0.35, The maximum distance to a match. Normalized to 0 for exact match, 1 for nothing matches.
     *  Can be >1 if the lengths do not match.
     *         maxDistanceMarginReturns = 0.02, Candidate cutoff given by
     *  Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns).
     *         bestDistanceMultiplier = 1.1,
     * }={}]
     * @memberof PlaceMatcherConfig
     */
    constructor({
        phoneticWeightPercentage = 0.7,
        maxReturns = 8,
        findThreshold = 0.35,
        maxDistanceMarginReturns = 0.02,
        bestDistanceMultiplier = 1.1} = {}) {
            this.phoneticWeightPercentage = phoneticWeightPercentage;
            this.maxReturns = maxReturns;
            this.findThreshold = findThreshold;
            this.maxDistanceMarginReturns = maxDistanceMarginReturns;
            this.bestDistanceMultiplier = bestDistanceMultiplier;
            if (this.phoneticWeightPercentage < 0 || this.phoneticWeightPercentage > 1) {
                throw new TypeError("require 0 <= phoneticWeightPercentage <= 1");
            }
    }
}

/**
 * A fuzzy matcher that uses domain knowledge about places and sets up a simpler API.
 *
 * @export
 * @class EnPlaceMatcher
 * @template Place The type of the place object.
 */
export class EnPlaceMatcher<Place> {
    private static readonly tokenizer = new WhitespaceTokenizer();
    private static readonly preprocessor = new EnPlacesPreProcessor();

    private readonly fuzzyMatcher: Speech.FuzzyMatcher<Target<Place>, string>;
    private readonly maxWindowSize: number;

    /**
     * Creates an instance of EnPlaceMatcher.
     *
     * @param {Place[]} places The places to match against.
     * @param {(place: Place) => PlaceFields} [extractPlaceFields=(place: Place): PlaceFields => place]
     *  Transform function from the user provided __Place__ object to __PlaceFields__, an intermediate representation used by the matcher.
     *  Defaults to the identity transform.
     * @param {PlaceMatcherConfig} [config=new PlaceMatcherConfig()] The matcher's configuration object.
     * @memberof EnPlaceMatcher
     */
    constructor(places: Place[], extractPlaceFields: (place: Place) => PlaceFields = (place: Place): PlaceFields => place,
            public readonly config: PlaceMatcherConfig = new PlaceMatcherConfig()) {
        const targets: Target<Place>[] = [];

        let maxWindowSize = 1;
        places.forEach((place, index) => {
            const fields = extractPlaceFields(place);

            let name = "";
            let address = "";
            if (fields.name) {
                name = EnPlaceMatcher.preprocessor.preProcess(fields.name);
            }
            if (fields.address) {
                address = EnPlaceMatcher.preprocessor.preProcess(fields.address);
            }
            const nameVariations = this.addNameVariations(place, index, name, address);
            maxWindowSize = Math.max(maxWindowSize, nameVariations.length);
            for (const variation of nameVariations) {
                targets.push(variation);
            }
            if (fields.types) {
                for (const type of fields.types) {
                    // Not preprocessing given types, respecting what was passed in.
                    const fieldVariations = this.addNameVariations(place, index, type);
                    maxWindowSize = Math.max(maxWindowSize, fieldVariations.length);
                    for (const variation of fieldVariations) {
                        targets.push(variation);
                    }
                }
            }
        });

        this.maxWindowSize = maxWindowSize;
        const distance = new EnHybridDistance(this.config.phoneticWeightPercentage);
        const extract = (place: Target<Place>) => place.phrase;
        this.fuzzyMatcher = new AcceleratedFuzzyMatcher(targets, distance, extract);
    }

    /**
     * Find a place.
     *
     * @param {string} query The search query.
     * @returns {Place[]} The matched places.
     * @memberof EnPlaceMatcher
     */
    find(query: string): Place[] {
        const target = EnPlaceMatcher.preprocessor.preProcess(query);
        const maxWindow = this.maxWindowSize * this.config.maxReturns;
        const candidates = this.fuzzyMatcher.kNearestWithin(target, maxWindow, this.config.findThreshold);
        const result = this.selectMatches(candidates);
        return result;
    }

    private addNameVariations(place: Place, index: number, name: string, address?: string): Target<Place>[] {
        const nameTokens = EnPlaceMatcher.tokenizer.tokenize(name);
        const addressTokens = address ? EnPlaceMatcher.tokenizer.tokenize(address) : [];
        const variations: Target<Place>[] = [];

        // The idea is to have a sliding window anchored at the beginning and the end of both name and address
        // individually, and as if they were concatenated.
        nameTokens.forEach((token, i) => {
            variations.push(new Target(place, name.substring(0, token.interval.last), index));
            const split = i + 1;
            if (split < nameTokens.length) {
                const suffix = name.substring(nameTokens[split].interval.first);
                variations.push(new Target(place, suffix, index));
                if (address) {
                    variations.push(new Target(place, suffix + ` ${address}`, index));
                }
            }
        });
        addressTokens.forEach((token, i) => {
            const prefix = address!.substring(0, token.interval.last);
            variations.push(new Target(place, prefix, index));
            if (name) {
                variations.push(new Target(place, `${name} ` + prefix, index));
            }
            const split = i + 1;
            if (split < addressTokens.length) {
                variations.push(new Target(place, address!.substring(addressTokens[split].interval.first), index));
            }
        });
        return variations;
    }

    private selectMatches(candidates: Speech.Match<Target<Place>>[]): Place[] {
        if (!candidates.length) {
            return [];
        }

        const bestDistance = candidates[0].distance;
        const maxDistance = Math.max(bestDistance * this.config.bestDistanceMultiplier, this.config.maxDistanceMarginReturns);

        const dedupe = new Set<number>([]);
        const matches: Place[] = [];
        for (const candidate of candidates) {
            if (matches.length === this.config.maxReturns) {
                break;
            }
            if (candidate.distance < maxDistance) {
                if (!dedupe.has(candidate.element.id)) {
                    dedupe.add(candidate.element.id);
                    matches.push(candidate.element.place);
                }
            }
        }
        return matches;
    }
}

class Target<Place> {
    constructor(public readonly place: Place, public readonly phrase: string, public readonly id: number) {}
}
