/**
 * @file
 * Contact matcher.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { AcceleratedFuzzyMatcher, EnPronouncer, Speech, EnHybridDistance } from "../maluuba";
import { WhitespaceTokenizer } from "../nlp/tokenizer";
import { EnPreProcessor } from "../nlp/preprocessor";

/**
 * Fields made available from the user defined Contact object for pronunciation and distance functions.
 *
 * @export
 * @interface ContactFields
 */
export interface ContactFields {
    /**
     * The name of the contact.
     *
     * @type {string}
     * @memberof ContactFields
     */
    name?: string;

    /**
     * The aliases the contact also goes by.
     *
     * @type {string[]}
     * @memberof ContactFields
     */
    aliases?: string[];
}

/**
 * Configurations to tweak the accuracy of the contact matcher.
 *
 * @export
 * @class ContactMatcherConfig
 */
export class ContactMatcherConfig {
    public readonly phoneticWeightPercentage: number;
    public maxReturns: number;
    public findThreshold: number;
    public maxDistanceMarginReturns: number;
    public bestDistanceMultiplier: number;

    /**
     *Creates an instance of ContactMatcherConfig.
     * @param {*} [{
     *         phoneticWeightPercentage = 0.7, Between 0 and 1. Weighting trade-off between the phonetic
     *  distance and the lexical distance scores. 1 meaning 100% phonetic score and 0% lexical score.
     *         maxReturns = 4, The maximum number of contacts the matcher can return.
     *         findThreshold = 0.35, The maximum distance to a match. Normalized to 0 for exact match, 1 for nothing matches.
     *  Can be >1 if the lengths do not match.
     *         maxDistanceMarginReturns = 0.02, Candidate cutoff given by
     *  Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns).
     *         bestDistanceMultiplier = 1.1, Candidate cutoff given by
     *  Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns).
     *  }={}]
     * @memberof ContactMatcherConfig
     */
    constructor({
        phoneticWeightPercentage = 0.7,
        maxReturns = 4,
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
 * A fuzzy matcher that uses domain knowledge about contacts and sets up a simpler API.
 *
 * @export
 * @class EnContactMatcher
 * @template Contact The type of the contact object.
 */
export class EnContactMatcher<Contact> {
    private static readonly tokenizer = new WhitespaceTokenizer();
    private static readonly preprocessor = new EnPreProcessor();

    private readonly nameFuzzyMatcher: Speech.FuzzyMatcher<Target<Contact>, string>;
    private readonly aliasFuzzyMatcher: Speech.FuzzyMatcher<Target<Contact>, string>;
    private readonly nameMaxWindowSize: number;
    private readonly aliasMaxWindowSize: number;


    /**
     * Creates an instance of EnContactMatcher.
     *
     * @param {Contact[]} contacts The contacts to match against.
     * @param {(contact: Contact) => ContactFields} [extractContactFields=(contact: Contact): ContactFields => contact]
     *  Transform function from the user provided __Contact__ object to __ContactFields__, an intermediate representation used by the matcher.
     *  Defaults to the identity transform.
     * @param {ContactMatcherConfig} [config=new ContactMatcherConfig()] The matcher's configuration object.
     * @memberof EnContactMatcher
     */
    constructor(contacts: Contact[], extractContactFields: (contact: Contact) => ContactFields = (contact: Contact): ContactFields => contact,
            public readonly config: ContactMatcherConfig = new ContactMatcherConfig()) {
        const nameTargets: Target<Contact>[] = [];
        const aliasTargets: Target<Contact>[] = [];

        let nameMaxWindowSize = 1;
        let aliasMaxWindowSize = 1;
        contacts.forEach((contact, index) => {
            const fields = extractContactFields(contact);

            if (fields.name) {
                const name = EnContactMatcher.preprocessor.preProcess(fields.name);
                const nameVariations = this.addNameVariations(contact, name, index);
                nameMaxWindowSize = Math.max(nameMaxWindowSize, nameVariations.length);
                for (const variation of nameVariations) {
                    nameTargets.push(variation);
                }
            }
            if (fields.aliases) {
                for (const alias of fields.aliases) {
                    // Not preprocessing given aliases, respecting what was passed in.
                    const aliasVariations = this.addNameVariations(contact, alias, index);
                    aliasMaxWindowSize = Math.max(aliasMaxWindowSize, aliasVariations.length);
                    for (const variation of aliasVariations) {
                        aliasTargets.push(variation);
                    }
                }
            }
        });

        this.nameMaxWindowSize = nameMaxWindowSize;
        this.aliasMaxWindowSize = aliasMaxWindowSize;
        const distance = new EnHybridDistance(this.config.phoneticWeightPercentage);
        const extract = (contact: Target<Contact>) => contact.phrase;
        this.nameFuzzyMatcher = new AcceleratedFuzzyMatcher(nameTargets, distance, extract);
        this.aliasFuzzyMatcher = new AcceleratedFuzzyMatcher(aliasTargets, distance, extract);
    }

    /**
     * Find a contact.
     *
     * @param {string} query The search query.
     * @returns {Contact[]} The matched contacts.
     * @memberof EnContactMatcher
     */
    find(query: string): Contact[] {
        const target = EnContactMatcher.preprocessor.preProcess(query);
        const nameMaxWindow = this.nameMaxWindowSize * this.config.maxReturns;
        const aliasMaxWindow = this.aliasMaxWindowSize * this.config.maxReturns;
        const names = this.nameFuzzyMatcher.kNearestWithin(target, nameMaxWindow, this.config.findThreshold);
        const aliases = this.aliasFuzzyMatcher.kNearestWithin(target, aliasMaxWindow, this.config.findThreshold);
        const candidates = this.merge(names, aliases);
        return this.selectMatches(candidates);
    }

    /**
     * Find a contact by only searching over their names.
     *
     * @param {string} name The name to search for.
     * @returns {Contact[]} The matched contacts.
     * @memberof EnContactMatcher
     */
    findByName(name: string): Contact[] {
        const target = EnContactMatcher.preprocessor.preProcess(name);
        const nameMaxWindow = this.nameMaxWindowSize * this.config.maxReturns;
        const candidates = this.nameFuzzyMatcher.kNearestWithin(target, nameMaxWindow, this.config.findThreshold);
        return this.selectMatches(candidates);
    }

    /**
     * Find a contact by only searching over their aliases.
     *
     * @param {string} alias The alias to search for.
     * @returns {Contact[]} The matched contacts.
     * @memberof EnContactMatcher
     */
    findByAlias(alias: string): Contact[] {
        const target = EnContactMatcher.preprocessor.preProcess(alias);
        const aliasMaxWindow = this.aliasMaxWindowSize * this.config.maxReturns;
        const candidates = this.aliasFuzzyMatcher.kNearestWithin(target, aliasMaxWindow, this.config.findThreshold);
        return this.selectMatches(candidates);
    }

    private addNameVariations(contact: Contact, name: string, index: number): Target<Contact>[] {
        const tokens = EnContactMatcher.tokenizer.tokenize(name);
        const variations: Target<Contact>[] = [];

        // The idea is to have a sliding window anchored at the beginning and the end.
        tokens.forEach((token, i) => {
            variations.push(new Target(contact, name.substring(0, token.interval.last), index));
            const split = i + 1;
            if (split < tokens.length) {
                variations.push(new Target(contact, name.substring(tokens[split].interval.first), index));
            }
        });
        return variations;
    }

    private merge(a: Speech.Match<Target<Contact>>[], b: Speech.Match<Target<Contact>>[]): Speech.Match<Target<Contact>>[] {
        const candidates: Speech.Match<Target<Contact>>[] = [];
        let aIndex = 0;
        let bIndex = 0;

        while (aIndex < a.length && bIndex < b.length) {
            if (a[aIndex].distance < b[bIndex].distance) {
                candidates.push(a[aIndex]);
                ++aIndex;
            } else {
                candidates.push(b[bIndex]);
                ++bIndex;
            }
        }
        return candidates.concat(a.slice(aIndex)).concat(b.slice(bIndex));
    }

    private selectMatches(candidates: Speech.Match<Target<Contact>>[]): Contact[] {
        if (!candidates.length) {
            return [];
        }

        const bestDistance = candidates[0].distance;
        const maxDistance = Math.max(bestDistance * this.config.bestDistanceMultiplier, this.config.maxDistanceMarginReturns);

        const dedupe = new Set<number>([]);
        const matches: Contact[] = [];
        for (const candidate of candidates) {
            if (matches.length === this.config.maxReturns) {
                break;
            }
            if (candidate.distance < maxDistance) {
                if (!dedupe.has(candidate.element.id)) {
                    dedupe.add(candidate.element.id);
                    matches.push(candidate.element.contact);
                }
            }
        }
        return matches;
    }
}

class Target<Contact> {
    constructor(public readonly contact: Contact, public readonly phrase: string, public readonly id: number) {}
}
