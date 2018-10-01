// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Tokenizer
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Tokenizer that splits on whitespace.
    /// </summary>
    public class WhitespaceTokenizer : SplittingTokenizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhitespaceTokenizer"/> class.
        /// </summary>
        public WhitespaceTokenizer() : base(new Regex(@"\s+"))
        {
        }
    }
}
