// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {EnPronunciation, Speech} from "../ts";

test("From ARPABET.", () => {
    const arpabet = EnPronunciation.fromArpabet(["dh", "ih1", "s", "ih1", "z", "ax0", "t", "eh1", "s", "t"]);
    expect(arpabet.ipa).toBe("ðɪsɪzətɛst");
    expect(arpabet.phones.length).toBeGreaterThan(0);
});

test("From IPA.", () => {
    const ipa = EnPronunciation.fromIpa("ðɪsɪzətɛst");
    expect(ipa.ipa).toBe("ðɪsɪzətɛst");
    expect(ipa.phones.length).toBeGreaterThan(0);
});

test("Phones.", () => {
    const pron = EnPronunciation.fromArpabet(["P", "R", "OW0", "N", "AH2", "N", "S", "IY0", "EY1", "SH", "AX0", "N"]);
    expect(pron.ipa).toBe("proʊ̯nʌnsieɪ̯ʃən");
    expect(pron.phones.length).toBeGreaterThan(3);

    // p
    let phone = pron.phones[0];
    expect(phone.type).toBe(Speech.PhoneType.CONSONANT);
    expect(phone.phonation).toBe(Speech.Phonation.VOICELESS);
    expect(phone.place).toBe(Speech.PlaceOfArticulation.BILABIAL);
    expect(phone.manner).toBe(Speech.MannerOfArticulation.PLOSIVE);
    expect(!phone.isSyllabic);

    // o
    phone = pron.phones[2];
    expect(phone.type).toBe(Speech.PhoneType.VOWEL);
    expect(phone.phonation).toBe(Speech.Phonation.MODAL);
    expect(phone.height).toBe(Speech.VowelHeight.CLOSE_MID);
    expect(phone.backness).toBe(Speech.VowelBackness.BACK);
    expect(phone.roundedness).toBe(Speech.VowelRoundedness.ROUNDED);
    expect(phone.isSyllabic);

    // ʊ̯
    phone = pron.phones[3];
    expect(phone.type).toBe(Speech.PhoneType.VOWEL);
    expect(phone.phonation).toBe(Speech.Phonation.MODAL);
    expect(phone.height).toBe(Speech.VowelHeight.NEAR_CLOSE);
    expect(phone.backness).toBe(Speech.VowelBackness.NEAR_BACK);
    expect(phone.roundedness).toBe(Speech.VowelRoundedness.ROUNDED);
    expect(!phone.isSyllabic);
});

test("Invalid ARPABET character (has space)", () => {
    expect(() => {
        const arpabet = EnPronunciation.fromArpabet(["F","B ","N","EH","T","IH","K"]);
    }).toThrow("Unrecognized");
});

test("Object import called as function exception.", () => {
    expect(() => {
        const pronunciation = (EnPronunciation as any)();
    }).toThrow();
});

test("Object import called as ctor exception.", () => {
    expect(() => {
        const pronouncer = new (EnPronunciation as any)();
    }).toThrow();
});
