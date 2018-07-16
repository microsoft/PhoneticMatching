// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { EnContactMatcher, ContactMatcherConfig, ContactFields,
    EnPlaceMatcher, PlaceMatcherConfig, PlaceFields } from "../../../ts/matchers"

interface TestElement<Element> {
    /**
     * A unique ID to refer back to this element.
     */
    element: Element & { id: string };
    /**
     * Test queries with the intent targeting this element in some way.
     */
    queries: TestQuery[];
}

interface TestQuery {
    /**
     * What the intention is. What should be heard or what was read.
     */
    query: string;
    /**
     * The records for this test query. What was actually heard or what was written.
     */
    transcriptions: Transcription[];
}

interface Transcription {
    /**
     * A label to track what made this transcription.
     */
    source: string;
    /**
     * What was actually heard/spoken (possible ASR/STT errors).
     */
    utterance: string;
}

// If the expected answer is within the top MAX_RETURNS, then it is a pass.
const MAX_RETURNS = 3;

function matcherAccuracy<Element>(matcher: any, testSet: TestElement<Element>[], label: string): number {
    let total = 0;
    let failed = 0;
    const failedTests = [];
    testSet.forEach((test) => {
        test.queries.forEach((testQuery) => {
            testQuery.transcriptions.forEach((transcription) => {
                let result;
                try {
                    result = matcher.find(transcription.utterance);
                    expect(result).toEqual(expect.arrayContaining([
                        expect.objectContaining({
                            id: test.element.id
                        })
                    ]))
                } catch (e) {
                    ++failed;
                    failedTests.push({expectedContact: test.element, actualResult: result, transcription, query: testQuery.query });
                }
                ++total;
            })
        });
    });
    const passed = total-failed;
    const passedPercentage = passed/total*100
    console.log(`${label} ${passed}/${total} (${passedPercentage}%) Tests passed`);
    // console.log("The failed tests are:\n", failedTests);
    return passedPercentage;
}

describe("TESTSET contacts", () => {
    const contacts: TestElement<ContactFields>[] = require("./contacts.json");

    const baselineExactConfig = new ContactMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS, findThreshold: 0});
    const baselineExact = new EnContactMatcher(contacts.map((test) => test.element), (element) => element, baselineExactConfig);

    const baselineStringConfig = new ContactMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS});
    const baselineString = new EnContactMatcher(contacts.map((test) => test.element), (element) => element, baselineStringConfig);

    const baselinePhoneticConfig = new ContactMatcherConfig({phoneticWeightPercentage: 1, maxReturns: MAX_RETURNS});
    const baselinePhonetic = new EnContactMatcher(contacts.map((test) => test.element), (element) => element, baselinePhoneticConfig);

    const config = new ContactMatcherConfig({maxReturns: MAX_RETURNS});
    const matcher = new EnContactMatcher(contacts.map((test) => test.element), (element) => element, config);

    test("accuracy - default config", () => {
        expect(matcherAccuracy(matcher, contacts, "Contacts matcher (default)")).toBeGreaterThan(0);
    });

    test("accuracy - pure string distance", () => {
        expect(matcherAccuracy(baselineString, contacts, "Contacts baseline (100% string distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - pure phonetic distance", () => {
        expect(matcherAccuracy(baselinePhonetic, contacts, "Contacts baseline (100% phonetic distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - baseline (exact match)", () => {
        expect(matcherAccuracy(baselineExact, contacts, "Contacts baseline (exact match)")).toBeGreaterThanOrEqual(0);
    });
});

describe("TESTSET places", () => {
    const places: TestElement<PlaceFields>[] = require("./places.json");

    const baselineExactConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS, findThreshold: 0});
    const baselineExact = new EnPlaceMatcher(places.map((test) => test.element), (element) => element, baselineExactConfig);

    const baselineStringConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS});
    const baselineString = new EnPlaceMatcher(places.map((test) => test.element), (element) => element, baselineStringConfig);

    const baselinePhoneticConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 1, maxReturns: MAX_RETURNS});
    const baselinePhonetic = new EnPlaceMatcher(places.map((test) => test.element), (element) => element, baselinePhoneticConfig);

    const config = new PlaceMatcherConfig({maxReturns: MAX_RETURNS});
    const matcher = new EnPlaceMatcher(places.map((test) => test.element), (element) => element, config);

    test("accuracy - default config", () => {
        expect(matcherAccuracy(matcher, places, "Places matcher (default)")).toBeGreaterThan(0);
    });

    test("accuracy - pure string distance", () => {
        expect(matcherAccuracy(baselineString, places, "Places baseline (100% string distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - pure phonetic distance", () => {
        expect(matcherAccuracy(baselinePhonetic, places, "Places baseline (100% phonetic distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - baseline (exact match)", () => {
        expect(matcherAccuracy(baselineExact, places, "Places baseline (exact match)")).toBeGreaterThanOrEqual(0);
    });
});
