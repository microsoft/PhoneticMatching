// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Preprocessor
{
    using System;

    /// <summary>
    /// Pre-Processor to preform the pre-processing with case.
    /// </summary>
    internal class CaseFoldingPreProcessor : IPreProcessor
    {
        /// <summary>
        /// Function to preform the pre-processing with case.
        /// </summary>
        /// <param name="query">The string to pre-process.</param>
        /// <returns>The pre-processed string.</returns>
        public string PreProcess(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query can't be null");
            }

            return query.ToLowerInvariant();
        }
    }
}
