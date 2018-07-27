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
    /// <typeparam name="Pronounceable">A type understood by the distance function. Note that Extraction == Pronounceable for the usual case.</typeparam>
    public abstract class AbstractAcceleratedFuzzyMatcher<Target, Extraction, Pronounceable> : AbstractFuzzyMatcher<Target, Extraction, Pronounceable>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAcceleratedFuzzyMatcher{Target,Extraction,Pronounceable}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance operator.</param>
        /// <param name="extractionToPronounceable">A mapping of the extraction type to a type understood by the distance function. Note that this is only required if Extraction != Pronounceable</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == Pronounceable for the usual case.</param>
        internal AbstractAcceleratedFuzzyMatcher(IList<Target> targets, IDistance<Pronounceable> distance, Func<Extraction, Pronounceable> extractionToPronounceable, Func<Target, Extraction> targetToExtraction = null)
            : this(targets, distance.Distance, extractionToPronounceable, targetToExtraction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAcceleratedFuzzyMatcher{Target,Extraction,Pronounceable}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance delegate.</param>
        /// <param name="extractionToPronounceable">A mapping of the extraction type to a type understood by the distance function. Note that this is only required if Extraction != Pronounceable</param>
        /// <param name="extractDelegate">A mapping of the input types to a type understood by the distance function. Note that Extraction == Pronounceable for the usual case.</param>
        internal AbstractAcceleratedFuzzyMatcher(IList<Target> targets, DistanceFunc distance, Func<Extraction, Pronounceable> extractionToPronounceable, Func<Target, Extraction> extractDelegate = null)
            : base(true, targets, distance, extractionToPronounceable, extractDelegate)
        {
        }

        /// <summary>
        /// Makes the native call to FindNearestWithin method virtual so we can use normal or accelerated version.
        /// </summary>
        /// <param name="native">Pointer to the native FuzzyMatcher object</param>
        /// <param name="count">maximum number of elements to retrieve</param>
        /// <param name="limit">threshold under which we have a match using the fuzzy matcher</param>
        /// <param name="nearestIdxs">array in which result elements are stored</param>
        /// <param name="distances">array in which result distances are stored</param>
        /// <param name="buffer">buffer for error message</param>
        /// <param name="bufferSize">size of the buffer</param>
        /// <returns>The result of the native operation.</returns>
        protected override NativeResult NativeFindNearestWithin(IntPtr native, int count, double limit, int[] nearestIdxs, double[] distances, StringBuilder buffer, ref int bufferSize)
        {
            return FuzzyMatcherBase.AcceleratedFuzzyMatcher_FindNearestWithin(native, count, limit, nearestIdxs, distances, buffer, ref bufferSize);
        }
    }
}
