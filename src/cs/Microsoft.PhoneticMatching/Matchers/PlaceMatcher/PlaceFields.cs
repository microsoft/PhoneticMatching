// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.PlaceMatcher
{
    using System.Collections.Generic;

    /// <summary>
    /// Fields made available from the user defined Place object for pronunciation and distance functions.
    /// </summary>
    public class PlaceFields
    {
        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets The address of the place.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets The tags/categories defining the place.
        /// </summary>
        public IList<string> Types { get; set; }
    }
}
