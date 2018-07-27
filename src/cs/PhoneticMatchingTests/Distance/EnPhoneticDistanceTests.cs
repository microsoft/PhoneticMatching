// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Distance
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;
    using PhoneticMatching.Distance;

    [TestClass]
    public class EnPhoneticDistanceTests : BaseDistanceTester<EnPronunciation>
    {
        /// <summary>
        /// Sam pasupalak
        /// </summary>
        private EnPronunciation sam = EnPronunciation.FromIpa("sæmpɑsupələk");

        /// <summary>
        /// Santa super black
        /// </summary>
        private EnPronunciation santa = EnPronunciation.FromIpa("sæntəsupɝblæk");

        /// <summary>
        ///  Samples pollux
        /// </summary>
        private EnPronunciation samples = EnPronunciation.FromIpa("sæmpəlzpɑləks");

        [TestMethod]
        public void GivenSamePronunciation_ExpectZeroDistance()
        {
            const string ThisIsATest = "ðɪsɪzətɛst";
            var test = EnPronunciation.FromIpa(ThisIsATest);
            var dist = this.Distance.Distance(test, test);
            Assert.AreEqual(0, dist);
        }

        /// <summary>
        /// Check identity of indiscernibles
        /// </summary>
        [TestMethod]
        public void GivenPhoneticDistances_ExpectIdentity()
        {
            Assert.AreEqual(0, this.Distance.Distance(this.sam, this.sam));
            Assert.AreEqual(0, this.Distance.Distance(this.santa, this.santa));
            Assert.AreEqual(0, this.Distance.Distance(this.samples, this.samples));
        }

        /// <summary>
        /// Check symmetry
        /// </summary>
        [TestMethod]
        public void GivenPhoneticDistances_ExpectSymmetry()
        {
            Assert.AreEqual(this.Distance.Distance(this.sam, this.santa), this.Distance.Distance(this.santa, this.sam));
            Assert.AreEqual(this.Distance.Distance(this.sam, this.samples), this.Distance.Distance(this.samples, this.sam));
            Assert.AreEqual(this.Distance.Distance(this.samples, this.santa), this.Distance.Distance(this.santa, this.samples));
        }

        /// <summary>
        /// Check triangle inequality
        /// </summary>
        [TestMethod]
        public void GivenPhoneticDistances_ExpectInequality()
        {
            Assert.IsTrue(this.Distance.Distance(this.sam, this.samples) < this.Distance.Distance(this.sam, this.santa) + this.Distance.Distance(this.santa, this.samples));
            Assert.IsTrue(this.Distance.Distance(this.sam, this.santa) < this.Distance.Distance(this.sam, this.samples) + this.Distance.Distance(this.samples, this.santa));
            Assert.IsTrue(this.Distance.Distance(this.santa, this.samples) < this.Distance.Distance(this.santa, this.sam) + this.Distance.Distance(this.sam, this.samples));
        }

        /// <summary>
        /// Check performance
        /// </summary>
        [TestMethod]
        public void GivenPhoneticDistances_ExpectPerformance()
        {
            Assert.IsTrue(this.Distance.Distance(this.sam, this.santa) < this.Distance.Distance(this.sam, this.samples));
            Assert.IsTrue(this.Distance.Distance(this.sam, this.samples) < this.Distance.Distance(this.santa, this.samples));
        }

        protected override IDistance<EnPronunciation> CreateDistanceOperator()
        {
            return new EnPhoneticDistance();
        }
    }
}
