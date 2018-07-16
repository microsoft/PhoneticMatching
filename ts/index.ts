// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

export { default } from "./maluuba";
export * from "./maluuba";

import * as nlpModule from "./nlp";
// TypeDoc gets 'typeof nlpModule', which translates into a "/full/machine/path" in the html docs? Creating an object obscures that.
/**
 * Bubble-up re-exports of __nlp__ module for convenience.
 */
export const nlp = {...nlpModule};

import * as matchersModule from "./matchers";
/**
 * Bubble-up re-exports of __matchers__ module for convenience.
 */
export const matchers = {...matchersModule}
