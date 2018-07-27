// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;

    [TestClass]
    public class EnPronunciationTests
    {
        [TestMethod]
        public void GivenIpaFromArpabet_ExpectPositiveMatch()
        {
            var arpabet = EnPronunciation.FromArpabet(new string[] { "dh", "ih1", "s", "ih1", "z", "ax0", "t", "eh1", "s", "t" });
            Assert.IsTrue(arpabet.Phones.Count > 0);
            Assert.AreEqual("ðɪsɪzətɛst", arpabet.Ipa);
        }

        [TestMethod]
        public void GivenIpaFromIpa_ExpectPositiveMatch()
        {
            var ipa = EnPronunciation.FromIpa("ðɪsɪzətɛst");
            Assert.IsTrue(ipa.Phones.Count > 0);
            Assert.AreEqual("ðɪsɪzətɛst", ipa.Ipa);
        }

        [TestMethod]
        public void GivenPronunciationFromArpabet_ExpectPositiveMatch()
        {
            var pron = EnPronunciation.FromArpabet(new string[] { "P", "R", "OW0", "N", "AH2", "N", "S", "IY0", "EY1", "SH", "AX0", "N" });
            Assert.IsTrue(pron.Phones.Count > 0);
            Assert.AreEqual("proʊ̯nʌnsieɪ̯ʃən", pron.Ipa);

            // p
            var phone = pron.Phones[0];
            Assert.AreEqual(PhoneType.Consonant, phone.Type);
            Assert.AreEqual(Phonation.Voiceless, phone.Phonation);
            Assert.AreEqual(PlaceOfArticulation.Bilabial, phone.Place);
            Assert.AreEqual(MannerOfArticulation.Plosive, phone.Manner);
            Assert.IsFalse(phone.IsSyllabic);
            Assert.IsNull(phone.Height);
            Assert.IsNull(phone.Backness);
            Assert.IsNull(phone.Roundedness);
            Assert.IsNull(phone.IsRhotic);

            // o
            phone = pron.Phones[2];
            Assert.AreEqual(PhoneType.Vowel, phone.Type);
            Assert.AreEqual(Phonation.Modal, phone.Phonation);
            Assert.AreEqual(VowelHeight.CloseMid, phone.Height);
            Assert.AreEqual(VowelBackness.Back, phone.Backness);
            Assert.AreEqual(VowelRoundedness.Rounded, phone.Roundedness);
            Assert.IsTrue(phone.IsSyllabic);

            // ʊ̯
            phone = pron.Phones[3];
            Assert.AreEqual(PhoneType.Vowel, phone.Type);
            Assert.AreEqual(Phonation.Modal, phone.Phonation);
            Assert.AreEqual(VowelHeight.NearClose, phone.Height);
            Assert.AreEqual(VowelBackness.NearBack, phone.Backness);
            Assert.AreEqual(VowelRoundedness.Rounded, phone.Roundedness);
            Assert.IsFalse(phone.IsSyllabic);
            Assert.IsNull(phone.Place);
            Assert.IsNull(phone.Manner);
        }

        [TestMethod]
        public void GivenInvalidSpaceInArpabet_ExpectException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var arpabet = EnPronunciation.FromArpabet(new string[] { "F", "B ", "N", "EH", "T", "IH", "K" });
            });
        }

        [TestMethod]
        public void GivenNullArgument_FromIpa_ExpectException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var arpabet = EnPronunciation.FromArpabet(null);
            });
        }

        [TestMethod]
        public void GivenNullArgument_FromArpabet_ExpectException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var ipa = EnPronunciation.FromIpa(null);
            });
        }

        [TestMethod]
        public void GivenInvalidIpa_FromIpa_ExpectException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var ipa = EnPronunciation.FromIpa("This is not an IPA");
            });
        }
    }
}
