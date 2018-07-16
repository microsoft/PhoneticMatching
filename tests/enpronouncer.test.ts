// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import maluuba, {EnPronouncer} from "../ts/maluuba";

test("English pronouncer.", () => {
    const pronouncer = new EnPronouncer();
    expect(pronouncer.pronounce("This, is a test.").ipa).toBe("ðɪsɪzətɛst");
});

test("English pronouncer (default import).", () => {
    const pronouncer = new maluuba.EnPronouncer();
    expect(pronouncer.pronounce("This, is a test.").ipa).toBe("ðɪsɪzətɛst");
});

test("ctor used as function exception.", () => {
    expect(() => {
        const pronouncer = (EnPronouncer as any)();
    }).toThrow();
});

test("Pronouncing undefined exception.", () => {
    expect(() => {
        const pronouncer = new EnPronouncer();
        pronouncer.pronounce(undefined as any);
    }).toThrow();
});
