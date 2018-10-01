// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Preprocessor
{
    /// <summary>
    /// A Pre-processor interface. To transform a string before any classification or understanding is known about it.
    /// </summary>
    public interface IPreProcessor
    {
        /// <summary>
        /// Function to preform the pre-processing.
        /// </summary>
        /// <param name="query">The string to pre-process.</param>
        /// <returns>The pre-processed string.</returns>
        string PreProcess(string query);
    }
}
