/**
 * @file
 * Bindings for native phonetic matching classes.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import path from "path";
const binary = require("node-pre-gyp");
const binding_path = binary.find(path.resolve(path.join(__dirname,"../package.json")));
const maluuba: Maluuba = require(binding_path);

/**
 * The exported classes for phonetic matching.
 *
 * @export
 * @interface Maluuba
 */
export interface Maluuba {
    EnPronunciation: EnPronunciationStatic;

    AcceleratedFuzzyMatcher: FuzzyMatcherConstructor;
    EnPhoneticDistance: EnPhoneticDistanceConstructor;
    EnHybridDistance: EnHybridDistanceConstructor;
    EnPronouncer: EnPronouncerConstructor;
    FuzzyMatcher: FuzzyMatcherConstructor;

    StringDistance: StringDistanceConstructor;
};

/**
 * Factory methods to create {@link Speech.EnPronunciation} from other sources.
 *
 * @export
 * @interface EnPronunciationStatic
 */
export interface EnPronunciationStatic {
    /**
     * Constructs a {@link Speech.EnPronunciation} from an IPA string. E.g. (phonetic) "fənɛtɪk".
     *
     * @param {string} ipa The IPA string.
     * @returns {Speech.EnPronunciation} The English pronunciation.
     * @memberof EnPronunciationStatic
     */
    fromIpa(ipa: string): Speech.EnPronunciation;

    /**
     * Constructs a {@link Speech.EnPronunciation} from an ARPABET string array. E.g. (phonetic) ["F","AH","N","EH","T","IH","K"].
     *
     * @param {Array<string>} arpabet The ARPABET array.
     * @returns {Speech.EnPronunciation} The English pronunciation.
     * @memberof EnPronunciationStatic
     */
    fromArpabet(arpabet: Array<string>): Speech.EnPronunciation;
};

/**
 * Constructs an English pronouncer.
 *
 * @export
 * @class
 * @interface EnPronouncerConstructor
 */
export interface EnPronouncerConstructor {
    new(): Speech.EnPronouncer;
};

/**
 * Constructs an English phonetic distance metric.
 *
 * @export
 * @class
 * @interface EnPhoneticDistanceConstructor
 */
export interface EnPhoneticDistanceConstructor {
    new(): Speech.Distance<Speech.EnPronunciation>;
};

/**
 * Input object for __EnHybridDistance__. Hold the text and the pronunciation of that text.
 *
 * @export
 * @interface DistanceInput
 */
export interface DistanceInput {
    phrase: string;
    pronunciation: Speech.EnPronunciation;
};

/**
 * Constructs a distance metric that weighs between an English phonetic distance and string distance.
 *
 * @export
 * @class
 * @interface EnHybridDistanceConstructor
 */
export interface EnHybridDistanceConstructor {
    /**
     * Create an English hybrid distance.
     *
     * @param {number} phoneticWeightPercentage Between 0 and 1. Weighting trade-off between the phonetic
     *  distance and the lexical distance scores. 1 meaning 100% phonetic score and 0% lexical score.
     * @returns {(Speech.Distance<DistanceInput> & {readonly phoneticWeightPercentage: number})}
     * @memberof EnHybridDistanceConstructor
     */
    new(phoneticWeightPercentage: number): Speech.Distance<DistanceInput> & {readonly phoneticWeightPercentage: number};
};

/**
 * Constructs a fuzzy matcher.
 *
 * @export
 * @class
 * @interface FuzzyMatcherConstructor
 */
export interface FuzzyMatcherConstructor {
    /**
     *
     *
     * @template Target The type of the object to match against.
     * @template Pronounceable The type of input for the distance function.
     * @template Extraction The type of query object.
     * @param {Array<Target>} targets The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.
     * @param {(((a: Pronounceable, b: Pronounceable) => number) | Speech.Distance<Pronounceable>)} distance The distance function.
     * @param {(target: Target) => Extraction} [extract] A mapping of the input types to a type understood by the distance funciton. Note that Extraction == Pronounceable for the usual case.
     * @returns {Speech.FuzzyMatcher<Target, Extraction>} The fuzzy matcher instance.
     * @memberof FuzzyMatcherConstructor
     */
    new<Target, Pronounceable, Extraction>(
        targets: Array<Target>,
        distance: ((a: Pronounceable, b: Pronounceable) => number) | Speech.Distance<Pronounceable>,
        extract?: (target: Target) => Extraction
    ): Speech.FuzzyMatcher<Target, Extraction>;
};

/**
 * Constructs a string edit distance metric.
 *
 * @export
 * @class
 * @interface StringDistanceConstructor
 */
export interface StringDistanceConstructor {
    new(): Speech.Distance<string>;
};

export namespace Speech {
    /**
     * Phone type (consonant or vowel).
     *
     * @export
     * @enum {number}
     */
    export const enum PhoneType {
        CONSONANT,
        VOWEL,
    };

    /**
     * Phonation (voice intensity).
     *
     * @export
     * @enum {number}
     */
    export const enum Phonation {
        VOICELESS,
        BREATHY,
        SLACK,
        MODAL,
        STIFF,
        CREAKY,
        GLOTTAL_CLOSURE,
    };

