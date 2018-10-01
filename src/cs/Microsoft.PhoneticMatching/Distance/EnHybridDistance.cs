// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Distance
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Compute the  phonetic distance between English pronunciations.
    /// </summary>
    public class EnHybridDistance : NativeResourceWrapper, IDistance<DistanceInput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnHybridDistance"/> class.
        /// </summary>
        /// <param name="phoneticWeightPercentage">Between 0 and 1. 
        /// Weighting trade-off between the phonetic distance and the lexical distance scores. 
        /// 1 meaning 100% phonetic score and 0% lexical score.</param>
        public EnHybridDistance(double phoneticWeightPercentage)
            : base(phoneticWeightPercentage)
        {
            this.PhoneticWeightPercentage = phoneticWeightPercentage;
        }

        /// <summary>
        /// Gets the phonetic weight percentage.
        /// </summary>
        public double PhoneticWeightPercentage { get; private set; }

        /// <summary>
        /// Computes an English phonetic distance metric.
        /// </summary>
        /// <param name="first">First pronunciation to compare</param>
        /// <param name="second">Second pronunciation to compare</param>
        /// <returns>The english phonetic distance between a and b</returns>
        public double Distance(DistanceInput first, DistanceInput second)
        {
            if (first == null || second == null)
            {
                throw new ArgumentNullException("distance input can't be null");
            }

            if (first.Phrase == null || first.Pronunciation == null)
            {
                throw new ArgumentException("First distance input is invalid. Phrase or Punctuation is null");
            }

            if (second.Phrase == null || second.Pronunciation == null)
            {
                throw new ArgumentException("Second distance input is invalid. Phrase or Punctuation is null");
            }

            double distance = 0;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnHybridDistance_Distance(this.Native, first.Phrase, first.Pronunciation.Native, second.Phrase, second.Pronunciation.Native, out distance, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });
            return distance;
        }

        /// <summary>
        /// Instantiate the native resource wrapped.
        /// </summary>
        /// <param name="args">phonetic Weight Percentage</param>
        /// <returns>A pointer to the native resource.</returns>
        protected override IntPtr CreateNativeResources(params object[] args)
        {
            double phoneticWeightPercentage = (double)args[0];
            if (phoneticWeightPercentage > 1 || phoneticWeightPercentage < 0)
            {
                throw new ArgumentOutOfRangeException("phoneticWeightPercentage must be between 0 and 1.");
            }

            IntPtr native = IntPtr.Zero;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnHybridDistance_Create(phoneticWeightPercentage, out native, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });
            return native;
        }

        /// <summary>
        /// Delete the native pointer using the type specified in native bindings.
        /// </summary>
        /// <param name="native">Pointer to the native object.</param>
        /// <param name="buffer">Buffer for any error message</param>
        /// <param name="bufferSize">Size of the buffer, to be adjusted if error doesn't fit the current size.</param>
        /// <returns>The result code from native library.</returns>
        protected override NativeResult NativeDelete(IntPtr native, StringBuilder buffer, ref int bufferSize)
        {
            return EnHybridDistance_Delete(native, buffer, ref bufferSize);
        }

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnHybridDistance_Delete(IntPtr ptr, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnHybridDistance_Create(double phoneticWeightPercentage, out IntPtr native, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnHybridDistance_Distance(IntPtr native, string a_string, IntPtr a_pronunciation, string b_string, IntPtr b_pronunciation, out double distance, StringBuilder buffer, ref int bufferSize);
    }
}
