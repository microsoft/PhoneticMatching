// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Distance
{
    /// <summary>
    /// Distance interface. Distance object are used to compute distance between two objects.
    /// </summary>
    /// <typeparam name="T">Type of elements between which we compute distance.</typeparam>
    public interface IDistance<T>
    {
        /// <summary>
        /// Computes the distance between first and second.
        /// </summary>
        /// <param name="first">First element.</param>
        /// <param name="second">Second element.</param>
        /// <returns>The distance between first and second.</returns>
        double Distance(T first, T second);
    }
}
