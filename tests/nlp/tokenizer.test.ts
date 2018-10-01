// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { WhitespaceTokenizer, Token } from "../../ts/nlp";

function values(tokens: Token[]): string[] {
    return tokens.map((token) => token.value);
}

describe("WhiteSpaceTokenizer", () => {
    const tokenizer = new WhitespaceTokenizer();

    test("empty string", () => {
        expect(values(tokenizer.tokenize(""))).toEqual([]);
    });

    test("no whitespace", () => {
        expect(values(tokenizer.tokenize("example"))).toEqual(["example"]);
    });

    test("Not ending with spaces", () => {
        expect(values(tokenizer.tokenize("  There  are some words, here! #blessed")))
            .toEqual(["There", "are", "some", "words,", "here!", "#blessed"]);
    });

    test("Ends with spaces", () => {
        expect(values(tokenizer.tokenize("  There  are some words, here! #blessed  ")))
            .toEqual(["There", "are", "some", "words,", "here!", "#blessed"]);
    });
});
