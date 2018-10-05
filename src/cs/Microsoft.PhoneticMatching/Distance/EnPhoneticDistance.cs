// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Distance
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// English phonetic distance utility.
    /// </summary>
    public class EnPhoneticDistance : NativeResourceWrapper, IDistance<EnPronunciation>
    {
        /// <summary>
        /// Computes an English phonetic distance metric.
        /// </summary>
        /// <param name="first">First pronunciation to compare</param>
        /// <param name="second">Second pronunciation to compare</param>
        /// <returns>The english phonetic distance between a and b</returns>
        public double Distance(EnPronunciation first, EnPronunciation second)
        {
            if (first == null || second == null)
            {
                throw new ArgumentNullException("distance input can't be null");
            }

            double distance = 0;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnPhoneticDistance_Distance(this.Native, first.Native, second.Native, out distance, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });
            return distance;
        }

        /// <summary>
        /// Instantiate the native resource wrapped
        /// </summary>
        /// <param name="args">The parameter is not used.</param>
        /// <returns>A pointer to the native resource.</returns>
        protected override IntPtr CreateNativeResources(params object[] args)
        {
            IntPtr native = IntPtr.Zero;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnPhoneticDistance_Create(out native, buffer, ref bufferSize);
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
            return EnPhoneticDistance_Delete(native, buffer, ref bufferSize);
        }

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPhoneticDistance_Delete(IntPtr ptr, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPhoneticDistance_Create(out IntPtr native, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPhoneticDistance_Distance(IntPtr native, IntPtr first, IntPtr second, out double distance, StringBuilder buffer, ref int bufferSize);
    }
}
