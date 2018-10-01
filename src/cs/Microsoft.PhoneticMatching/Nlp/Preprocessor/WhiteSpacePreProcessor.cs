// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Preprocessor
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Pre-processor that removes consecutive and trailing white spaces.
    /// </summary>
    internal class WhiteSpacePreProcessor : IPreProcessor
    {
        private readonly Regex pattern = new Regex(@"\s{2,}");

        /// <summary>
        /// Function to preform the pre-processing.
        /// </summary>
        /// <param name="query">The string to pre-process.</param>
        /// <returns>The pre-processed string.</returns>
        public string PreProcess(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query can't be null");
            }

            return this.pattern.Replace(query.Trim(), " ");
        }
    }
}
