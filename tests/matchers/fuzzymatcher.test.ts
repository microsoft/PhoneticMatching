// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {AcceleratedFuzzyMatcher, FuzzyMatcher} from "../../ts/matchers"
import {StringDistance,EnPhoneticDistance,EnHybridDistance} from "../../ts/distance"

const targetStrings = [
    "Andrew Smith",
    "Andrew",
    "John B",
    "John C",
    "Jennifer",
];

interface TestContact {
    firstName: string;
    lastName: string;
    tele?: string;
}

const targets: Array<TestContact> = [
    {
        firstName: "Andrew",
        lastName: "Smith",
        tele: "1234567",
    },
    {
        firstName: "Andrew",
        lastName: "",
    },
    {
        firstName: "John",
        lastName: "B",
        tele: "7654321",
    },
    {
        firstName: "John",
        lastName: "C",
        tele: "2222222",
    },
    {
        firstName: "Jennifer",
        lastName: "",
    }
];

const stringDistance = new StringDistance();
function adjustThreshold(query: string, threshold: number) {
    return threshold*query.length;
}

function simpleDistance(a: TestContact|string, b: TestContact|string) {
    let stringA: string;
    let stringB: string;
    if (typeof a === "string") {
        stringA = a;
    } else {
        stringA = a.firstName + a.lastName;
    }
    if (typeof b === "string") {
        stringB = b;
    } else {
        stringB = b.firstName + b.lastName;
    }
    const distance = stringDistance.distance(stringA, stringB);
    return distance;
}

describe("FuzzyMatcher", () => {
    test("Fuzzy matcher similar match.", () => {
        const matcher = new FuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearest("andru").element).toBe("Andrew");
    });

    test("Fuzzy matcher lower case match.", () => {
        const matcher = new FuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearest("andrew smith").element).toBe("Andrew Smith");
    });

    test("nearestWithin empty.", () => {
        const matcher = new FuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearestWithin("", 0.35)).toBeUndefined();
    });

    test("with StringDistance", () => {
        const matcher = new FuzzyMatcher(targetStrings, new StringDistance());
        // case sensitive, "john b" is the same distance to "John B" and "John C".
        expect(matcher.nearest("john B").element).toBe("John B");
    });

    test("with StringDistance and extractor", () => {
        const matcher = new FuzzyMatcher(targets, new StringDistance(), (target) => `${target.firstName} ${target.lastName}`);
        expect(matcher.nearest("john B").element).toEqual(expect.objectContaining({
            firstName: "John",
            lastName: "B",
            tele: "7654321",
        }));
    });

    test("with EnPhoneticDistance", () => {
        const matcher = new FuzzyMatcher(targetStrings, new EnPhoneticDistance());
        expect(matcher.nearest("john bee").element).toBe("John B");
    });

    test("with EnHybridDistance", () => {
        const matcher = new FuzzyMatcher(targetStrings, new EnHybridDistance(0.7));
        expect(matcher.nearest("john bee").element).toBe("John B");
    });

    test("kNearest john.", () => {
        const matcher = new FuzzyMatcher(targetStrings, simpleDistance);
        const results = matcher.kNearest("john", 2);
        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                element: expect.stringContaining("John B"),
                distance: expect.any(Number)
            }),
            expect.objectContaining({
                element: expect.stringContaining("John C"),
                distance: expect.any(Number)
            })
        ]))
    });

    test("kNearestWithin john.", () => {
        const matcher = new FuzzyMatcher(targetStrings, simpleDistance);
        const query = "john";
        const threshold = adjustThreshold(query, 0.8);
        const results = matcher.kNearestWithin(query, 4, threshold);
        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                element: expect.stringContaining("John B"),
                distance: {
                    asymmetricMatch: (other: any): boolean => {
                        expect(other).toBeLessThan(threshold)
                        return true;
                    }
                }
            }),
            expect.objectContaining({
                element: expect.stringContaining("John C"),
                distance: {
                    asymmetricMatch: (other: any): boolean => {
                        expect(other).toBeLessThan(threshold)
                        return true;
                    }
                }
            })
        ]))
    });

    test("limit 0 exact match", () => {
        const matcher = new FuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearestWithin("John C", 0).element).toBe("John C");
    });

    test("Fuzzy matcher (objects).", () => {
        const matcher = new FuzzyMatcher(targets, simpleDistance);
        expect(matcher.nearest("andrew smith").element).toEqual(expect.objectContaining({
            firstName: "Andrew",
            lastName: "Smith",
            tele: "1234567",
        }));
    });

    test("ctor used as function exception.", () => {
        expect(() => {
            const matcher = (FuzzyMatcher as any)(targets);
        }).toThrow();
    });

    test("Pronouncing undefined exception.", () => {
        expect(() => {
            const matcher = new FuzzyMatcher(targets, simpleDistance);
            matcher.nearest(undefined as any);
        }).toThrow();
    });
});

