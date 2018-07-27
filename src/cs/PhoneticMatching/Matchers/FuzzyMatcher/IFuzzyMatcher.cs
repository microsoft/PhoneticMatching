// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatching.Matchers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Fuzzy Matcher interface.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    /// <typeparam name="Extraction">The type of the query object.</typeparam>
    public interface IFuzzyMatcher<Target, Extraction>
    {
        /// <summary>
        /// Gets the size of the matcher. The number of targets constructed with.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Find the nearest element.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <returns>The closest match to target, or null if the initial targets list was empty.</returns>
        Match<Target> FindNearest(Extraction target);

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target.</returns>
        IList<Match<Target>> FindNearest(Extraction target, int count);

        /// <summary>
        /// Find the nearest element.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <returns>The closest match to target within limit, or null if no match is found.</returns>
        Match<Target> FindNearestWithin(Extraction target, double limit);

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target within limit</returns>
        IList<Match<Target>> FindNearestWithin(Extraction target, double limit, int count);
    }
}
