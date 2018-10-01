// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Distance
{
    using System;

    /// <summary>
    /// Input object for <see cref="Distance.EnHybridDistance"/>. Hold the text and the pronunciation of that text.
    /// </summary>
    public class DistanceInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceInput"/> class.
        /// </summary>
        /// <param name="phrase">the text to compute distance on</param>
        /// <param name="pronunciation">the pronunciation to compute distance on</param>
        public DistanceInput(string phrase, EnPronunciation pronunciation)
        {
            this.Phrase = phrase;
            this.Pronunciation = pronunciation;
        }

        /// <summary>
        /// Gets the text to compute distance on.
        /// </summary>
        public string Phrase { get; private set; }

        /// <summary>
        /// Gets the pronunciation to compute distance on.
        /// </summary>
        public EnPronunciation Pronunciation { get; private set; }
    }
}
