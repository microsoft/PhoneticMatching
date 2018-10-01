// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.FuzzyMatcher.Normalized
{
    using System;
    using System.Collections.Generic;
    using FuzzyMatcher;
    using PhoneticMatching.Distance;

    /// <summary>
    /// A string fuzzy matcher which normalizes results based on length of queries. The fuzziness it determined by the provided distance function.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    public class StringFuzzyMatcher<Target> : NormalizedFuzzyMatcher<Target, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringFuzzyMatcher{Target}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == string for normalized cases.</param>
        /// <param name="isAccelerated">Whether the fuzzy matcher uses accelerated implementation or not.</param>
        public StringFuzzyMatcher(IList<Target> targets, Func<Target, string> targetToExtraction = null, bool isAccelerated = true)
        {
            this.GenericFuzzyMatcher = new FuzzyMatcher<Target, string>(targets, new StringDistance(), targetToExtraction, isAccelerated);
        }

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target within limit</returns>
        public override IList<Match<Target>> FindNearestWithin(string query, double limit, int count)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query can't be null");
            }

            double thresholdScale = query.Length;
            return this.FindNearestWithinNormalized(query, limit, count, thresholdScale);
        }
    }
}