describe("AcceleratedMatchers.FuzzyMatcher", () => {
    test("Accelerated Fuzzy matcher similar match.", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearest("andru").element).toBe("Andrew");
    });

    test("Accelerated Fuzzy matcher lower case match.", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearest("andrew smith").element).toBe("Andrew Smith");
    });

    test("nearestWithin empty.", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, simpleDistance);
        const results = matcher.nearestWithin("", 0.35);
        expect(results).toBeUndefined();
    });

    test("with StringDistance", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, new StringDistance());
        // case sensitive, "john b" is the same distance to "John B" and "John C".
        expect(matcher.nearest("john B").element).toBe("John B");
    });

    test("with StringDistance and extractor", () => {
        const matcher = new AcceleratedFuzzyMatcher(targets, new StringDistance(), (target) => `${target.firstName} ${target.lastName}`);
        expect(matcher.nearest("john B").element).toEqual(expect.objectContaining({
            firstName: "John",
            lastName: "B",
            tele: "7654321",
        }));
    });

    test("with EnPhoneticDistance", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, new EnPhoneticDistance());
        expect(matcher.nearest("john bee").element).toBe("John B");
    });

    test("with EnHybridDistance", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, new EnHybridDistance(0.7));
        expect(matcher.nearest("john bee").element).toBe("John B");
    });

    test("kNearest john.", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, simpleDistance);
        const results = matcher.kNearest("john", 2);
        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                element: expect.stringContaining("John B"),
                distance: expect.any(Number)
            }),
            expect.objectContaining({
                element: expect.stringContaining("John C"),
                distance: expect.any(Number)
            })
        ]))
    });

    test("kNearestWithin john.", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, simpleDistance);
        const query = "john";
        const threshold = adjustThreshold(query, 0.8);
        const results = matcher.kNearestWithin(query, 4, threshold);
        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                element: expect.stringContaining("John B"),
                distance: {
                    asymmetricMatch: (other: any): boolean => {
                        expect(other).toBeLessThan(threshold)
                        return true;
                    }
                }
            }),
            expect.objectContaining({
                element: expect.stringContaining("John C"),
                distance: {
                    asymmetricMatch: (other: any): boolean => {
                        expect(other).toBeLessThan(threshold)
                        return true;
                    }
                }
            })
        ]))
    });

    test("limit 0 exact match", () => {
        const matcher = new AcceleratedFuzzyMatcher(targetStrings, simpleDistance);
        expect(matcher.nearestWithin("John C", 0).element).toBe("John C");
    });

    test("Accelerated Fuzzy matcher (objects).", () => {
        const matcher = new AcceleratedFuzzyMatcher(targets, simpleDistance);
        expect(matcher.nearest("andrew smith").element).toEqual(expect.objectContaining({
            firstName: "Andrew",
            lastName: "Smith",
            tele: "1234567",
        }));
    });

    test("ctor used as function exception.", () => {
        expect(() => {
            const matcher = (AcceleratedFuzzyMatcher as any)(targets);
        }).toThrow();
    });

    test("Pronouncing undefined exception.", () => {
        expect(() => {
            const matcher = new AcceleratedFuzzyMatcher(targets, simpleDistance);
            matcher.nearest(undefined as any);
        }).toThrow();
    });
});
