// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.FuzzyMatcher
{
    using System;
    using System.Collections.Generic;
    using PhoneticMatching.Distance;

    /// <summary>
    /// A fuzzy matcher. The fuzziness it determined by the provided distance function.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    /// <typeparam name="Extraction">The type of the query object.</typeparam>
    public class FuzzyMatcher<Target, Extraction> : AbstractFuzzyMatcher<Target, Extraction, Extraction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyMatcher{Target, Extraction}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance operator.</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == Pronounceable for the usual case.</param>
        /// <param name="isAccelerated">Whether the fuzzy matcher uses accelerated implementation or not.</param>
        public FuzzyMatcher(IList<Target> targets, IDistance<Extraction> distance, Func<Target, Extraction> targetToExtraction = null, bool isAccelerated = false)
            : this(targets, distance.Distance, targetToExtraction, isAccelerated)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyMatcher{Target, Extraction}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance delegate.</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == Pronounceable for the usual case.</param>
        /// <param name="isAccelerated">Whether the fuzzy matcher uses accelerated implementation or not.</param>
        public FuzzyMatcher(IList<Target> targets, DistanceFunc distance, Func<Target, Extraction> targetToExtraction = null, bool isAccelerated = false)
            : base(isAccelerated, targets, distance, null, targetToExtraction)
        {
        }
    }
}
