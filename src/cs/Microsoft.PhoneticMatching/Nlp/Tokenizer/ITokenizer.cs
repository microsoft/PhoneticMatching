// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Tokenizer
{
    using System.Collections.Generic;

    /// <summary>
    /// Tokenizer interface for strings.
    /// </summary>
    public interface ITokenizer
    {
        /// <summary>
        /// Tokenize the query.
        /// </summary>
        /// <param name="query">Query to tokenize.</param>
        /// <returns>Collection of tokens.</returns>
        IList<Token> Tokenize(string query);
    }
}
