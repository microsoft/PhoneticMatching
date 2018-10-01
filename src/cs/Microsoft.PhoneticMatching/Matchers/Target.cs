// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers
{
    /// <summary>
    /// Target of a matcher
    /// </summary>
    /// <typeparam name="T">Type of the target</typeparam>
    public class Target<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Target{T}"/> class.
        /// </summary>
        /// <param name="value">Target value</param>
        /// <param name="phrase">Target phrase</param>
        /// <param name="id">Target identifier</param>
        public Target(T value, string phrase, int id)
        {
            this.Value = value;
            this.Phrase = phrase;
            this.Id = id;
        }

        /// <summary>
        /// Gets the Target element value.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Gets the Target element phrase.
        /// </summary>
        public string Phrase { get; private set; }

        /// <summary>
        /// Gets the Target element identifier.
        /// </summary>
        public int Id { get; private set; }
    }
}
