// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Nlp.Tokenizer
{
    /// <summary>
    /// An Interval holds the first and last index bounds.
    /// </summary>
    public class Interval
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> class.
        /// </summary>
        /// <param name="first">Starting index (inclusive).</param>
        /// <param name="last">Ending index (exclusive).</param>
        public Interval(int first, int last)
        {
            this.First = first;
            this.Last = last;
        }

        /// <summary>
        /// Gets the Starting index (inclusive).
        /// </summary>
        public int First { get; private set; }

        /// <summary>
        /// Gets the Ending index (exclusive).
        /// </summary>
        public int Last { get; private set; }

        /// <summary>
        /// Gets the length of the token.
        /// </summary>
        public int Length
        {
            get
            {
                return this.Last - this.First;
            }
        }
    }
}
