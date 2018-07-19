// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { WhitespaceTokenizer } from "../../../ts/nlp";

function soundexNumber(c: string) {
    switch (c) {
        case "B":
        case "F":
        case "P":
        case "V":
            return "1";
        case "C":
        case "G":
        case "J":
        case "K":
        case "Q":
        case "S":
        case "X":
        case "Z":
            return "2";
        case "D":
        case "T":
            return "3";
        case "L":
            return "4";
        case "M":
        case "N":
            return "5";
        case "R":
            return "6";

        default:
            return c;
    }
}

function encodeWord(word: string): string {
    let soundex = "";
    if (word.length === 0) {
        return soundex;
    }

    let i = 0;
    let c = word.charAt(i);
    let n = soundexNumber(c);

    soundex += c;

    for (++i; i < word.length; ++i) {
        c = word.charAt(i);
        if (c == "H" || c == "W") {
            // Completely ignore H and W
            continue;
        }

        const newN = soundexNumber(c);
        if (newN === c) {
            // Ignore vowels, but make sure to encode consonants on either
            // side twice (i.e., "SIS" => "22")
            n = "0";
            continue;
        }

        if (n !== newN) {
            n = newN;
            soundex += n;
        }
    }
    if (soundex.length < 4) {
        soundex += "0".repeat(4 - soundex.length);
    }
    return soundex.substr(0, 4);
}


/**
 * Modified version of Soundex to apply the original fixed-length Soundex on each word,
 * then concatenate those encoded results together.
 *
 * @abstract
 * @class Soundex
 */
abstract class Soundex {
    private static readonly tokenizer = new WhitespaceTokenizer();

    static encode(text: string): string {
        const tokens = Soundex.tokenizer.tokenize(text.toUpperCase());
        return tokens.map(token => encodeWord(token.value)).join(" ");
    }
}

export default Soundex;
