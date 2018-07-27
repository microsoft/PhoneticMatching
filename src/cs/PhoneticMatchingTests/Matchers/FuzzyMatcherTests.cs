// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;
    using PhoneticMatching.Distance;
    using PhoneticMatching.Matchers.FuzzyMatcher;

    [TestClass]
    public class FuzzyMatcherTests : BaseFuzzyMatcherTester
    {
        [TestMethod]
        public void GivenSimilarString_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenSimilarString_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenEmptyTarget_ExpectNull()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenEmptyTarget_ExpectNull(matcher);
        }

        [TestMethod]
        public void GivenDifferentCase_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenDifferentCase_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenStringDistanceAndExtractor_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<TestContact, string>(Targets, this.StringDistance, (target) => target.ToString());
            BaseFuzzyMatcherTester.GivenStringDistanceAndExtractor_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenPhoneticDistance_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, EnPronunciation>(this.TargetStrings, this.PhoneticDistance, phrase => PhraseToDistanceInput(phrase).Pronunciation);
            BaseFuzzyMatcherTester.GivenEnPronunciation_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenHybridDistance_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, DistanceInput>(this.TargetStrings, this.HybridDistance, PhraseToDistanceInput);
            BaseFuzzyMatcherTester.GivenDistanceInput_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenMultipleNearest_ExpectMultiplePositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenMultipleNearest_ExpectMultiplePositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenMultipleNearestWithinThreshold_ExpectMultiplePositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenMultipleNearestWithinThreshold_ExpectMultiplePositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenZeroThresholdAndExactTarget_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenZeroThresholdAndExactTarget_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenZeroThresholdAndWrongTarget_ExpectNull()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenZeroThresholdAndWrongTarget_ExpectNull(matcher);
        }

        [TestMethod]
        public void GivenContactFuzzyMatcher_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<TestContact, TestContact>(Targets, this.SimpleDistance);
            BaseFuzzyMatcherTester.GivenContactFuzzyMatcher_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenNullTarget_ExpectException()
        {
            var matcher = new FuzzyMatcher<TestContact, string>(Targets, this.StringDistance, contact => contact.FullName);
            BaseFuzzyMatcherTester.GivenNullTarget_ExpectException(matcher);
        }

        [TestMethod]
        public void GivenCustomDistanceDelegate_ExpectPositiveMatch()
        {
            var matcher = new FuzzyMatcher<string, string>(this.TargetStrings, InverseStringDistance);
            BaseFuzzyMatcherTester.GivenCustomDistanceDelegate_ExpectPositiveMatch(matcher);
        }
    }
}
