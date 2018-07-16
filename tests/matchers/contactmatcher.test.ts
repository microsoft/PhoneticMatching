// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { EnContactMatcher, ContactFields } from "../../ts/matchers/contactmatcher";

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

function extractContactFields(contact: TestContact): ContactFields {
    return {
        name: `${contact.firstName} ${contact.lastName}`
    }
}

describe("EnContactMatcher", () => {
    test("Phonetic weight.", () => {
        const matcher = new EnContactMatcher(targets, extractContactFields);
        const results = matcher.find("andru");

        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                firstName: "Andrew",
                lastName: "",
            }),
            expect.objectContaining({
                firstName: "Andrew",
                lastName: "Smith",
            })
        ]))
    });

    test("Duplicate names.", () => {
        const matcher = new EnContactMatcher(targets, extractContactFields);
        const results = matcher.find("john");

        expect(results.length).toBe(2);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                firstName: "John",
                lastName: "B",
            }),
            expect.objectContaining({
                firstName: "John",
                lastName: "C",
            })
        ]))
    });

    test("Exact match.", () => {
        const matcher = new EnContactMatcher(targets, extractContactFields);
        const results = matcher.find("Andrew Smith");

        expect(results.length).toBe(1);
        expect(results).toEqual(expect.arrayContaining([
            expect.objectContaining({
                firstName: "Andrew",
                lastName: "Smith",
                tele: "1234567",
            }),
        ]))
    });

    test("Find empty.", () => {
        const matcher = new EnContactMatcher(targets, extractContactFields);
        const results = matcher.find("");
        expect(results).toEqual([]);
    });

    test("ctor used as function exception.", () => {
        expect(() => {
            const matcher = (EnContactMatcher as any)(targets, extractContactFields);
        }).toThrow();
    });

    test("Find undefined exception.", () => {
        expect(() => {
            const matcher = new EnContactMatcher(targets, extractContactFields);
            matcher.find(undefined as any);
        }).toThrow();
    });
});
