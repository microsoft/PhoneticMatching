// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching.Distance
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// String distance utility. 
    /// </summary>
    public class StringDistance : NativeResourceWrapper, IDistance<string>
    {
        /// <summary>
        /// Computes a string edit distance metric.
        /// </summary>
        /// <param name="first">First string to compare</param>
        /// <param name="second">Second string to compare</param>
        /// <returns>Returns the distance between string a and b.</returns>
        public double Distance(string first, string second)
        {
            if (first == null || second == null)
            {
                throw new ArgumentNullException("distance input can't be null");
            }

            double distance = 0;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = StringDistance_Distance(this.Native, first, second, out distance, buffer, ref bufferSize);
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
                var result = StringDistance_Create(out native, buffer, ref bufferSize);
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
            return StringDistance_Delete(native, buffer, ref bufferSize);
        }

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult StringDistance_Delete(IntPtr ptr, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult StringDistance_Create(out IntPtr native, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult StringDistance_Distance(IntPtr ptr, string s1, string s2, out double distance, StringBuilder buffer, ref int bufferSize);
    }
}
