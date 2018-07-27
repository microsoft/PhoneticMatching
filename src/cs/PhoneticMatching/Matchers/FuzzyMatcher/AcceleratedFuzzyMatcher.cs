// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatching.Matchers.FuzzyMatcher
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PhoneticMatching.Distance;
    
    /// <summary>
    /// A fuzzy matcher that compares the target to a precomputed data structure of the stored elements to minimize the number of comparisons.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    /// <typeparam name="Extraction">The type of the query object.</typeparam>
    public class AcceleratedFuzzyMatcher<Target, Extraction> : AbstractAcceleratedFuzzyMatcher<Target, Extraction, Extraction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AcceleratedFuzzyMatcher{Target,Extraction}" /> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance operator.</param>
        /// <param name="targetToExtraction">A mapping of the input types to a type understood by the distance function. Note that Extraction == Pronounceable for the usual case.</param>
        public AcceleratedFuzzyMatcher(IList<Target> targets, IDistance<Extraction> distance, Func<Target, Extraction> targetToExtraction = null)
            : this(targets, distance.Distance, targetToExtraction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceleratedFuzzyMatcher{Target,Extraction}" /> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance delegate.</param>
        /// <param name="targetToExtraction">A mapping of the input types to a type understood by the distance function. Note that Extraction == Pronounceable for the usual case.</param>
        public AcceleratedFuzzyMatcher(IList<Target> targets, DistanceFunc distance, Func<Target, Extraction> targetToExtraction = null)
            : base(targets, distance, null, targetToExtraction)
        {
        }
    }
}
