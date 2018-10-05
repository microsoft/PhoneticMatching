// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingTests.Matchers
{
    public class BaseContactMatcherTester
    {
        protected readonly string[] TargetStrings =
        {
            "Andrew Smith",
            "Andrew",
            "John B",
            "John C",
            "Jennifer"
        };

        protected readonly TestContact[] Targets =
        {
            new TestContact()
            {
                FirstName = "Andrew",
                LastName = "Smith",
                Id = "1234567"
            },
            new TestContact()
            {
                FirstName = "Andrew",
                LastName = string.Empty,
            },
            new TestContact()
            {
                FirstName = "John",
                LastName = "B",
                Id = "7654321"
            },
            new TestContact()
            {
                FirstName = "John",
                LastName = "C",
                Id = "2222222"
            },
            new TestContact()
            {
                FirstName = "Jennifer",
                LastName = string.Empty
            }
        };

        protected class TestContact
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Id { get; set; }

            public string FullName
            {
                get
                {
                    return string.Format("{0} {1}", this.FirstName, this.LastName);
                }
            }

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
                        var other = (TestContact)obj;
                        return other.FirstName == this.FirstName &&
                            other.LastName == this.LastName &&
                            other.Id == this.Id;
                    }
                }

                return false;
            }

            public override int GetHashCode()
            {
                return (this.FirstName + this.LastName + this.Id).GetHashCode();
            }

            public override string ToString()
            {
                return this.FullName;
            }
        }
    }
}
