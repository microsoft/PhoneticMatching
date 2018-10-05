// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching
{
    using System;

    /// <summary>
    /// Used to keep track of the last exception that occurred during a managed callback was invoked from native code. Otherwise, native code swallows the exception.
    /// </summary>
    internal static class ManagedCallback
    {
        /// <summary>
        /// Gets or sets the last exception that occurred during a managed callback was invoked from native code.
        /// </summary>
        public static Exception LastError { get; set; }
    }
}