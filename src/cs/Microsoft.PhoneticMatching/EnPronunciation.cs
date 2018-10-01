// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// English pronunciation.
    /// </summary>
    public class EnPronunciation : NativeResourceWrapper
    {
        private IList<Phone> phones = null;
        private string ipa = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnPronunciation"/> class.
        /// </summary>
        /// <param name="ptr">Pointer to the native resource wrapped to delete on dispose.</param>
        public EnPronunciation(IntPtr ptr) : base(ptr)
        {
        }

        /// <summary>
        /// Gets the IPA representation.
        /// </summary>
        public string Ipa
        {
            get
            {
                if (this.ipa == null)
                {
                    NativeResourceWrapper.CallNative((buffer) =>
                    {
                        int bufferSize = NativeResourceWrapper.BufferSize;
                        IntPtr ipaPtr = Marshal.AllocHGlobal(NativeResourceWrapper.BufferSize);
                        var code = EnPronunciation_Ipa(this.Native, ipaPtr, buffer, ref bufferSize);
                        if (code == NativeResult.BufferTooSmall)
                        {
                            Marshal.FreeHGlobal(ipaPtr);
                            ipaPtr = Marshal.AllocHGlobal(NativeResourceWrapper.BufferSize);
                            code = EnPronunciation_Ipa(this.Native, ipaPtr, buffer, ref bufferSize);
                        }

                        if (code == NativeResult.Success)
                        {
                            this.ipa = Marshal.PtrToStringUni(ipaPtr);
                        }

                        Marshal.FreeHGlobal(ipaPtr);
                        
                        NativeResourceWrapper.BufferSize = bufferSize;
                        return code;
                    });
                }

                return this.ipa;
            }
        }

        /// <summary>
        /// Gets the phones.
        /// </summary>
        public IList<Phone> Phones
        {
            get
            {
                if (this.phones == null)
                {
                    int count = 0;

                    NativeResourceWrapper.CallNative((buffer) =>
                    {
                        int bufferSize = NativeResourceWrapper.BufferSize;
                        var result = EnPronunciation_Count(this.Native, out count, buffer, ref bufferSize);
                        NativeResourceWrapper.BufferSize = bufferSize;
                        return result;
                    });

                    var phones = new PhoneFields[count];

                    NativeResourceWrapper.CallNative((buffer) =>
                    {
                        int bufferSize = NativeResourceWrapper.BufferSize;
                        var result = EnPronunciation_Phones(this.Native, phones, buffer, ref bufferSize);
                        NativeResourceWrapper.BufferSize = bufferSize;
                        return result;
                    });

                    this.phones = new List<Phone>();
                    foreach (var phoneFields in phones)
                    {
                        this.phones.Add(new Phone(
                            phoneFields.Type, 
                            phoneFields.Phonation,
                            phoneFields.Place, 
                            phoneFields.Manner, 
                            phoneFields.Height, 
                            phoneFields.Backness, 
                            phoneFields.Roundedness, 
                            phoneFields.IsRhotic != 0, 
                            phoneFields.IsSyllabic != 0));
                    }
                }

                return this.phones;
            }
        }

        /// <summary>
        /// Constructs a <see cref="EnPronunciation"/> from an IPA string. E.g. (phonetic) "fənɛtɪk".
        /// </summary>
        /// <param name="ipa">The IPA string.</param>
        /// <returns>The English pronunciation.</returns>
        public static EnPronunciation FromIpa(string ipa)
        {
            var utf8 = Encoding.UTF8.GetBytes(ipa);
            IntPtr nativePronunciation = IntPtr.Zero;
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnPronunciation_FromIpa(utf8, out nativePronunciation, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });
            return new EnPronunciation(nativePronunciation);
        }

        /// <summary>
        /// Constructs a <see cref="EnPronunciation"/> from an ARPABET string array. E.g. (phonetic) ["F","AH","N","EH","T","IH","K"].
        /// </summary>
        /// <param name="arpabet">The ARPABET array.</param>
        /// <returns>The English pronunciation.</returns>
        public static EnPronunciation FromArpabet(IList<string> arpabet)
        {
            IntPtr native = IntPtr.Zero;

            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = EnPronunciation_FromArpabet(arpabet.ToArray(), arpabet.Count, out native, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });

            return new EnPronunciation(native);
        }

        /// <summary>
        /// Native resources is created through FromIpa/FromArpabet.
        /// </summary>
        /// <param name="args">The parameter is not used.</param>
        /// <returns>Nothing. There is no public default constructor for <see cref="EnPronunciation"/>. We use static initializers or Pronouncer instead.</returns>
        protected override IntPtr CreateNativeResources(params object[] args)
        {
            throw new NotImplementedException();
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
            return EnPronunciation_Delete(native, buffer, ref bufferSize);
        }

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronunciation_Delete(IntPtr ptr, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronunciation_Ipa(IntPtr ptr, IntPtr ipa, StringBuilder errorBuffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronunciation_FromIpa(byte[] ipa, out IntPtr nativePronunciation, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronunciation_FromArpabet(string[] head, int count, out IntPtr unmanaged, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronunciation_Count(IntPtr pronunciation, out int count, StringBuilder buffer, ref int bufferSize);

        [DllImport("maluubaspeech-csharp.dll")]
        private static extern NativeResult EnPronunciation_Phones(IntPtr pronunciation, [In, Out] PhoneFields[] phones, StringBuilder buffer, ref int bufferSize);

        /// <summary>
        /// Wraps all phone fields to marshal and retrieve from native code.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PhoneFields
        {
            public PhoneType Type;
            public Phonation Phonation;
            public PlaceOfArticulation Place;
            public MannerOfArticulation Manner;
            public VowelHeight Height;
            public VowelBackness Backness;
            public VowelRoundedness Roundedness;
            public int IsRhotic;
            public int IsSyllabic;
        }
    }
}
