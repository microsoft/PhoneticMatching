// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;
    using PhoneticMatching.Distance;
    using PhoneticMatching.Matchers;
    using PhoneticMatching.Matchers.FuzzyMatcher;
    using PhoneticMatching.Matchers.FuzzyMatcher.Normalized;

    [TestClass]
    public class NormalizedMatcherTests
    {
        private const string Zero = "zero";
        private const string One = "one";
        private const string Two = "two";
        private const string Three = "three";
        private const string Four = "four";
        private const string Five = "five";
        private const string Six = "six";
        private const string Seven = "seven";
        private const string Eight = "eight";
        private const string Nine = "nine";
        private const double PhoneticWeightPercentage = 0.7;
        private static readonly string[] StringTargets = new string[]
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine
        };

        private static readonly MyTargetType[] Targets = new MyTargetType[]
        {
            new MyTargetType(Zero, 0),
            new MyTargetType(One, 1),
            new MyTargetType(Two, 2),
            new MyTargetType(Three, 3),
            new MyTargetType(Four, 4),
            new MyTargetType(Five, 5),
            new MyTargetType(Six, 6),
        };

        private static StringFuzzyMatcher<string> stringMatcher = new StringFuzzyMatcher<string>(StringTargets);
        private static EnPhoneticFuzzyMatcher<string> phoneticMatcher = new EnPhoneticFuzzyMatcher<string>(StringTargets);
        private static EnHybridFuzzyMatcher<string> hybridMatcher = new EnHybridFuzzyMatcher<string>(StringTargets, PhoneticWeightPercentage);
        private static EnPronouncer pronouncer = EnPronouncer.Instance;

        private static Func<string, string> queryToString = (query) => query;
        private static Func<string, EnPronunciation> queryToPronunciation = (query) => pronouncer.Pronounce(query);
        private static Func<string, DistanceInput> queryToDistanceInput = (query) => new DistanceInput(query, pronouncer.Pronounce(query));

        private static IFuzzyMatcher<string, string>[] normalizedMatchers = new IFuzzyMatcher<string, string>[]
        {
            stringMatcher,
            phoneticMatcher,
            hybridMatcher
        };

        /// <summary>
        /// Free memory.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            stringMatcher = null;
            phoneticMatcher = null;
            hybridMatcher = null;
            normalizedMatchers = null;
        }

        [TestMethod]
        public void GivenSameLengthDistance_ExpectPositiveMatch()
        {
            const string Query = "zoro";
            var match = stringMatcher.FindNearest(Query);
            Assert.AreEqual(Query.Length, match.Element.Length);
            Assert.IsTrue(match.Distance < 1 && match.Distance > 0);
        }

        [TestMethod]
        public void GivenTarget_ExpectPositiveMatch()
        {
            foreach (var matcher in normalizedMatchers)
            {
                var match = matcher.FindNearest(Three);
                Assert.AreEqual(Three, match.Element);
                Assert.AreEqual(0, match.Distance);
            }
        }

        [TestMethod]
        public void GivenSimilarTarget_ExpectCloseMatch()
        {
            const string Query = "code";
            foreach (var matcher in normalizedMatchers)
            {
                var match = matcher.FindNearest(Query);
                Assert.IsTrue(match.Distance < 1 && match.Distance > 0);
            }
        }

        [TestMethod]
        public void GivenHugeTarget_ExpectFarMatch()
        {
            string[] target = new string[] { "This is clearly not a number. It doesn't even sound like a number." };
            IFuzzyMatcher<string, string>[] matchers = new IFuzzyMatcher<string, string>[]
            {
                new StringFuzzyMatcher<string>(target),
                new EnPhoneticFuzzyMatcher<string>(target),
                new EnHybridFuzzyMatcher<string>(target, PhoneticWeightPercentage),
            };

            foreach (var matcher in matchers)
            {
                var match = matcher.FindNearest(One);
                Assert.IsTrue(match.Distance > 1, $"{matcher.GetType().Name} failed with distance={match.Distance}");
            }
        }

        [TestMethod]
        public void GivenNullQuery_ExpectException()
        {
            foreach (var matcher in normalizedMatchers)
            {
                Assert.ThrowsException<ArgumentNullException>(() =>
                {
                    matcher.FindNearest(null);
                });
            }
        }

        [TestMethod]
        public void GivenEmptyQuery_ExpectNormalizedResult()
        {
            const string Query = "random string";
            string[] targets = new string[] { Query };
            var matcher = new StringFuzzyMatcher<string>(targets);

            var match = matcher.FindNearest(string.Empty);

            // this is the regular matcher equivalent (an empty string threshold scale is 1)
            Assert.AreEqual(Query.Length, match.Distance);
            Assert.AreEqual(Query, match.Element);
        }

        [TestMethod]
        public void GivenEmptyQuery_ExpectPositiveMatch()
        {
            foreach (var matcher in normalizedMatchers)
            {
                var match = matcher.FindNearest(string.Empty);
                Assert.IsNotNull(match);
            }
        }

        [TestMethod]
        public void GivenEmptyQuery_ExpectEmptyMatch()
        {
            string[] emptyTargets = new string[] { string.Empty };

            GivenEmptyQuery_ExpectEmptyMatch(new StringFuzzyMatcher<string>(emptyTargets));
            GivenEmptyQuery_ExpectEmptyMatch(new EnPhoneticFuzzyMatcher<string>(emptyTargets));
            GivenEmptyQuery_ExpectEmptyMatch(new EnHybridFuzzyMatcher<string>(emptyTargets, PhoneticWeightPercentage));
        }

        [TestMethod]
        public void GivenRegularMatcher_ExpectGreaterDistanceThanNormalizedMatcher()
        {
            IFuzzyMatcher<string, string> regularStringMatcher = new FuzzyMatcher<string, string>(StringTargets, new StringDistance());
            GivenNormalizedMatcher_ExpectLessDistanceThanRegularMatcher(stringMatcher, regularStringMatcher, queryToString);

            IFuzzyMatcher<string, EnPronunciation> regularPhoneticMatcher = new FuzzyMatcher<string, EnPronunciation>(StringTargets, new EnPhoneticDistance(), queryToPronunciation);
            GivenNormalizedMatcher_ExpectLessDistanceThanRegularMatcher(phoneticMatcher, regularPhoneticMatcher, queryToPronunciation);

            IFuzzyMatcher<string, DistanceInput> regularHybridMatcher = new FuzzyMatcher<string, DistanceInput>(StringTargets, new EnHybridDistance(PhoneticWeightPercentage), queryToDistanceInput);
            GivenNormalizedMatcher_ExpectLessDistanceThanRegularMatcher(hybridMatcher, regularHybridMatcher, queryToDistanceInput);
        }

        [TestMethod]
        public void GivenThreshold_ExpectPositiveMatch()
        {
            const double Threshold = 0.5;
            foreach (var matcher in normalizedMatchers)
            {
                var matches = matcher.FindNearestWithin(One, Threshold, StringTargets.Length);
                Assert.IsTrue(matches.Count > 0);
                foreach (var match in matches)
                {
                    Assert.IsTrue(match.Distance <= Threshold);
                }
            }
        }

        [TestMethod]
        public void GivenMissingTransformation_ExpectException()
        {
            Assert.ThrowsException<InvalidCastException>(() =>
            {
                var matcher = new StringFuzzyMatcher<MyTargetType>(Targets);
            });

            Assert.ThrowsException<InvalidCastException>(() =>
            {
                var matcher = new EnPhoneticFuzzyMatcher<MyTargetType>(Targets);
            });

            Assert.ThrowsException<InvalidCastException>(() =>
            {
                var matcher = new EnHybridFuzzyMatcher<MyTargetType>(Targets, PhoneticWeightPercentage);
            });
        }

        /// <summary>
        /// Test all constructor parameters.
        /// </summary>
        [TestMethod]
        public void GivenIntegerType_ExpectPositive()
        {
            Func<MyTargetType, string> targetToPhrase = (target) => target.Phrase;
            GivenIntegerType_ExpectPositive(new StringFuzzyMatcher<MyTargetType>(Targets, targetToPhrase, false));
            GivenIntegerType_ExpectPositive(new EnPhoneticFuzzyMatcher<MyTargetType>(Targets, targetToPhrase, false));
            GivenIntegerType_ExpectPositive(new EnHybridFuzzyMatcher<MyTargetType>(Targets, PhoneticWeightPercentage, targetToPhrase, false));
        }

        /// <summary>
        /// Make sure every normalized matcher APIs are tested.
        /// </summary>
        [TestMethod]
        public void GivenCount_ExpectPositiveMatches()
        {
            const int Count = 3;
            foreach (var matcher in normalizedMatchers)
            {
                var matches = matcher.FindNearest("zoro", Count);
                Assert.AreEqual(Count, matches.Count);
            }
        }

        private static void GivenEmptyQuery_ExpectEmptyMatch(IFuzzyMatcher<string, string> matcher)
        {
            var match = matcher.FindNearest(string.Empty);
            Assert.AreEqual(string.Empty, match.Element);
            Assert.AreEqual(0, match.Distance);
        }

        private static void GivenNormalizedMatcher_ExpectLessDistanceThanRegularMatcher<Extraction>(IFuzzyMatcher<string, string> normalizedMatcher, IFuzzyMatcher<string, Extraction> regularMatcher, Func<string, Extraction> queryToExtraction)
        {
            string query = "ten";
            var normalizedMatch = normalizedMatcher.FindNearest(query);
            var regularMatch = regularMatcher.FindNearest(queryToExtraction(query));
            Assert.AreEqual(normalizedMatch.Element, regularMatch.Element);
            Assert.IsTrue(normalizedMatch.Distance < regularMatch.Distance);
        }

        private static void GivenIntegerType_ExpectPositive(IFuzzyMatcher<MyTargetType, string> matcher)
        {
            var match = matcher.FindNearest(One);
            Assert.AreEqual(0, match.Distance);
            Assert.AreEqual(1, match.Element.Id);
        }

        private class MyTargetType
        {
            public MyTargetType(string phrase, int id)
            {
                this.Phrase = phrase;
                this.Id = id;
            }

            public string Phrase { get; private set; }

            public int Id { get; private set; }
        }
    }
}
