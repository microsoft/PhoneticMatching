// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Preprocessor
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Pre-processes by appling a list of rules sequentially. First rules added are applied first.
    /// </summary>
    public class ChainedRuleBasedPreProcessor : IPreProcessor
    {
        private readonly List<Tuple<Regex, string>> rules = new List<Tuple<Regex, string>>();

        /// <summary>
        /// Function to preform the pre-processing.
        /// </summary>
        /// <param name="query">The string to pre-process.</param>
        /// <returns>The pre-processed string.</returns>
        public string PreProcess(string query)
        {
            string result = query;
            foreach (var rule in this.rules)
            {
                result = rule.Item1.Replace(result, rule.Item2);
            }

            return result;
        }

        /// <summary>
        /// Add a replacement rule
        /// </summary>
        /// <param name="pattern">Pattern to replace</param>
        /// <param name="replacement">String to replace with.</param>
        public void AddRule(Regex pattern, string replacement)
        {
            this.rules.Add(new Tuple<Regex, string>(pattern, replacement));
        }
    }
}
