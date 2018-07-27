// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;

    [TestClass]
    public class NativeResourceWrapperTests
    {
        [TestMethod]
        public void GivenBufferTooSmall_ExpectErrorCode()
        {
            TestNativeWrapper.TestBufferTooSmall();
        }

        private abstract class TestNativeWrapper : NativeResourceWrapper
        {
            public static void TestBufferTooSmall()
            {
                double distance;

                // 2 is obviously too small to contain any error
                const int InitialBufferSize = 2;
                int bufferSize = InitialBufferSize;
                StringBuilder buffer = new StringBuilder(bufferSize);

                // IntPtr.Zero is a null reference exception
                var code = StringDistance_Distance(IntPtr.Zero, "123", "456", out distance, buffer, ref bufferSize);

                Assert.AreEqual(NativeResult.BufferTooSmall, code);
                Assert.IsTrue(bufferSize > InitialBufferSize);
                Assert.AreEqual(string.Empty, buffer.ToString());

                // use the new buffer size returned by native
                buffer.Capacity = bufferSize;
                code = StringDistance_Distance(IntPtr.Zero, "123", "456", out distance, buffer, ref bufferSize);

                Assert.AreEqual(NativeResult.InvalidParameter, code);
                Assert.AreEqual("pointer is null", buffer.ToString());
            }
            
            // Random dll import to test 
            [DllImport("maluubaspeech-csharp.dll")]
            private static extern NativeResult StringDistance_Distance(IntPtr ptr, string s1, string s2, out double distance, StringBuilder buffer, ref int bufferSize);
        }
    }
}
