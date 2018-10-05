// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract matcher to implement common logic between various kind of simplified matchers.
    /// </summary>
    /// <typeparam name="T">Type of elements being matched.</typeparam>
    public abstract class BaseMatcher<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMatcher{T}"/> class.
        /// </summary>
        /// <param name="config">Matcher configurations</param>
        protected BaseMatcher(MatcherConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// Gets or sets the matcher configurations
        /// </summary>
        public MatcherConfig Config { get; protected set; }

        /// <summary>
        /// Find a contact.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>The matched contacts.</returns>
        public abstract IList<T> Find(string query);

        /// <summary>
        /// Select the best matches from the candidates according to the matcher's configurations. 
        /// </summary>
        /// <param name="candidates">Matches candidates</param>
        /// <returns>A collection of items matching the target with the limit configured.</returns>
        protected IList<T> SelectMatches(IList<Match<Target<T>>> candidates)
        {
            var matches = new List<T>();
            if (candidates.Count != 0)
            {
                var bestDistance = candidates[0].Distance;
                var maxDistance = Math.Max(
                    bestDistance * this.Config.BestDistanceMultiplier,
                    this.Config.MaxDistanceMarginReturns);

                var dedupe = new HashSet<int>();
                foreach (var candidate in candidates)
                {
                    // supports MaxReturns == 0
                    if (matches.Count >= this.Config.MaxReturns)
                    {
                        break;
                    }

                    if (candidate.Distance < maxDistance)
                    {
                        if (!dedupe.Contains(candidate.Element.Id))
                        {
                            dedupe.Add(candidate.Element.Id);
                            matches.Add(candidate.Element.Value);
                        }
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Target equality comparer based on phrase and target identifier.
        /// </summary>
        protected class TargetEqualityComparer : IEqualityComparer<Target<T>>
        {
            /// <summary>
            /// Returns true only if x and y have the same phrase and target identifier
            /// </summary>
            /// <param name="x">first target</param>
            /// <param name="y">second target</param>
            /// <returns>True only if x and y have the same phrase and target identifier</returns>
            public bool Equals(Target<T> x, Target<T> y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                else if (x == null || y == null)
                {
                    return false;
                }

                return string.Equals(x.Phrase, y.Phrase) && int.Equals(x.Id, y.Id);
            }

            /// <summary>
            /// Computes the hash code based on phrase and target identifier.
            /// </summary>
            /// <param name="obj">target object</param>
            /// <returns>Hash code of anonymous object constructed with phrase and target identifier</returns>
            public int GetHashCode(Target<T> obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return new { obj.Phrase, obj.Id }.GetHashCode();
            }
        }
    }
}
