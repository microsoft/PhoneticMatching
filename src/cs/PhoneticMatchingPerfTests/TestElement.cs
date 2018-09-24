// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingPerfTests
{
    internal class TestElement<T>
    {
        /// <summary>
        /// Gets or sets A unique ID to refer back to this element.
        /// </summary>
        public T Element { get; set; }

        /// <summary>
        /// Gets or sets Test queries with the intent targeting this element in some way.
        /// </summary>
        public TestQuery[] Queries { get; set; }
    }
}
