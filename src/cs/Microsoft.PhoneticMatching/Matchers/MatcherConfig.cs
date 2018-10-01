// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers
{
    using System;

    /// <summary>
    /// Simple matcher configuration without default values.
    /// </summary>
    public class MatcherConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatcherConfig"/> class.
        /// </summary>
        /// <param name="phoneticWeightPercentage">Weighting trade-off between the phonetic distance and the lexical distance scores.</param>
        /// <param name="maxReturns">Maximum number of places the matcher can return</param>
        /// <param name="findThreshold">Maximum distance to a match. Normalized to 0 for exact match, 1 for nothing matches</param>
        /// <param name="maxDistanceMarginReturns">Candidate cutoff given by Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns)</param>
        /// <param name="bestDistanceMultiplier">best distance multiplier</param>
        public MatcherConfig(
            double phoneticWeightPercentage,
            int maxReturns,
            double findThreshold,
            double maxDistanceMarginReturns,
            double bestDistanceMultiplier)
        {
            this.PhoneticWeightPercentage = phoneticWeightPercentage;
            this.MaxReturns = maxReturns;
            this.FindThreshold = findThreshold;
            this.MaxDistanceMarginReturns = maxDistanceMarginReturns;
            this.BestDistanceMultiplier = bestDistanceMultiplier;

            if (this.PhoneticWeightPercentage < 0 || this.PhoneticWeightPercentage > 1)
            {
                throw new ArgumentException("require 0 <= phoneticWeightPercentage <= 1");
            }
        }

        /// <summary>
        /// Gets or sets the Weighting trade-off between the phonetic distance and 
        /// the lexical distance scores. Between 0 and 1. 1 meaning 100% phonetic score and 0% lexical score.
        /// </summary>
        public double PhoneticWeightPercentage { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum number of places the matcher can return.
        /// </summary>
        public int MaxReturns { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance to a match. Normalized to 0 for exact match, 1 for nothing matches. 
        /// Can be >1 if the lengths do not match.
        /// </summary>
        public double FindThreshold { get; set; }

        /// <summary>
        /// Gets or sets the candidate cutoff given by Math.max({best matched distance} * bestDistanceMultiplier, maxDistanceMarginReturns).
        /// </summary>
        public double MaxDistanceMarginReturns { get; set; }

        /// <summary>
        /// Gets or sets the best distance multiplier.
        /// </summary>
        public double BestDistanceMultiplier { get; set; }
    }
}
