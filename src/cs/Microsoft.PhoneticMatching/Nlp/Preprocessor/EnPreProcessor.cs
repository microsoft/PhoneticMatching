// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Preprocessor
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// English Pre-processor.
    /// </summary>
    public class EnPreProcessor : IPreProcessor
    {
        /// <summary>
        /// Rules to apply in chain to the query before pre-processing white spaces. Rules are applied in the order they added to the collection.
        /// </summary>
        protected readonly ChainedRuleBasedPreProcessor Rules = new ChainedRuleBasedPreProcessor();

        private const string StopWords = "a|an|at|by|el|i|in|la|las|los|my|of|on|san|santa|some|the|with|you";

        // TODO this belongs in native code to provide functionality cross language/platform. Will probably have to use libicu in some way.
        private readonly UnicodePreProcessor unicode = new UnicodePreProcessor();
        private readonly CaseFoldingPreProcessor caseFold = new CaseFoldingPreProcessor();
        private readonly WhiteSpacePreProcessor whitespace = new WhiteSpacePreProcessor();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EnPreProcessor"/> class.
        /// </summary>
        public EnPreProcessor()
        {
            // remove stop words
            this.Rules.AddRule(new Regex(string.Format(@"\b({0})\b ?", StopWords)), string.Empty);
            this.Rules.AddRule(new Regex(string.Format(@" ?\b({0})\b", StopWords)), string.Empty);

            // clear punctuation
            this.Rules.AddRule(new Regex(@"[\p{P}\p{S}]+"), " ");
        }

        /// <summary>
        /// Pre-process a string.
        /// </summary>
        /// <param name="query">The string to pre-process.</param>
        /// <returns>The pre-processed string.</returns>
        public string PreProcess(string query)
        {
            string result = query;
            result = this.unicode.PreProcess(result);
            result = this.caseFold.PreProcess(result);
            result = this.Rules.PreProcess(result);
            result = this.whitespace.PreProcess(result);
            return result;
        }
    }
}
