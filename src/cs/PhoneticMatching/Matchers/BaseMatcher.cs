// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatching.Matchers
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
    }
}
