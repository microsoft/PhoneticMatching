// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Matchers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Abstract class to define static imports for generic fuzzy matcher.
    /// </summary>
    public abstract class FuzzyMatcherBase : NativeResourceWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyMatcherBase"/> class.
        /// </summary>
        /// <param name="args">Parameter(s) required to initialize the native object if any.</param>
        public FuzzyMatcherBase(params object[] args) : base(args)
        {
        }

        /// <summary>
        /// Delegate type passed to native code to access the managed objects using their indexes and compute distance on them.
        /// </summary>
        /// <param name="firstIdx">Index of the first managed object</param>
        /// <param name="secondIdx">Index of the second managed object.</param>
        /// <returns>The distance between the first and second managed objects.</returns>
        protected delegate double DistanceDelegate(int firstIdx, int secondIdx);

        [DllImport("maluubaspeech-csharp.dll")]
        protected static extern NativeResult FuzzyMatcher_Create(int count, DistanceDelegate distance, bool isAccelerated, out IntPtr fuzzyMatcher, StringBuilder errorMsg, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        protected static extern NativeResult FuzzyMatcher_FindNearestWithin(IntPtr native, int count, double limit, [In, Out] int[] nearestIdx, [In, Out] double[] distances, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        protected static extern NativeResult AcceleratedFuzzyMatcher_FindNearestWithin(IntPtr native, int count, double limit, [In, Out] int[] nearestIdx, [In, Out] double[] distances, StringBuilder buffer, ref int bufferSize);

        /// <summary>
        /// Delete the native pointer using the type specified in native bindings.
        /// </summary>
        /// <param name="native">Pointer to the native object.</param>
        /// <param name="buffer">Buffer for any error message</param>
        /// <param name="bufferSize">Size of the buffer, to be adjusted if error doesn't fit the current size.</param>
        /// <returns>The result code from native library.</returns>
        protected override NativeResult NativeDelete(IntPtr native, StringBuilder buffer, ref int bufferSize)
        {
            return FuzzyMatcher_Delete(native, buffer, ref bufferSize);
        }

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult FuzzyMatcher_Delete(IntPtr ptr, StringBuilder buffer, ref int bufferSize);
    }
}
