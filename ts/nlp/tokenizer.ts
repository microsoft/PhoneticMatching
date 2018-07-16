/**
 * @file
 * Tokenizers.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

/**
 * An Interval holds the first and last index bounds.
 *
 * @export
 * @class Interval
 */
export class Interval {
    /**
     * Creates an instance of Interval.
     *
     * @param {number} first Starting index (inclusive).
     * @param {number} last Ending index (exclusive).
     * @memberof Interval
     */
    constructor(readonly first: number, readonly last: number) { }

    /**
     * The length of the token.
     *
     * @returns {number} The length.
     * @memberof Interval
     */
    length(): number {
        return this.last - this.first;
    }
}

/**
 * The substring token of the original string with its interval location.
 *
 * @export
 * @class Token
 */
export class Token {
    /**
     * Creates an instance of Token.
     *
     * @param {string} value The substring.
     * @param {Interval} interval The interval location.
     * @memberof Token
     */
    constructor(readonly value: string, readonly interval: Interval) { }
}

/**
 * Tokenizer interface for strings.
 *
 * @export
 * @interface Tokenizer
 */
export interface Tokenizer {
    /**
     * Tokenizes a string.
     *
     * @param {string} query The string to tokenize.
     * @returns {Token[]} The tokens.
     * @memberof Tokenizer
     */
    tokenize(query: string): Token[];
}

/**
 * Tokenizing base-class that will split on the given RegExp.
 *
 * @export
 * @abstract
 * @class SplittingTokenizer
 * @implements {Tokenizer}
 */
export abstract class SplittingTokenizer implements Tokenizer {
    /**
     * Creates an instance of SplittingTokenizer.
     *
     * @param {RegExp} pattern The pattern to split on.
     * @memberof SplittingTokenizer
     */
    constructor(private readonly pattern: RegExp) { }

    tokenize(query: string): Token[] {
        const result: Token[] = [];
        let boundary = 0;
        let match;
        while ((match = this.pattern.exec(query)) !== null) {
            if (boundary < match.index) {
                const interval = new Interval(boundary, match.index);
                const token = new Token(query.substring(interval.first, interval.last), interval);
                result.push(token);
            }
            boundary = this.pattern.lastIndex;
        }

        if (boundary < query.length) {
            // Add the rest.
            const interval = new Interval(boundary, query.length);
            const token = new Token(query.substring(interval.first, interval.last), interval);
            result.push(token);
        }
        return result;
    }
}

/**
 * Tokenizer that splits on whitespace.
 *
 * @export
 * @class WhitespaceTokenizer
 * @extends {SplittingTokenizer}
 */
export class WhitespaceTokenizer extends SplittingTokenizer {
    constructor() {
        super(/\s+/g);
    }
}
