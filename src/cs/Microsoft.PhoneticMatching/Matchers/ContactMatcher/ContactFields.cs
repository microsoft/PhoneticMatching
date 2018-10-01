// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers.ContactMatcher
{
    using System.Collections.Generic;

    /// <summary>
    /// Fields made available from the user defined Contact object for pronunciation and distance functions.
    /// </summary>
    public class ContactFields
    {
        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the aliases the contact also goes by.
        /// </summary>
        public IList<string> Aliases { get; set; }
    }
}
