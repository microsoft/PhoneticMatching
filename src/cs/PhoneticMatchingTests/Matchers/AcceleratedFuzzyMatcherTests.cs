// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.PhoneticMatching;
    using Microsoft.PhoneticMatching.Distance;
    using Microsoft.PhoneticMatching.Matchers.FuzzyMatcher;

    [TestClass]
    public class AcceleratedFuzzyMatcherTests : BaseFuzzyMatcherTester
    {
        [TestMethod]
        public void GivenSimilarString_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenSimilarString_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenEmptyTarget_ExpectNull()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenEmptyTarget_ExpectNull(matcher);
        }

        [TestMethod]
        public void GivenDifferentCase_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenDifferentCase_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenStringDistanceAndExtractor_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<TestContact, string>(Targets, this.StringDistance, (target) => target.ToString());
            BaseFuzzyMatcherTester.GivenStringDistanceAndExtractor_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenPhoneticDistance_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, EnPronunciation>(this.TargetStrings, this.PhoneticDistance, phrase => PhraseToDistanceInput(phrase).Pronunciation);
            BaseFuzzyMatcherTester.GivenEnPronunciation_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenHybridDistance_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, DistanceInput>(this.TargetStrings, this.HybridDistance, PhraseToDistanceInput);
            BaseFuzzyMatcherTester.GivenDistanceInput_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenMultipleNearest_ExpectMultiplePositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenMultipleNearest_ExpectMultiplePositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenMultipleNearestWithinThreshold_ExpectMultiplePositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenMultipleNearestWithinThreshold_ExpectMultiplePositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenZeroThresholdAndExactTarget_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenZeroThresholdAndExactTarget_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenZeroThresholdAndWrongTarget_ExpectNull()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, this.StringDistance);
            BaseFuzzyMatcherTester.GivenZeroThresholdAndWrongTarget_ExpectNull(matcher);
        }

        [TestMethod]
        public void GivenContactFuzzyMatcher_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<TestContact, TestContact>(Targets, this.SimpleDistance);
            BaseFuzzyMatcherTester.GivenContactFuzzyMatcher_ExpectPositiveMatch(matcher);
        }

        [TestMethod]
        public void GivenNullTarget_ExpectException()
        {
            var matcher = new AcceleratedFuzzyMatcher<TestContact, string>(Targets, this.StringDistance, contact => contact.FullName);
            BaseFuzzyMatcherTester.GivenNullTarget_ExpectException(matcher);
        }

        [TestMethod]
        public void GivenCustomDistanceDelegate_ExpectPositiveMatch()
        {
            var matcher = new AcceleratedFuzzyMatcher<string, string>(this.TargetStrings, InverseStringDistance);
            BaseFuzzyMatcherTester.GivenCustomDistanceDelegate_ExpectPositiveMatch(matcher);
        }

        /// <summary>
        /// Uses FuzzyMatcher in accelerated mode.
        /// </summary>
        /// <typeparam name="Target">Target type</typeparam>
        /// <typeparam name="Extraction">Extraction type</typeparam>
        private class AcceleratedFuzzyMatcher<Target, Extraction> : FuzzyMatcher<Target, Extraction>
        {
            public AcceleratedFuzzyMatcher(IList<Target> targets, IDistance<Extraction> distance, Func<Target, Extraction> targetToExtraction = null)
                : this(targets, distance.Distance, targetToExtraction)
            {
            }
            
            public AcceleratedFuzzyMatcher(IList<Target> targets, DistanceFunc distance, Func<Target, Extraction> targetToExtraction = null)
                : base(targets, distance, targetToExtraction, true)
            {
            }
        }
    }
}
