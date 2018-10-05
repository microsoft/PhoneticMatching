// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.FuzzyMatcher.Normalized
{
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper on FuzzyMatcher that exposes find methods with strings and normalizes distances based on length of extraction objects.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    /// <typeparam name="Extraction">The type of the query object used by the actual fuzzy matcher.</typeparam>
    public abstract class NormalizedFuzzyMatcher<Target, Extraction> : IFuzzyMatcher<Target, string>
    {
        /// <summary>
        /// Gets the size of the matcher. The number of targets constructed with.
        /// </summary>
        public int Count => this.GenericFuzzyMatcher.Count;

        /// <summary>
        /// Gets or sets the actual fuzzy matcher nested in this normalized wrapper.
        /// </summary>
        protected FuzzyMatcher<Target, Extraction> GenericFuzzyMatcher { get; set; }

        /// <summary>
        /// Find the nearest element.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <returns>The closest match to target, or null if the initial targets list was empty.</returns>
        public Match<Target> FindNearest(string query)
        {
            var matches = this.FindNearestWithin(query, double.MaxValue, 1);
            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return matches[0];
            }
        }

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target.</returns>
        public IList<Match<Target>> FindNearest(string query, int count)
        {
            return this.FindNearestWithin(query, double.MaxValue, count);
        }

        /// <summary>
        /// Find the nearest element.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <returns>The closest match to target within limit, or null if no match is found.</returns>
        public Match<Target> FindNearestWithin(string query, double limit)
        {
            var matches = this.FindNearestWithin(query, limit, 1);
            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return matches[0];
            }
        }

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target within limit</returns>
        public abstract IList<Match<Target>> FindNearestWithin(string query, double limit, int count);

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <param name="thresholdScale">Threshold scale</param>
        /// <returns>The __k__ nearest matches to target within limit</returns>
        protected IList<Match<Target>> FindNearestWithinNormalized(Extraction query, double limit, int count, double thresholdScale)
        {
            thresholdScale = thresholdScale == 0 ? 1 : thresholdScale;
            var matches = this.GenericFuzzyMatcher.FindNearestWithin(query, limit * thresholdScale, count);
            var normalizedMatches = new Match<Target>[matches.Count];
            for (int idx = 0; idx < matches.Count; ++idx)
            {
                var match = matches[idx];
                normalizedMatches[idx] = new Match<Target>(match.Element, match.Distance / thresholdScale);
            }

            return normalizedMatches;
        }
    }
}
