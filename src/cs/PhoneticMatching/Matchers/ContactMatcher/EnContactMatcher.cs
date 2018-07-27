// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatching.Matchers.ContactMatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PhoneticMatching.Distance;
    using PhoneticMatching.Matchers.FuzzyMatcher;
    using PhoneticMatching.Nlp.Preprocessor;
    using PhoneticMatching.Nlp.Tokenizer;

    /// <summary>
    /// A fuzzy matcher that uses domain knowledge about contacts and sets up a simpler API.
    /// </summary>
    /// <typeparam name="Contact">The type of the contact object.</typeparam>
    public class EnContactMatcher<Contact> : BaseMatcher<Contact>
    {
        private static readonly ITokenizer Tokenizer = new WhitespaceTokenizer();
        private static readonly EnPreProcessor Preprocessor = new EnPreProcessor();
        
        private readonly IFuzzyMatcher<Target<Contact>, DistanceInput> nameFuzzyMatcher;
        private readonly IFuzzyMatcher<Target<Contact>, DistanceInput> aliasFuzzyMatcher;
        private readonly int nameMaxWindowSize;
        private readonly int aliasMaxWindowSize;
        private readonly EnPronouncer pronouncer = new EnPronouncer();

        /// <summary>
        /// Initializes a new instance of the <see cref="EnContactMatcher{Contact}"/> class. Uses default configurations.
        /// </summary>
        /// <param name="contacts">List of contacts.</param>
        /// <param name="extractContactFields">Delegate to extract contact fields from contact object.</param>
        public EnContactMatcher(IList<Contact> contacts, Func<Contact, ContactFields> extractContactFields)
            : this(contacts, extractContactFields, new ContactMatcherConfig())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnContactMatcher{Contact}"/> class.
        /// </summary>
        /// <param name="contacts">List of contacts.</param>
        /// <param name="extractContactFields">Delegate to extract contact fields from contact object.</param>
        /// <param name="config">Matcher configurations.</param>
        public EnContactMatcher(IList<Contact> contacts, Func<Contact, ContactFields> extractContactFields, ContactMatcherConfig config)
            : base(config)
        {
            List<Target<Contact>> nameTargets = new List<Target<Contact>>();
            List<Target<Contact>> aliasTargets = new List<Target<Contact>>();

            this.nameMaxWindowSize = 1;
            this.aliasMaxWindowSize = 1;

            for (int idx = 0; idx < contacts.Count; ++idx)
            {
                var contact = contacts[idx];
                var fields = extractContactFields(contact);

                if (fields.Name != null)
                {
                    var name = Preprocessor.PreProcess(fields.Name);
                    var nameVariations = this.AddNameVariations(contact, name, idx);

                    this.nameMaxWindowSize = Math.Max(this.nameMaxWindowSize, nameVariations.Count);

                    foreach (var variation in nameVariations)
                    {
                        nameTargets.Add(variation);
                    }
                }

                if (fields.Aliases != null)
                {
                    foreach (var aliasName in fields.Aliases)
                    {
                        var aliasVariations = this.AddNameVariations(contact, aliasName, idx);
                        this.aliasMaxWindowSize = Math.Max(this.aliasMaxWindowSize, aliasVariations.Count);
                        foreach (var variation in aliasVariations)
                        {
                            aliasTargets.Add(variation);
                        }
                    }
                }
            }
            
            Func<Target<Contact>, DistanceInput> extract = contact => this.PhraseToDistanceInput(contact.Phrase);
            var distance = new EnHybridDistance(this.Config.PhoneticWeightPercentage);
            this.nameFuzzyMatcher = new AcceleratedFuzzyMatcher<Target<Contact>, DistanceInput>(nameTargets, distance, extract);
            this.aliasFuzzyMatcher = new AcceleratedFuzzyMatcher<Target<Contact>, DistanceInput>(aliasTargets, distance, extract);
        }

        /// <summary>
        /// Find a contact.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>The matched contacts.</returns>
        public override IList<Contact> Find(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query should not be null");
            }

            var target = Preprocessor.PreProcess(query);
            var nameMaxWindow = this.nameMaxWindowSize * this.Config.MaxReturns;
            var aliasMaxWindow = this.aliasMaxWindowSize * this.Config.MaxReturns;
            var threshold = this.Config.FindThreshold * target.Length;
            
            var names = this.nameFuzzyMatcher.FindNearestWithin(this.PhraseToDistanceInput(target), threshold, nameMaxWindow);
            var aliases = this.aliasFuzzyMatcher.FindNearestWithin(this.PhraseToDistanceInput(target), threshold, aliasMaxWindow);
            var candidates = this.Merge(names, aliases);

            return this.SelectMatches(candidates);
        }

        /// <summary>
        /// Find a contact by only searching over their names.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The matched contacts.</returns>
        public IList<Contact> FindByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name should not be null");
            }

            var target = Preprocessor.PreProcess(name);
            var nameMaxWindow = this.nameMaxWindowSize * this.Config.MaxReturns;
            DistanceInput extraction = new DistanceInput(target, this.pronouncer.Pronounce(target));

            var candidates = this.nameFuzzyMatcher.FindNearestWithin(extraction, this.Config.FindThreshold, nameMaxWindow);

            return this.SelectMatches(candidates);
        }

        /// <summary>
        /// Find a contact by only searching over their aliases.
        /// </summary>
        /// <param name="alias">The alias to search for.</param>
        /// <returns>The matched contacts.</returns>
        public IList<Contact> FindByAlias(string alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException("alias should not be null");
            }

            var target = Preprocessor.PreProcess(alias);
            var aliasMaxWindow = this.aliasMaxWindowSize * this.Config.MaxReturns;
            var candidates = this.aliasFuzzyMatcher.FindNearestWithin(this.PhraseToDistanceInput(target), this.Config.FindThreshold, aliasMaxWindow);

            return this.SelectMatches(candidates);
        }

        private DistanceInput PhraseToDistanceInput(string target)
        {
            DistanceInput extraction = new DistanceInput(target, this.pronouncer.Pronounce(target));

            return extraction;
        }

        private IList<Match<Target<Contact>>> Merge(IList<Match<Target<Contact>>> first, IList<Match<Target<Contact>>> second)
        {
            var candidates = new List<Match<Target<Contact>>>();
            var firstIndex = 0;
            var secondIndex = 0;
            
            while (firstIndex < first.Count && secondIndex < second.Count)
            {
                if (first[firstIndex].Distance < second[secondIndex].Distance)
                {
                    candidates.Add(first[firstIndex]);
                    ++firstIndex;
                }
                else
                {
                    candidates.Add(second[secondIndex]);
                    ++secondIndex;
                }
            }

            return candidates.Concat(first.Skip(firstIndex)).Concat(second.Skip(secondIndex)).ToArray();
        }

        private IList<Target<Contact>> AddNameVariations(Contact contact, string name, int index)
        {
            var tokens = Tokenizer.Tokenize(name);
            var variations = new List<Target<Contact>>();

            // The idea is to have a sliding window anchored at the beginning and the end.
            for (int idx = 0; idx < tokens.Count; ++idx)
            {
                var token = tokens[idx];
                variations.Add(new Target<Contact>(contact, name.Substring(0, token.Interval.Last), index));
                var split = idx + 1;
                if (split < tokens.Count)
                {
                    variations.Add(new Target<Contact>(contact, name.Substring(tokens[split].Interval.First), index));
                }
            }

            return variations;
        }
    }
}
