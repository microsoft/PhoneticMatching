// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { EnPreProcessor } from "../../ts/nlp";

describe("EnPreProcessor", () => {
    const processor = new EnPreProcessor();

    test("Hi", () => {
        // "Híﬃ"
        // í has a combining acute accent, ﬃ is a ligature
        expect(processor.preProcess("Hi\u0301\uFB03")).toBe("h\u00EDffi");
    });

    test("Digits", () => {
        expect(processor.preProcess("123 King  St")).toBe("123 king st");
        expect(processor.preProcess("2 Wildwood  Place")).toBe("2 wildwood place");
    });

    test("Punctuation", () => {
        expect(processor.preProcess("!omg! ch!ll ?how?")).toBe("omg ch ll how");
    });

    test("Apostrophe and case", () => {
        expect(processor.preProcess("Justin's haus")).toBe("justin s haus");
    });

    test("simple tokenization", () => {
        expect(processor.preProcess("call mom")).toBe("call mom");
        expect(processor.preProcess("call MoM!")).toBe("call mom");
        expect(processor.preProcess("*(*&call,   MoM! )_+")).toBe("call mom");
        expect(processor.preProcess(":call/mom")).toBe("call mom");
        expect(processor.preProcess("Call  mom.")).toBe("call mom");
        expect(processor.preProcess("Call  mom .")).toBe("call mom");
        expect(processor.preProcess("Call  mom .")).toBe("call mom");
    });
});
