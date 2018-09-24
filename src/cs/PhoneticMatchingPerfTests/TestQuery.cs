// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingPerfTests
{
    public class TestQuery
    {
        /// <summary>
        /// Gets or sets What the intention is. What should be heard or what was read.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets The records for this test query. What was actually heard or what was written.
        /// </summary>
        public Transcription[] Transcriptions { get; set; }
    }
}