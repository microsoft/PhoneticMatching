// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Tokenizer
{
    /// <summary>
    /// The substring token of the original string with its interval location.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="value">Value of the token.</param>
        /// <param name="interval">Interval of the value.</param>
        public Token(string value, Interval interval)
        {
            this.Value = value;
            this.Interval = interval;
        }

        /// <summary>
        /// Gets the value of the token.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the interval of the token.
        /// </summary>
        public Interval Interval { get; private set; }
    }
}
