// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Tokenizer
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Tokenizing base-class that will split on the given RegExp.
    /// </summary>
    public class SplittingTokenizer : ITokenizer
    {
        private readonly Regex pattern;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplittingTokenizer"/> class.
        /// </summary>
        /// <param name="pattern">Pattern that splits the query when matched.</param>
        public SplittingTokenizer(Regex pattern)
        {
            this.pattern = pattern;
        }

        /// <summary>
        /// Tokenize the query.
        /// </summary>
        /// <param name="query">Query to tokenize.</param>
        /// <returns>Collection of tokens.</returns>
        public IList<Token> Tokenize(string query)
        {
            List<Token> result = new List<Token>();
            var index = 0;
            MatchCollection matches = this.pattern.Matches(query);
            foreach (Match match in matches)
            {
                if (index < match.Index)
                {
                    var interval = new Interval(index, match.Index);
                    var token = new Token(query.Substring(interval.First, interval.Length), interval);
                    result.Add(token);
                    index += interval.Length + match.Length;
                }
                else if (index == match.Index)
                {
                    index += match.Length;
                }
            }

            // Add the rest.
            if (index < query.Length)
            {
                var interval = new Interval(index, query.Length);
                var token = new Token(query.Substring(interval.First, interval.Length), interval);
                result.Add(token);
            }

            return result;
        }
    }
}
