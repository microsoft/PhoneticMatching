//-----------------------------------------------------------------------
// <copyright file="BaseDistanceTester.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace PhoneticMatchingTests.Distance
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PhoneticMatching.Distance;

    public abstract class BaseDistanceTester<T> where T : class
    {
        public BaseDistanceTester()
        {
            this.Distance = this.CreateDistanceOperator();
        }

        protected IDistance<T> Distance { get; private set; }

        [TestMethod]
        public void GivenNull_ExpectException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var dist = this.Distance.Distance(null, null);
            });
        }

        protected abstract IDistance<T> CreateDistanceOperator();
    }
}
