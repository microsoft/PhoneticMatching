// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.PlaceMatcher
{
    using System;

    /// <summary>
    /// Configurations to tweak the accuracy of the place matcher.
    /// </summary>
    public class PlaceMatcherConfig : MatcherConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaceMatcherConfig"/> class.
        /// </summary>
        /// <param name="phoneticWeightPercentage">Weighting trade-off between the phonetic distance and the lexical distance scores.</param>
        /// <param name="maxReturns">Maximum number of places the matcher can return</param>
        /// <param name="findThreshold">Maximum distance to a match. Normalized to 0 for exact match, 1 for nothing matches</param>
        /// <param name="maxDistanceMarginReturns">Candidate cutoff given by Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns)</param>
        /// <param name="bestDistanceMultiplier">best distance multiplier</param>
        public PlaceMatcherConfig(
            double phoneticWeightPercentage = 0.7,
            int maxReturns = 8,
            double findThreshold = 0.35,
            double maxDistanceMarginReturns = 0.02,
            double bestDistanceMultiplier = 1.1)
            : base(phoneticWeightPercentage, maxReturns, findThreshold, maxDistanceMarginReturns, bestDistanceMultiplier)
        {
        }
    }
}
