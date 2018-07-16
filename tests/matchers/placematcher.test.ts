// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { EnPlaceMatcher, PlaceFields } from "../../ts/matchers/placematcher";

interface TestPlace {
    name: string;
    address: string;
    tele?: string;
    categories?: string[];
}

const targets: Array<TestPlace> = [
    {
        name: "Marbles Restaurant",
        address: "8 William Street E",
        categories: ["Canadian (New)"],
        tele: "1234567",
    },
    {
        name: "Beertown",
        address: "75 King Street S",
        categories: ["Canadian (New)", "Beer, Wine & Spirits", "Bars"],
        tele: "7654321",
    },
    {
        name: "Nick and Nat's Uptown 21",
        address: "21 King St N",
        categories: ["Canadian (New)"],
    },
    {
        name: "The Shops",
        address: "7 Fake Cres. Toronto",
    }
];

function extractPlaceFields(place: TestPlace): PlaceFields {
    return {
        name: place.name,
        address: place.address,
        types: place.categories,
    }
}

describe("EnPlaceMatcher", () => {
    test("By Address.", () => {
        const matcher = new EnPlaceMatcher(targets, extractPlaceFields);
        const results = matcher.find("king street");

        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                name: "Beertown",
                address: "75 King Street S",
            }),
            expect.objectContaining({
                name: "Nick and Nat's Uptown 21",
                address: "21 King St N",
            })
        ]))
    });

    test("Address expansions.", () => {
        const matcher = new EnPlaceMatcher(targets, extractPlaceFields);
        const results = matcher.find("fake crescent");

        expect(results.length).toBe(1);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                name: "The Shops",
                address: "7 Fake Cres. Toronto",
            })
        ]))
    });

    test("By Types.", () => {
        const matcher = new EnPlaceMatcher(targets, extractPlaceFields);
        const results = matcher.find("Bars");

        expect(results.length).toBe(1);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                name: "Beertown",
                address: "75 King Street S",
                categories: ["Canadian (New)", "Beer, Wine & Spirits", "Bars"],
                tele: "7654321",
            })
        ]))
    });

    test("Exact match.", () => {
        const matcher = new EnPlaceMatcher(targets, extractPlaceFields);
        const results = matcher.find("The Shops");

        expect(results.length).toBe(1);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                name: "The Shops",
                address: "7 Fake Cres. Toronto",
            }),
        ]))
    });

    test("Find empty.", () => {
        const matcher = new EnPlaceMatcher(targets, extractPlaceFields);
        const results = matcher.find("");
        expect(results).toEqual([]);
    });

    test("ctor used as function exception.", () => {
        expect(() => {
            const matcher = (EnPlaceMatcher as any)(targets, extractPlaceFields);
        }).toThrow();
    });

    test("Find undefined exception.", () => {
        expect(() => {
            const matcher = new EnPlaceMatcher(targets, extractPlaceFields);
            matcher.find(undefined as any);
        }).toThrow();
    });
});
