// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.FuzzyMatcher.Normalized
{
    using System;
    using System.Collections.Generic;
    using PhoneticMatching.Distance;

    /// <summary>
    /// A hybrid fuzzy matcher which normalizes results based on length of queries. The fuzziness it determined by the provided distance function.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    public class EnHybridFuzzyMatcher<Target> : NormalizedFuzzyMatcher<Target, DistanceInput>
    {
        private double phoneticWeightPercentage = 0;
        private EnPronouncer pronouncer = EnPronouncer.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnHybridFuzzyMatcher{Target}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="phoneticWeightPercentage">Between 0 and 1. 
        /// Weighting trade-off between the phonetic distance and the lexical distance scores. 
        /// 1 meaning 100% phonetic score and 0% lexical score.</param>
        /// <param name="targetToExtractionPhrase">A mapping of the input types to the query(extraction) type. Note that Extraction == string for normalized cases.</param>
        /// <param name="isAccelerated">Whether the fuzzy matcher uses accelerated implementation or not.</param>
        public EnHybridFuzzyMatcher(IList<Target> targets, double phoneticWeightPercentage, Func<Target, string> targetToExtractionPhrase = null, bool isAccelerated = true)
        {
            this.phoneticWeightPercentage = phoneticWeightPercentage;

            Func<Target, DistanceInput> targetToExtraction = (target) =>
            {
                string phrase = targetToExtractionPhrase == null ? target as string : targetToExtractionPhrase(target);
                if (phrase == null)
                {
                    throw new InvalidCastException($"Can't cast Target type [{typeof(Target)}] to Extraction type [string]. You must provide a conversion function 'targetToExtractionPhrase'.");
                }

                return new DistanceInput(phrase, this.pronouncer.Pronounce(phrase));
            };
            this.GenericFuzzyMatcher = new FuzzyMatcher<Target, DistanceInput>(targets, new EnHybridDistance(phoneticWeightPercentage), targetToExtraction, isAccelerated);
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
            var input = new DistanceInput(query, this.pronouncer.Pronounce(query));
            double thresholdScale = (this.phoneticWeightPercentage * input.Pronunciation.Phones.Count) + ((1 - this.phoneticWeightPercentage) * input.Phrase.Length);
            return this.FindNearestWithinNormalized(input, limit, count, thresholdScale);
        }
    }
}
