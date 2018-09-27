// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching;
    using PhoneticMatching.Distance;
    using PhoneticMatching.Matchers;

    public class BaseFuzzyMatcherTester : BaseContactMatcherTester
    {
        protected StringDistance StringDistance { get; private set; } = new StringDistance();

        protected EnPhoneticDistance PhoneticDistance { get; private set; } = new EnPhoneticDistance();

        protected EnHybridDistance HybridDistance { get; private set; } = new EnHybridDistance(0.7);

        protected static void GivenSimilarString_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            GivenNearest_ExpectPositiveMatch(matcher, "andru", "Andrew");
        }

        protected static void GivenDifferentCase_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            GivenNearest_ExpectPositiveMatch(matcher, "john B", "John B");
        }

        protected static void GivenEmptyTarget_ExpectNull<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            var nearest = matcher.FindNearestWithin(string.Empty, 0.35);
            Assert.IsNull(nearest);
        }

        protected static void GivenStringDistanceAndExtractor_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<TestContact, string>
        {
            var nearest = matcher.FindNearest("john B");
            var expected = new TestContact()
            {
                FirstName = "John",
                LastName = "B",
                Id = "7654321"
            };
            Assert.AreEqual(expected, nearest.Element);
        }

        protected static void GivenEnPronunciation_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, EnPronunciation>
        {
            var match = matcher.FindNearest(PhraseToPronunciation("john bee"));
            Assert.AreEqual("John B", match.Element);
        }

        protected static void GivenDistanceInput_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, DistanceInput>
        {
            var match = matcher.FindNearest(PhraseToDistanceInput("john bee"));
            Assert.AreEqual("John B", match.Element);
        }

        protected static void GivenMultipleNearest_ExpectMultiplePositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            var matches = matcher.FindNearest("john", 2);

            Assert.AreEqual(2, matches.Count, "Number of matches is not what it should be according to test targets.");
            Assert.IsTrue(matches.Any((match) => string.Equals("John B", match.Element)), "John B wasn't matched as expected");
            Assert.IsTrue(matches.Any((match) => string.Equals("John C", match.Element)), "John C wasn't matched as expected");
        }

        protected static void GivenMultipleNearestWithinThreshold_ExpectMultiplePositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            const string Query = "john";
            var threshold = AdjustThreshold(Query, 0.8);
            var matches = matcher.FindNearestWithin(Query, threshold, 4);

            Assert.AreEqual(2, matches.Count, "Number of matches is not what it should be according to test targets.");
            Assert.IsTrue(matches.Any((match) => string.Equals("John B", match.Element) && match.Distance < threshold), "John B wasn't matched as expected");
            Assert.IsTrue(matches.Any((match) => string.Equals("John C", match.Element) && match.Distance < threshold), "John C wasn't matched as expected");
        }

        protected static void GivenZeroThresholdAndExactTarget_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            var match = matcher.FindNearestWithin("John C", 0);
            Assert.AreEqual("John C", match.Element);
        }

        protected static void GivenZeroThresholdAndWrongTarget_ExpectNull<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            var match = matcher.FindNearestWithin("John Sea", 0);
            Assert.IsNull(match);
        }

        protected static void GivenContactFuzzyMatcher_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<TestContact, TestContact>
        {
            var match = matcher.FindNearest(FullnameToTestContact("andrew smith"));
            var expected = new TestContact()
            {
                FirstName = "Andrew",
                LastName = "Smith",
                Id = "1234567"
            };

            Assert.AreEqual(expected, match.Element);
        }

        protected static void GivenNullTarget_ExpectException<T>(T matcher) where T : IFuzzyMatcher<TestContact, string>
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                matcher.FindNearest(null);
            });
        }

        protected static void GivenCustomDistanceDelegate_ExpectPositiveMatch<T>(T matcher) where T : IFuzzyMatcher<string, string>
        {
            const string Padding = "--------------------";
            const string Andrew = "Andrew";

            // Andrew is not different enough from targets
            var matchesAndrew = matcher.FindNearestWithin(Andrew, 0.1, int.MaxValue);
            Assert.AreEqual(0, matchesAndrew.Count);

            var matchesImprobable = matcher.FindNearestWithin(Padding, 1.0, int.MaxValue);
            Assert.AreEqual(matcher.Count, matchesImprobable.Count);

            var matchAndrew = matcher.FindNearest(Andrew);
            Assert.AreNotEqual(Andrew, matchAndrew);
        }

        protected static double InverseStringDistance(string a, string b)
        {
            StringDistance dist = new StringDistance();
            double realDistance = dist.Distance(a, b);
            return realDistance == 0 ? double.MaxValue : 1 / realDistance;
        }

        protected static DistanceInput PhraseToDistanceInput(string phrase)
        {
            EnPronouncer pronouncer = EnPronouncer.Instance;
            return new DistanceInput(phrase, pronouncer.Pronounce(phrase));
        }

        protected double SimpleDistance(TestContact a, TestContact b)
        {
            return this.StringDistance.Distance(a.FullName, b.FullName);
        }

        private static double AdjustThreshold(string query, double threshold)
        {
            return threshold * query.Length;
        }

        private static void GivenNearest_ExpectPositiveMatch<T>(T matcher, string given, string expected) where T : IFuzzyMatcher<string, string>
        {
            var nearest = matcher.FindNearest(given);
            Assert.AreEqual(expected, nearest.Element);
        }

        private static EnPronunciation PhraseToPronunciation(string phrase)
        {
            return PhraseToDistanceInput(phrase).Pronunciation;
        }

        /// <summary>
        /// Arbitrarily puts all but last in first name field.
        /// </summary>
        /// <param name="fullname">All given names separated by spaces.</param>
        /// <returns>A <see cref="TestContact"/> object with the given full name</returns>
        private static TestContact FullnameToTestContact(string fullname)
        {
            string[] names = fullname.Split(' ');
            string first;
            string last;
            if (names.Length == 1)
            {
                first = names[0];
                last = string.Empty;
            }
            else
            {
                first = string.Join(' ', names.Take(names.Length - 1));
                last = names.Last();
            }

            return new TestContact()
            {
                FirstName = first,
                LastName = last
            };
        }
    }
}
