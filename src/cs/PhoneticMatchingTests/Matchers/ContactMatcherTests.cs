// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.PhoneticMatching.Matchers.ContactMatcher;

    [TestClass]
    public class ContactMatcherTests : BaseContactMatcherTester
    {
        [TestMethod]
        public void GivenSimilarPhoneticWeight_ExpectPositiveMatch()
        {
            var matcher = new EnContactMatcher<TestContact>(this.Targets, this.ContactFieldsExtrator);
            var results = matcher.Find("andru");

            Assert.AreEqual(2, results.Count);
            var expected = new TestContact()
            {
                FirstName = "Andrew",
                LastName = string.Empty
            };
            Assert.IsTrue(results.Contains(expected));
            expected = new TestContact()
            {
                FirstName = "Andrew",
                LastName = "Smith",
                Id = "1234567"
            };
            Assert.IsTrue(results.Contains(expected));
        }

        [TestMethod]
        public void GivenDuplicateNames_ExpectPositiveMatch()
        {
            var matcher = new EnContactMatcher<TestContact>(this.Targets, this.ContactFieldsExtrator);
            var results = matcher.Find("john");

            Assert.AreEqual(2, results.Count);
            var expected = new TestContact()
            {
                FirstName = "John",
                LastName = "B",
                Id = "7654321"
            };
            Assert.IsTrue(results.Contains(expected));
            expected = new TestContact()
            {
                FirstName = "John",
                LastName = "C",
                Id = "2222222"
            };
            Assert.IsTrue(results.Contains(expected));
        }

        [TestMethod]
        public void GivenExactMatch_ExpectPositiveMatch()
        {
            var matcher = new EnContactMatcher<TestContact>(this.Targets, this.ContactFieldsExtrator);
            var results = matcher.Find("Andrew Smith");

            Assert.AreEqual(1, results.Count);
            var expected = new TestContact()
            {
                FirstName = "Andrew",
                LastName = "Smith",
                Id = "1234567"
            };
            Assert.AreEqual(expected, results[0]);
        }

        [TestMethod]
        public void GivenEmptyQuery_ExpectEmptyResult()
        {
            var matcher = new EnContactMatcher<TestContact>(this.Targets, this.ContactFieldsExtrator);
            var results = matcher.Find(string.Empty);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GivenNullQuery_ExpectException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var matcher = new EnContactMatcher<TestContact>(this.Targets, this.ContactFieldsExtrator);
                matcher.Find(null);
            });
        }

        private ContactFields ContactFieldsExtrator(TestContact contact)
        {
            return new ContactFields()
            {
                Name = contact.FullName
            };
        }
    }
}