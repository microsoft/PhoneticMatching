/**
 * @file
 * Matcher config.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

/**
 * Configurations to tweak the accuracy of a matcher.
 *
 * @export
 * @class MatcherConfig
 */
export class MatcherConfig {
    public readonly phoneticWeightPercentage: number;
    public maxReturns: number;
    public findThreshold: number;
    public maxDistanceMarginReturns: number;
    public bestDistanceMultiplier: number;

    /**
     *Creates an instance of MatcherConfig.
     * @param {*} [{
     *         phoneticWeightPercentage, Between 0 and 1. Weighting trade-off between the phonetic
     *  distance and the lexical distance scores. 1 meaning 100% phonetic score and 0% lexical score.
     *         maxReturns, The maximum number of places the matcher can return.
     *         findThreshold, The maximum distance to a match. Normalized to 0 for exact match, 1 for nothing matches.
     *  Can be >1 if the lengths do not match.
     *         maxDistanceMarginReturns, Candidate cutoff given by
     *  Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns).
     *         bestDistanceMultiplier,
     * }={}]
     * @memberof PlaceMatcherConfig
     */
    constructor(
        phoneticWeightPercentage : number,
        maxReturns : number,
        findThreshold : number,
        maxDistanceMarginReturns : number,
        bestDistanceMultiplier :number ) {
            this.phoneticWeightPercentage = phoneticWeightPercentage;
            this.maxReturns = maxReturns;
            this.findThreshold = findThreshold;
            this.maxDistanceMarginReturns = maxDistanceMarginReturns;
            this.bestDistanceMultiplier = bestDistanceMultiplier;
            if (this.phoneticWeightPercentage < 0 || this.phoneticWeightPercentage > 1) {
                throw new TypeError("require 0 <= phoneticWeightPercentage <= 1");
            }
    }
}