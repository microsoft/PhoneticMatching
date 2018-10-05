// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.PhoneticMatching.Matchers.PlaceMatcher;

    [TestClass]
    public class PlaceMatcherTests
    {
        private static readonly TestPlace[] Targets = new TestPlace[]
        {
            new TestPlace()
            {
                Name = "Marbles Restaurant",
                Address = "8 William Street E",
                Categories = new string[] { "Canadian (New)" },
                Id = "1234567"
            },
            new TestPlace()
            {
                Name = "Beertown",
                Address = "75 King Street S",
                Categories = new string[] { "Canadian (New)", "Beer, Wine & Spirits", "Bars" },
                Id = "7654321"
            },
            new TestPlace()
            {
                Name = "Nick and Nat's Uptown 21",
                Address = "21 King St N",
                Categories = new string[] { "Canadian (New)" }
            },
            new TestPlace()
            {
                Name = "The Shops",
                Address = "7 Fake Cres. Toronto"
            },
        };

        private EnPlaceMatcher<TestPlace> matcher = new EnPlaceMatcher<TestPlace>(Targets, ExtractPlaceFields);
        
        [TestMethod]
        public void GivenAddress_ExpectTwoPositiveMatches()
        {
            var result = this.matcher.Find("king street");

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(new TestPlace() { Name = "Beertown", Address = "75 King Street S", Categories = new string[] { "Canadian (New)", "Beer, Wine & Spirits", "Bars" }, Id = "7654321" }));
            Assert.IsTrue(result.Contains(new TestPlace() { Name = "Nick and Nat's Uptown 21", Address = "21 King St N", Categories = new string[] { "Canadian (New)" } }));
        }

        [TestMethod]
        public void GivenAddressExpansion_ExpectPositiveMatch()
        {
            var result = this.matcher.Find("fake crescent");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new TestPlace() { Name = "The Shops", Address = "7 Fake Cres. Toronto" }, result[0]);
        }

        [TestMethod]
        public void GivenType_ExpectPositiveMatch()
        {
            var result = this.matcher.Find("Bars");

            Assert.AreEqual(1, result.Count);

            var expected = new TestPlace()
            {
                Name = "Beertown",
                Address = "75 King Street S",
                Categories = new string[] { "Canadian (New)", "Beer, Wine & Spirits", "Bars" },
                Id = "7654321"
            };
            Assert.AreEqual(expected, result[0]);
        }

        [TestMethod]
        public void GivenExactName_ExpectPositiveMatch()
        {
            var result = this.matcher.Find("The Shops");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new TestPlace() { Name = "The Shops", Address = "7 Fake Cres. Toronto" }, result[0]);
        }

        [TestMethod]
        public void GivenEmptyString_ExpectNegative()
        {
            var result = this.matcher.Find(string.Empty);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GivenUnrelatedString_ExpectNegative()
        {
            var result = this.matcher.Find("Unrelated");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GivenNull_ExpectException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var result = this.matcher.Find(null);
            });
        }

        private static PlaceFields ExtractPlaceFields(TestPlace place)
        {
            return new PlaceFields()
            {
                Name = place.Name,
                Address = place.Address,
                Types = place.Categories
            };
        }

        private class TestPlace
        {
            public string Name { get; set; }

            public string Address { get; set; }

            public string Id { get; set; }

            public string[] Categories { get; set; }

            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }

                if (obj != null)
                {
                    if (obj.GetType() == this.GetType())
                    {
                        var other = (TestPlace)obj;
                        bool isSimilar = other.Name == this.Name &&
                            other.Address == this.Address &&
                            other.Id == this.Id;
                        if (isSimilar)
                        {
                            return AreCategoriesEqual(other.Categories, this.Categories);
                        }
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return (this.Name + this.Address + this.Id + string.Concat(this.Categories)).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("{0} {1}", this.Name, this.Address);
            }

            private static bool AreCategoriesEqual(string[] categories1, string[] categories2)
            {
                if (categories1 == null || categories2 == null)
                {
                    // one or both is null
                    return categories1 == categories2;
                }

                if (categories1.Length == categories2.Length)
                {
                    for (int idx = 0; idx < categories1.Length; ++idx)
                    {
                        if (categories1[idx] != categories2[idx])
                        {
                            // categories are not the same or are not ordered
                            return false;
                        }
                    }

                    // categories are the same
                    return true;
                }

                // categories don't have same length
                return false;
            }
        }
    }
}
