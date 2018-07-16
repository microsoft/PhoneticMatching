// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import maluuba, {EnHybridDistance, DistanceInput, EnPronouncer} from "../ts/maluuba";

const pronouncer = new EnPronouncer();
function makeInput(phrase: string): DistanceInput {
    return {
        phrase,
        pronunciation: pronouncer.pronounce(phrase)
    }
}

test("English hybrid distance (default import).", () => {
    const dist = new maluuba.EnHybridDistance(0.7);
    expect(dist.distance(makeInput("This, is a test."), makeInput("This, is a test."))).toBe(0);
});

test("English hybrid distance get phoneticWeightPercentage.", () => {
    const dist = new EnHybridDistance(0.7);
    expect(dist.phoneticWeightPercentage).toBe(0.7)
});

test("English hybrid distance.", () => {
    const dist = new EnHybridDistance(0.7);

    expect(dist.distance(makeInput("aaa"), makeInput("bbb"))).toBeGreaterThan(0);
    expect(dist.distance(makeInput("aaa"), makeInput("aaa"))).toBe(0);
    expect(dist.distance(makeInput(""), makeInput(""))).toBe(0);
});

test("ctor used as function exception.", () => {
    expect(() => {
        const distance = (EnHybridDistance as any)();
    }).toThrow();
});

test("Distance on undefined exception.", () => {
    expect(() => {
        const dist = new EnHybridDistance(0.7);
        dist.distance(undefined, undefined);
    }).toThrow();
});

test("Distance on empty objects.", () => {
    expect(() => {
        const dist = new EnHybridDistance(0.7);
        dist.distance({} as any, {} as any);
    }).toThrow();
});

test("Distance on empty input.", () => {
    expect(() => {
        const dist = new EnHybridDistance(0.7);
        dist.distance({phrase:"", pronunciation: undefined}, {phrase:"", pronunciation: undefined} as any);
    }).toThrow();
});

test("phoneticWeightPercentage undefined.", () => {
    expect(() => {
        const dist = new EnHybridDistance(undefined);
    }).toThrow();
});

test("phoneticWeightPercentage out of range.", () => {
    expect(() => {
        const dist = new EnHybridDistance(2);
    }).toThrow();
});