    /**
     * Place of articulation for consonants.
     *
     * @export
     * @enum {number}
     */
    export const enum PlaceOfArticulation {
        BILABIAL,
        LABIODENTAL,
        DENTAL,
        ALVEOLAR,
        PALATO_ALVEOLAR,
        RETROFLEX,
        ALVEOLO_PALATAL,
        LABIAL_PALATAL,
        PALATAL,
        PALATAL_VELAR,
        LABIAL_VELAR,
        VELAR,
        UVULAR,
        PHARYNGEAL,
        EPIGLOTTAL,
        GLOTTAL,
    };

    /**
     * Manner of articulation for consonants.
     *
     * @export
     * @enum {number}
     */
    export const enum MannerOfArticulation {
        NASAL,
        PLOSIVE,
        SIBILANT_FRICATIVE,
        NON_SIBILANT_FRICATIVE,
        APPROXIMANT,
        FLAP,
        TRILL,
        LATERAL_FRICATIVE,
        LATERAL_APPROXIMANT,
        LATERAL_FLAP,
        CLICK,
        IMPLOSIVE,
        EJECTIVE,
    };

    /**
     * Vowel height.
     *
     * @export
     * @enum {number}
     */
    export const enum VowelHeight {
        CLOSE,
        NEAR_CLOSE,
        CLOSE_MID,
        MID,
        OPEN_MID,
        NEAR_OPEN,
        OPEN,
    };

    /**
     * Horizontal vowel position.
     *
     * @export
     * @enum {number}
     */
    export const enum VowelBackness {
        FRONT,
        NEAR_FRONT,
        CENTRAL,
        NEAR_BACK,
        BACK,
    };

    /**
     * Vowel roundedness.
     *
     * @export
     * @enum {number}
     */
    export const enum VowelRoundedness {
        UNROUNDED,
        LESS_ROUNDED,
        ROUNDED,
        MORE_ROUNDED,
    };

    /**
     * A __phone__ is a unit of speech sound.
     *
     * @export
     * @interface Phone
     */
    export interface Phone {
        readonly type: PhoneType;
        readonly phonation: Phonation;
        readonly place: PlaceOfArticulation;
        readonly manner: MannerOfArticulation;
        readonly height: VowelHeight;
        readonly backness: VowelBackness;
        readonly roundedness: VowelRoundedness;
        readonly isRhotic: boolean;
        readonly isSyllabic: boolean;
    };

    /**
     * A matched element with its distance score.
     *
     * @export
     * @interface Match
     * @template T The element type.
     */
    export interface Match<T> {
        readonly element: T;
        readonly distance: number;
    };

    /**
     * A phonetic pronunciation by a general english speaker.
     *
     * @export
     * @interface EnPronunciation
     */
    export interface EnPronunciation {
        readonly ipa: string;
        readonly phones: Array<Phone>;
    };

    /**
     * Pronounces English texts.
     *
     * @export
     * @interface EnPronouncer
     */
    export interface EnPronouncer {
        /**
         * Pronounce text.
         *
         * @param {string} phrase The text to pronounce.
         * @returns {EnPronunciation} The English Pronunciation.
         * @memberof EnPronouncer
         */
        pronounce(phrase: string): EnPronunciation;
    };

    export interface Distance<T> {
        distance(a: T, b: T): number;
    };

    /**
     * A fuzzy matcher. The fuzziness it determined by the provided distance function.
     *
     * @export
     * @interface FuzzyMatcher
     * @template Target The type of the returned matched object.
     * @template Extraction The type of the query object.
     */
    export interface FuzzyMatcher<Target, Extraction> {
        /**
         * @returns {boolean} true iff size === 0
         * @memberof FuzzyMatcher
         */
        empty(): boolean;

        /**
         * @returns {number} The number of targets constructed with.
         * @memberof FuzzyMatcher
         */
        size(): number;

        /**
         * Find the nearest element.
         *
         * @param {Extraction} target The search target.
         * @returns {(Match<Target> | undefined)} The closest match to __target__, or __undefined__ if the inital __targets__ list was empty.
         * @memberof FuzzyMatcher
         */
        nearest(target: Extraction): Match<Target> | undefined;

        /**
         * Find the nearest element.
         *
         * @param {Extraction} target The search target.
         * @param {number} threshold The maximum distance to a match.
         * @returns {(Match<Target> | undefined)} The closest match to __target__ within __threshold__, or __undefined__ if no match is found.
         * @memberof FuzzyMatcher
         */
        nearestWithin(target: Extraction, threshold: number): Match<Target> | undefined;

        /**
         * Find the __k__ nearest elements.
         *
         * @param {Extraction} target The search target.
         * @param {number} k The maximum number of result to return.
         * @returns {Array<Match<Target>>} The __k__ nearest matches to __target__.
         * @memberof FuzzyMatcher
         */
        kNearest(target: Extraction, k: number): Array<Match<Target>>;

        /**
         * Find the __k__ nearest elements.
         *
         * @param {Extraction} target The search target.
         * @param {number} k The maximum number of result to return.
         * @param {number} threshold The maximum distance to a match.
         * @returns {Array<Match<Target>>} The __k__ nearest matches to __target__ within __threshold__.
         * @memberof FuzzyMatcher
         */
        kNearestWithin(target: Extraction, k: number, threshold: number): Array<Match<Target>>;
    };
}

export const { EnPronouncer, EnPronunciation, EnPhoneticDistance, FuzzyMatcher, AcceleratedFuzzyMatcher,
    EnHybridDistance, StringDistance } = maluuba;
export default maluuba;
