// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Distance
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;
    using PhoneticMatching.Distance;

    [TestClass]
    public class EnHybridDistanceTests : BaseDistanceTester<DistanceInput>
    {
        [TestMethod]
        public void GivenHybridDistance_ExpectPhoneticWeightPercentage()
        {
            var hybrid = this.Distance as EnHybridDistance;
            Assert.AreEqual(0.7, hybrid.PhoneticWeightPercentage);
        }

        [TestMethod]
        public void GivenExactString_ExpectPositiveMatch()
        {
            const string Phrase = "This, is a test.";
            var inputA = CreateInput(Phrase);
            var inputB = CreateInput(Phrase);
            var dist = this.Distance.Distance(inputA, inputB);
            Assert.AreEqual(0, dist);
        }

        [TestMethod]
        public void GivenHybridDistance_ExpectValidDistance()
        {
            const string PhraseA = "aaa";
            const string PhraseB = "bbb";

            var inputA = CreateInput(PhraseA);
            var inputB = CreateInput(PhraseB);
            var inputEmpty = CreateInput(string.Empty);

            var dist = this.Distance.Distance(inputA, inputA);
            Assert.AreEqual(0, dist);
            dist = this.Distance.Distance(inputA, inputB);
            Assert.IsTrue(dist > 0);
            dist = this.Distance.Distance(inputEmpty, inputEmpty);
            Assert.AreEqual(0, dist);
        }

        [TestMethod]
        public void GivenSimilarAndDifferent_ExpectGreaterDistanceForDifferent()
        {
            var inputI = CreateInput("aaiaa");
            var inputE = CreateInput("aaeaa");
            var inputZ = CreateInput("zzrzz");

            var distSimilar = this.Distance.Distance(inputI, inputE);
            var distDifferent = this.Distance.Distance(inputI, inputZ);
            Assert.IsTrue(distDifferent > distSimilar);
        }

        [TestMethod]
        public void GivenInvalidInput_ExpectException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var input = new DistanceInput(null, null);
                var dist = this.Distance.Distance(input, input);
            });
        }

        [TestMethod]
        public void GivenEmptyInput_ExpectException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var input = new DistanceInput(string.Empty, null);
                var dist = this.Distance.Distance(input, input);
            });
        }

        [TestMethod]
        public void GivenOutOfBoundLower_ExpectException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                var distance = new EnHybridDistance(-0.1);
            });
        }

        [TestMethod]
        public void GivenOutOfBoundUpper_ExpectException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                var distance = new EnHybridDistance(2);
            });
        }

        protected override IDistance<DistanceInput> CreateDistanceOperator()
        {
            return new EnHybridDistance(0.7);
        }

        private static DistanceInput CreateInput(string phrase)
        {
            EnPronouncer pronouncer = new EnPronouncer();

            return new DistanceInput(phrase, pronouncer.Pronounce(phrase));
        }
    }
}
