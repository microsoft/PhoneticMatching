// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;

    [TestClass]
    public class EnPronouncerTests
    {
        private EnPronouncer pronouncer = new EnPronouncer();

        [TestMethod]
        public void GivenPronunciation_ExpectPositiveMatch()
        {
            var pronunciation = this.pronouncer.Pronounce("This, is a test.");
            Assert.AreEqual("ðɪsɪzətɛst", pronunciation.Ipa);
        }

        [TestMethod]
        public void GivenNullArgument_ExpectException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var pronunciation = this.pronouncer.Pronounce(null);
            });
        }
    }
}
