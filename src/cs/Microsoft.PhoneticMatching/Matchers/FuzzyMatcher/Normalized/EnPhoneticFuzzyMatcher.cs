// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.FuzzyMatcher.Normalized
{
    using System;
    using System.Collections.Generic;
    using PhoneticMatching.Distance;

    /// <summary>
    /// An english pronunciation fuzzy matcher which normalizes results based on length of queries. The fuzziness it determined by the provided distance function.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    public class EnPhoneticFuzzyMatcher<Target> : NormalizedFuzzyMatcher<Target, EnPronunciation>
    {
        private EnPronouncer pronouncer = EnPronouncer.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnPhoneticFuzzyMatcher{Target}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="targetToExtractionPhrase">A mapping of the input types to the query(extraction) type. Note that Extraction == string for normalized cases.</param>
        /// <param name="isAccelerated">Whether the fuzzy matcher uses accelerated implementation or not.</param>
        public EnPhoneticFuzzyMatcher(IList<Target> targets, Func<Target, string> targetToExtractionPhrase = null, bool isAccelerated = true)
        {
            Func<Target, EnPronunciation> targetToExtraction = (target) =>
            {
                string phrase = targetToExtractionPhrase == null ? target as string : targetToExtractionPhrase(target);
                if (phrase == null)
                {
                    throw new InvalidCastException($"Can't cast Target type [{typeof(Target)}] to Extraction type [string]. You must provide a conversion function 'targetToExtractionPhrase'.");
                }

                return this.pronouncer.Pronounce(phrase);
            };
            this.GenericFuzzyMatcher = new FuzzyMatcher<Target, EnPronunciation>(targets, new EnPhoneticDistance(), targetToExtraction, isAccelerated);
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
            var pronunciation = this.pronouncer.Pronounce(query);
            double thresholdScale = pronunciation.Ipa.Length;
            return this.FindNearestWithinNormalized(pronunciation, limit, count, thresholdScale);
        }
    }
}
