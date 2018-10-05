// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingPerfTests
{
    public class Transcription
    {
        /// <summary>
        /// Gets or sets A label to track what made this transcription.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets What was actually heard/spoken (possible ASR/STT errors).
        /// </summary>
        public string Utterance { get; set; }
    }
}