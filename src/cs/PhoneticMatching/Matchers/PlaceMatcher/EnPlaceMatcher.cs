// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatching.Matchers.PlaceMatcher
{
    using System;
    using System.Collections.Generic;
    using PhoneticMatching.Distance;
    using PhoneticMatching.Matchers.FuzzyMatcher;
    using PhoneticMatching.Nlp.Preprocessor;
    using PhoneticMatching.Nlp.Tokenizer;

    /// <summary>
    /// A fuzzy matcher that uses domain knowledge about places and sets up a simpler API.
    /// </summary>
    /// <typeparam name="Place">The type of the place object.</typeparam>
    public class EnPlaceMatcher<Place> : BaseMatcher<Place>
    {
        private static readonly ITokenizer Tokenizer = new WhitespaceTokenizer();
        private static readonly IPreProcessor Preprocessor = new EnPlacesPreProcessor();

        private readonly IFuzzyMatcher<Target<Place>, DistanceInput> fuzzyMatcher;
        private readonly int maxWindowSize;
        private readonly EnPronouncer pronouncer = new EnPronouncer();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnPlaceMatcher{Place}"/> class. Using default <see cref="PlaceMatcherConfig"/>.
        /// </summary>
        /// <param name="places">The places to match against.</param>
        /// <param name="placeFieldsExtractor">Transform function from the user provided __Place__ object to __PlaceFields__, an intermediate representation used by the matcher. Defaults to the identity transform.</param>
        public EnPlaceMatcher(IList<Place> places, Func<Place, PlaceFields> placeFieldsExtractor)
            : this(places, placeFieldsExtractor, new PlaceMatcherConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnPlaceMatcher{Place}"/> class.
        /// </summary>
        /// <param name="places">The places to match against.</param>
        /// <param name="placeFieldsExtractor">Transform function from the user provided __Place__ object to __PlaceFields__, an intermediate representation used by the matcher. Defaults to the identity transform.</param>
        /// <param name="config">The matcher's configuration object.</param>
        public EnPlaceMatcher(IList<Place> places, Func<Place, PlaceFields> placeFieldsExtractor, PlaceMatcherConfig config)
            : base(config)
        {
            var targets = new List<Target<Place>>();

            this.maxWindowSize = 1;
            for (int idx = 0; idx < places.Count; ++idx)
            {
                var place = places[idx];
                var fields = placeFieldsExtractor(place);

                var name = string.Empty;
                var address = string.Empty;

                if (fields.Name != null)
                {
                    name = Preprocessor.PreProcess(fields.Name);
                }

                if (fields.Address != null)
                {
                    address = Preprocessor.PreProcess(fields.Address);
                }

                IList<Target<Place>> nameVariations = this.AddNameVariations(place, idx, name, address);
                this.maxWindowSize = Math.Max(this.maxWindowSize, nameVariations.Count);

                targets.AddRange(nameVariations);

                if (fields.Types != null)
                {
                    foreach (var type in fields.Types)
                    {
                        var fieldVariations = this.AddNameVariations(place, idx, type);
                        this.maxWindowSize = Math.Max(this.maxWindowSize, fieldVariations.Count);
                        targets.AddRange(fieldVariations);
                    }
                }
            }

            this.fuzzyMatcher = new AcceleratedFuzzyMatcher<Target<Place>, DistanceInput>(
                targets,
                new EnHybridDistance(this.Config.PhoneticWeightPercentage),
                (target) => new DistanceInput(target.Phrase, this.pronouncer.Pronounce(target.Phrase)));
        }

        /// <summary>
        /// Find a place.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>The matched places.</returns>
        public override IList<Place> Find(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query can't be null");
            }

            var target = Preprocessor.PreProcess(query);

            return this.Find(new DistanceInput(target, this.pronouncer.Pronounce(target)));
        }

        private IList<Place> Find(DistanceInput query)
        {
            var maxWindow = this.maxWindowSize * this.Config.MaxReturns;
            var candidates = this.fuzzyMatcher.FindNearestWithin(query, this.Config.FindThreshold * query.Phrase.Length, maxWindow);
            IList<Place> result = this.SelectMatches(candidates);
            return result;
        }

        private IList<Target<Place>> AddNameVariations(Place place, int index, string name, string address = null)
        {
            var nameTokens = Tokenizer.Tokenize(name);
            var addressTokens = !string.IsNullOrEmpty(address) ? Tokenizer.Tokenize(address) : new Token[] { };
            var variations = new List<Target<Place>>();

            // The idea is to have a sliding window anchored at the beginning and the end of both name and address
            // individually, and as if they were concatenated.
            for (int idx = 0; idx < nameTokens.Count; ++idx)
            {
                var token = nameTokens[idx];
                variations.Add(new Target<Place>(place, name.Substring(0, token.Interval.Last), index));
                var split = idx + 1;
                if (split < nameTokens.Count)
                {
                    var suffix = name.Substring(nameTokens[split].Interval.First);
                    variations.Add(new Target<Place>(place, suffix, index));
                    if (!string.IsNullOrEmpty(address))
                    {
                        variations.Add(new Target<Place>(place, string.Format("{0} {1}", suffix, address), index));
                    }
                }
            }

            for (int idx = 0; idx < addressTokens.Count; ++idx)
            {
                var token = addressTokens[idx];
                var prefix = !string.IsNullOrEmpty(address) ? address.Substring(0, token.Interval.Last) : string.Empty;
                variations.Add(new Target<Place>(place, prefix, index));
                if (!string.IsNullOrEmpty(name))
                {
                    variations.Add(new Target<Place>(place, string.Format("{0} {1}", name, prefix), index));
                }

                var split = idx + 1;
                if (split < addressTokens.Count)
                {
                    string phrase = !string.IsNullOrEmpty(address) ? address.Substring(addressTokens[split].Interval.First) : string.Empty;
                    variations.Add(new Target<Place>(place, phrase, index));
                }
            }

            return variations;
        }
    }
}
