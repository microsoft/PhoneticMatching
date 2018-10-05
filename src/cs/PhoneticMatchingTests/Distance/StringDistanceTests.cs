// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Distance
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.PhoneticMatching;
    using Microsoft.PhoneticMatching.Distance;

    [TestClass]
    public class StringDistanceTests : BaseDistanceTester<string>
    {
        [TestMethod]
        public void GivenExactString_ExpectZeroDistance()
        {
            Assert.AreEqual(0, this.Distance.Distance("This, is a test.", "This, is a test."));
        }

        [TestMethod]
        public void GivenKnownDistances_ExpectPositiveMatches()
        {
            const string Aaa = "aaa";
            const string Bbb = "bbb";
            const string Aba = "aba";

            Assert.AreEqual(3, this.Distance.Distance(Aaa, Bbb));
            Assert.AreEqual(0, this.Distance.Distance(Aaa, Aaa));
            Assert.AreEqual(1, this.Distance.Distance(Aaa, Aba));
            Assert.AreEqual(0, this.Distance.Distance(string.Empty, string.Empty));
            Assert.AreEqual(3, this.Distance.Distance(string.Empty, Aaa));
            Assert.AreEqual(3, this.Distance.Distance(Aaa, string.Empty));
        }

        protected override IDistance<string> CreateDistanceOperator()
        {
            return new StringDistance();
        }
    }
}
