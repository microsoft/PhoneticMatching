// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.ContactMatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PhoneticMatching.Matchers.FuzzyMatcher.Normalized;
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
        
        private readonly EnHybridFuzzyMatcher<Target<Contact>> nameFuzzyMatcher;
        private readonly EnHybridFuzzyMatcher<Target<Contact>> aliasFuzzyMatcher;
        private readonly int nameMaxWindowSize;
        private readonly int aliasMaxWindowSize;

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
        public EnContactMatcher(IList<Contact> contacts, Func<Contact, ContactFields> extractContactFields, MatcherConfig config)
            : base(config)
        {
            var targetEqualityComparer = new TargetEqualityComparer();
            HashSet<Target<Contact>> nameTargets = new HashSet<Target<Contact>>(targetEqualityComparer);
            HashSet<Target<Contact>> aliasTargets = new HashSet<Target<Contact>>(targetEqualityComparer);

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
            
            this.nameFuzzyMatcher = new EnHybridFuzzyMatcher<Target<Contact>>(nameTargets.ToArray(), this.Config.PhoneticWeightPercentage, (contact) => contact.Phrase);
            this.aliasFuzzyMatcher = new EnHybridFuzzyMatcher<Target<Contact>>(aliasTargets.ToArray(), this.Config.PhoneticWeightPercentage, (contact) => contact.Phrase);
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

            query = Preprocessor.PreProcess(query);
            var nameMaxWindow = this.nameMaxWindowSize * this.Config.MaxReturns;
            var aliasMaxWindow = this.aliasMaxWindowSize * this.Config.MaxReturns;
            
            var names = this.nameFuzzyMatcher.FindNearestWithin(query, this.Config.FindThreshold, nameMaxWindow);
            var aliases = this.aliasFuzzyMatcher.FindNearestWithin(query, this.Config.FindThreshold, aliasMaxWindow);
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

            var query = Preprocessor.PreProcess(name);
            var nameMaxWindow = this.nameMaxWindowSize * this.Config.MaxReturns;

            var candidates = this.nameFuzzyMatcher.FindNearestWithin(query, this.Config.FindThreshold, nameMaxWindow);

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

            var query = Preprocessor.PreProcess(alias);
            var aliasMaxWindow = this.aliasMaxWindowSize * this.Config.MaxReturns;
            var candidates = this.aliasFuzzyMatcher.FindNearestWithin(query, this.Config.FindThreshold, aliasMaxWindow);

            return this.SelectMatches(candidates);
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
