// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { EnContactMatcher, ContactMatcherConfig, ContactFields,
    EnPlaceMatcher, PlaceMatcherConfig, PlaceFields } from "../../../ts/matchers"
import Soundex from "./soundex";

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

function matcherAccuracy<Element>(matcher: any, testSet: TestElement<Element>[], label: string, transformQuery?: (query: string) => string): number {
    let total = 0;
    let failed = 0;
    const failedTests = [];
    testSet.forEach((test) => {
        test.queries.forEach((testQuery) => {
            testQuery.transcriptions.forEach((transcription) => {
                let result;
                try {
                    let utterance = transcription.utterance;
                    if (transformQuery) {
                        utterance = transformQuery(utterance);
                    }
                    result = matcher.find(utterance);
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
    const contactsTestSet: TestElement<ContactFields>[] = require("./contacts.json");
    const contacts = contactsTestSet.map((test) => test.element);

    const baselineExactConfig = new ContactMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS, findThreshold: 0});
    const baselineExact = new EnContactMatcher(contacts, (element) => element, baselineExactConfig);

    const baselineStringConfig = new ContactMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS});
    const baselineString = new EnContactMatcher(contacts, (element) => element, baselineStringConfig);

    const baselineSoundexConfig = new ContactMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS});
    const baselineSoundex = new EnContactMatcher(contacts, (element) => {
        const soundexContact = {...element};
        soundexContact.name = Soundex.encode(soundexContact.name);
        soundexContact.aliases = soundexContact.aliases ? soundexContact.aliases.map((alias) => Soundex.encode(alias)) : [];
        return soundexContact;
    }, baselineSoundexConfig);

    const baselinePhoneticConfig = new ContactMatcherConfig({phoneticWeightPercentage: 1, maxReturns: MAX_RETURNS});
    const baselinePhonetic = new EnContactMatcher(contacts, (element) => element, baselinePhoneticConfig);

    const config = new ContactMatcherConfig({maxReturns: MAX_RETURNS});
    const matcher = new EnContactMatcher(contacts, (element) => element, config);

    test("accuracy - default config", () => {
        expect(matcherAccuracy(matcher, contactsTestSet, "Contacts matcher (default)")).toBeGreaterThan(0);
    });

    test("accuracy - pure string distance", () => {
        expect(matcherAccuracy(baselineString, contactsTestSet, "Contacts baseline (100% string distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - Soundex distance", () => {
        expect(matcherAccuracy(baselineSoundex, contactsTestSet, "Contacts baseline (Soundex distance)", Soundex.encode)).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - pure phonetic distance", () => {
        expect(matcherAccuracy(baselinePhonetic, contactsTestSet, "Contacts baseline (100% phonetic distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - baseline (exact match)", () => {
        expect(matcherAccuracy(baselineExact, contactsTestSet, "Contacts baseline (exact match)")).toBeGreaterThanOrEqual(0);
    });
});

describe("TESTSET places", () => {
    const placesTestSet: TestElement<PlaceFields>[] = require("./places.json");
    const places = placesTestSet.map((test) => test.element);

    const baselineExactConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS, findThreshold: 0});
    const baselineExact = new EnPlaceMatcher(places, (element) => element, baselineExactConfig);

    const baselineStringConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS});
    const baselineString = new EnPlaceMatcher(places, (element) => element, baselineStringConfig);

    const baselineSoundexConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 0, maxReturns: MAX_RETURNS});
    const baselineSoundex = new EnPlaceMatcher(places, (element) => {
        const soundexPlace = {...element};
        soundexPlace.name = Soundex.encode(soundexPlace.name);
        soundexPlace.address = Soundex.encode(soundexPlace.address);
        soundexPlace.types = soundexPlace.types ? soundexPlace.types.map((type) => Soundex.encode(type)) : [];
        return soundexPlace;
    }, baselineSoundexConfig);

    const baselinePhoneticConfig = new PlaceMatcherConfig({phoneticWeightPercentage: 1, maxReturns: MAX_RETURNS});
    const baselinePhonetic = new EnPlaceMatcher(places, (element) => element, baselinePhoneticConfig);

    const config = new PlaceMatcherConfig({maxReturns: MAX_RETURNS});
    const matcher = new EnPlaceMatcher(places, (element) => element, config);

    test("accuracy - default config", () => {
        expect(matcherAccuracy(matcher, placesTestSet, "Places matcher (default)")).toBeGreaterThan(0);
    });

    test("accuracy - pure string distance", () => {
        expect(matcherAccuracy(baselineString, placesTestSet, "Places baseline (100% string distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - Soundex distance", () => {
        expect(matcherAccuracy(baselineSoundex, placesTestSet, "Places baseline (Soundex distance)", Soundex.encode)).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - pure phonetic distance", () => {
        expect(matcherAccuracy(baselinePhonetic, placesTestSet, "Places baseline (100% phonetic distance)")).toBeGreaterThanOrEqual(0);
    });

    test("accuracy - baseline (exact match)", () => {
        expect(matcherAccuracy(baselineExact, placesTestSet, "Places baseline (exact match)")).toBeGreaterThanOrEqual(0);
    });
});
