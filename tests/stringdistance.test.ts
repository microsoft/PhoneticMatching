// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import maluuba, {StringDistance} from "../ts/maluuba";

test("String distance (default import).", () => {
    const dist = new maluuba.StringDistance();
    expect(dist.distance("This, is a test.", "This, is a test.")).toBe(0);
});

test("String distance.", () => {
    const dist = new StringDistance();

    expect(dist.distance("aaa", "bbb")).toBe(3);
    expect(dist.distance("aaa", "aaa")).toBe(0);
    expect(dist.distance("aaa", "aba")).toBe(1);
    expect(dist.distance("", "")).toBe(0);
    expect(dist.distance("", "aaa")).toBe(3);
    expect(dist.distance("aaa", "")).toBe(3);
});

test("ctor used as function exception.", () => {
    expect(() => {
        const distance = (StringDistance as any)();
    }).toThrow();
});

test("Distance on undefined exception.", () => {
    expect(() => {
        const dist = new StringDistance();
        dist.distance(undefined, undefined);
    }).toThrow();
});
