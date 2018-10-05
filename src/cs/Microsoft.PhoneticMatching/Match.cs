// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching
{
    /// <summary>
    /// A matched element with its distance score.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public class Match<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Match{T}"/> class.
        /// </summary>
        /// <param name="element">the element wrapped</param>
        /// <param name="distance">the distance with query target</param>
        public Match(T element, double distance)
        {
            this.Element = element;
            this.Distance = distance;
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        public T Element { get; private set; }

        /// <summary>
        /// Gets the distance with the target matched.
        /// </summary>
        public double Distance { get; private set; }
    }
}
