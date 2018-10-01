// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Pronounces English texts.
    /// </summary>
    public sealed class EnPronouncer : NativeResourceWrapper
    {
        /// <summary>
        /// This implementation using System.Lazy makes the singleton thread-safe
        /// </summary>
        private static readonly Lazy<EnPronouncer> LazyInstance = new Lazy<EnPronouncer>(() => new EnPronouncer());

        /// <summary>
        /// Prevents a default instance of the <see cref="EnPronouncer"/> class from being created.
        /// </summary>
        private EnPronouncer()
        {
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static EnPronouncer Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        /// <summary>
        /// Pronounce text.
        /// </summary>
        /// <param name="phrase">The text to pronounce.</param>
        /// <returns>The English Pronunciation.</returns>
        public EnPronunciation Pronounce(string phrase)
        {
            if (phrase == null)
            {
                throw new ArgumentNullException("phrase can't be null");
            }

            IntPtr nativePronunciation = IntPtr.Zero;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnPronouncer_Pronounce(this.Native, phrase, out nativePronunciation, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });

            return new EnPronunciation(nativePronunciation);
        }

        /// <summary>
        /// Instantiate the native resource wrapped.
        /// </summary>
        /// <param name="args">The parameter is not used.</param>
        /// <returns>A pointer to the native resource.</returns>
        protected override IntPtr CreateNativeResources(params object[] args)
        {
            IntPtr native = IntPtr.Zero;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnPronouncer_Create(out native, buffer, ref bufferSize);
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
            return EnPronouncer_Delete(native, buffer, ref bufferSize);
        }

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronouncer_Delete(IntPtr ptr, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronouncer_Create(out IntPtr native, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronouncer_Pronounce(IntPtr nativePtr, string phrase, out IntPtr pronunciation, StringBuilder buffer, ref int bufferSize);
    }
}
