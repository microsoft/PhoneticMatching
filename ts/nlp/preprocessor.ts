/**
 * @file
 * Pre-processors.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import XRegExp from "xregexp";

/**
 * A Pre-processor. To transfor a string before any classification or understanding is known about it.
 *
 * @export
 * @interface PreProcessor
 */
export interface PreProcessor {

    /**
     * Function to preform the pre-processing.
     *
     * @param {string} query The string to pre-process.
     * @returns {string} The pre-processed string.
     * @memberof PreProcessor
     */
    preProcess(query: string): string;
}

class UnicodePreProcessor implements PreProcessor {
    preProcess(query: string): string {
        return query.normalize("NFKC");
    }
}

class CaseFoldingPreProcessor implements PreProcessor {
    preProcess(query: string): string {
        // Unicode-aware only are recent versions of NodeJS.
        return query.toLowerCase();
    }
}

/**
 * Pre-processes by appling a list of rules sequentially. First rules added are applied first.
 *
 * @export
 * @class ChainedRuleBasedPreProcessor
 * @implements {PreProcessor}
 */
export class ChainedRuleBasedPreProcessor implements PreProcessor {
    private readonly rules: Array<{pattern: RegExp, replacement: string}> = [];

    /**
     * Pre-process a string.
     *
     * @param {string} query The string to pre-process.
     * @returns {string} The pre-processed string.
     * @memberof ChainedRuleBasedPreProcessor
     */
    preProcess(query: string): string {
        let result = query;
        for (const rule of this.rules) {
            result = result.replace(rule.pattern, rule.replacement);
        }
        return result
    }

    /**
     * Add a new rule.
     *
     * @param {RegExp} pattern The pattern to find that will be replaced.
     * @param {string} replacement The replacement string if the pattern is matched.
     * @memberof ChainedRuleBasedPreProcessor
     */
    add(pattern: RegExp, replacement: string) {
        this.rules.push({pattern, replacement});
    }
}

class WhiteSpacePreProcessor implements PreProcessor {
    private readonly pattern = /\s{2,}/g;

    preProcess(query: string): string {
        return query.trim().replace(this.pattern, " ");
    }
}

/**
 * English Pre-processor.
 *
 * @export
 * @class EnPreProcessor
 * @implements {PreProcessor}
 */
export class EnPreProcessor implements PreProcessor {
    // TODO this belongs in native code to provide functionality cross language/platform. Will probably have to use libicu in some way.
    private readonly unicode = new UnicodePreProcessor();
    private readonly caseFold = new CaseFoldingPreProcessor();
    protected readonly rules = new ChainedRuleBasedPreProcessor();
    private readonly whitespace = new WhiteSpacePreProcessor();

    private static STOP_WORDS = ["a", "an", "at", "by", "el", "i", "in", "la", "las", "los", "my",
        "of", "on", "san", "santa", "some", "the", "with", "you"].join("|");

    /**
     * Creates an instance of EnPreProcessor.
     *
     * @memberof EnPreProcessor
     */
    constructor() {
        // remove stop words
        this.rules.add(XRegExp(`\\b(${EnPreProcessor.STOP_WORDS})\\b ?`, "g"), "");
        this.rules.add(XRegExp(` ?\\b(${EnPreProcessor.STOP_WORDS})\\b`, "g"), "");

        // clear punctuation
        this.rules.add(XRegExp("[\\p{P}\\p{S}]+", "g"), " ");
    }

    /**
     * Pre-process a string.
     *
     * @param {string} query The string to pre-process.
     * @returns {string} The pre-processed string.
     * @memberof EnPreProcessor
     */
    preProcess(query: string): string {
        let result = query;
        result = this.unicode.preProcess(result);
        result = this.caseFold.preProcess(result);
        result = this.rules.preProcess(result);
        result = this.whitespace.preProcess(result);
        return result
    }
}

/**
 * English Pre-processor with specific rules for places.
 *
 * @export
 * @class EnPlacesPreProcessor
 * @extends {EnPreProcessor}
 */
export class EnPlacesPreProcessor extends EnPreProcessor {

    /**
     * Creates an instance of EnPlacesPreProcessor.
     *
     * @memberof EnPlacesPreProcessor
     */
    constructor() {
        super();
        // Cardinal Directions
        this.rules.add(XRegExp("\\be\\b", "g"), "east");
        this.rules.add(XRegExp("\\bn\\b", "g"), "north");
        this.rules.add(XRegExp("\\bs\\b", "g"), "south");
        this.rules.add(XRegExp("\\bw\\b", "g"), "west");

        this.rules.add(XRegExp("\\bne\\b", "g"), "north east");
        this.rules.add(XRegExp("\\bnw\\b", "g"), "north west");
        this.rules.add(XRegExp("\\bse\\b", "g"), "south east");
        this.rules.add(XRegExp("\\bsw\\b", "g"), "south west");

        // Address Abbreviations
        // Word boundary doesn't work after the "." so we need look-ahead.
        this.rules.add(XRegExp("\\baly\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "alley");
        this.rules.add(XRegExp("\\bave?\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "avenue");
        this.rules.add(XRegExp("\\bblvd\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "boulevard");
        this.rules.add(XRegExp("\\bbnd\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "bend");
        this.rules.add(XRegExp("\\bcres\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "crescent");
        this.rules.add(XRegExp("\\bcir\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "circle");
        this.rules.add(XRegExp("\\bct\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "court");
        this.rules.add(XRegExp("\\bdr\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "drive");
        this.rules.add(XRegExp("\\best\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "estate");
        this.rules.add(XRegExp("\\bln\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "lane");
        this.rules.add(XRegExp("\\bpkwy\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "parkway");
        this.rules.add(XRegExp("\\bpl\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "place");
        this.rules.add(XRegExp("\\brd\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "road");
        // Assume "st" at the beginning is for "saint".
        this.rules.add(XRegExp("^st\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "saint");
        // If "st" does not occur at the start of the string, then we cannot known if it is for "saint" or "street".
        this.rules.add(XRegExp("\\bst\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "street");
        this.rules.add(XRegExp("\\bxing\.?(?=[\\s\\p{P}\\p{S}]|$)", "g"), "crossing");
    }
}
