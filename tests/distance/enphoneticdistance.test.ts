// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {EnPronunciation} from "../../ts";
import {EnPhoneticDistance} from "../../ts/distance"

test("English phonetic distance equality.", () => {
    const dist = new EnPhoneticDistance();
    // This, is a test.
    const test = EnPronunciation.fromIpa("ðɪsɪzətɛst");
    expect(dist.distance(test, test)).toBe(0);
});

test("English phonetic distance.", () => {
    const dist = new EnPhoneticDistance();

    // sam pasupalak
    const sam     = EnPronunciation.fromIpa("sæmpɑsupələk");
    // santa super black
    const santa   = EnPronunciation.fromIpa("sæntəsupɝblæk");
    // samples pollux
    const samples = EnPronunciation.fromIpa("sæmpəlzpɑləks");

    // Check identity of indiscernibles
    expect(dist.distance(sam,     sam)).toBe(0.0);
    expect(dist.distance(santa,   santa)).toBe(0.0);
    expect(dist.distance(samples, samples)).toBe(0.0);

    // Check symmetry
    expect(dist.distance(sam,   santa)   == dist.distance(santa,   sam));
    expect(dist.distance(sam,   samples) == dist.distance(samples, sam));
    expect(dist.distance(santa, samples) == dist.distance(samples, santa));

    // Check triangle inequality
    expect(dist.distance(sam,   samples) < dist.distance(sam,   santa)   + dist.distance(santa,   samples));
    expect(dist.distance(sam,   santa)   < dist.distance(sam,   samples) + dist.distance(samples, santa));
    expect(dist.distance(santa, samples) < dist.distance(santa, sam)     + dist.distance(sam,     samples));

    // Check performance
    expect(dist.distance(sam, santa)   < dist.distance(sam, samples));
    expect(dist.distance(sam, samples) < dist.distance(santa, samples));
});

test("ctor used as function exception.", () => {
    expect(() => {
        const distance = (EnPhoneticDistance as any)();
    }).toThrow();
});

test("Distance on undefined exception.", () => {
    expect(() => {
        const dist = new EnPhoneticDistance();
        dist.distance(undefined, undefined);
    }).toThrow();
});
