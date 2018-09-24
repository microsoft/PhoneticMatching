// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingPerfTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using PhoneticMatching.Matchers.ContactMatcher;
    using PhoneticMatching.Matchers.PlaceMatcher;

    internal class Program
    {
        private const string Contact = "contact";
        private const string Place = "place";

        private static void Main(string[] args)
        {
            string type = args[0];
            int timeoutMilliseconds;
            double maxTimeout = TimeSpan.FromDays(7).TotalMilliseconds;
            string errorTimeout = $"second argument is the time during the profiling will last. It must be a valid integer between 1 and {maxTimeout} (one week)";
            if (!int.TryParse(args[1], out timeoutMilliseconds))
            {
                throw new ArgumentException(errorTimeout);
            }

            if (timeoutMilliseconds < 1 || timeoutMilliseconds > maxTimeout)
            {
                throw new ArgumentOutOfRangeException(errorTimeout + $" - current value : {timeoutMilliseconds}");
            }

            Console.WriteLine("Starting tests...");
            var sw = new Stopwatch();
            sw.Start();
            switch (type.ToLowerInvariant())
            {
                case Contact:
                    {
                        TestElement<ContactFields>[] contacts = JsonConvert.DeserializeObject<TestElement<ContactFields>[]>(File.ReadAllText(@".\contacts.json"));
                        Console.WriteLine($"Took {sw.Elapsed} to deserialize contact fields.");
                        sw.Restart();
                        var contactFields = contacts.Select(c => c.Element).ToArray();
                        var matcher = new EnContactMatcher<ContactFields>(contactFields, c => c);
                        var tester = new FuzzyMatcherPerfTester<ContactFields>(matcher, contacts);
                        Console.WriteLine($"Took {sw.Elapsed} to instantiate Contact Matcher with {contactFields.Length} contacts.");
                        tester.Run(TimeSpan.FromMilliseconds(timeoutMilliseconds));
                        break;
                    }

                case Place:
                    {
                        TestElement<PlaceFields>[] places = JsonConvert.DeserializeObject<TestElement<PlaceFields>[]>(File.ReadAllText(@".\places.json"));
                        Console.WriteLine($"Took {sw.Elapsed} to deserialize place fields.");
                        var placeFields = places.Select(c => c.Element).ToArray();
                        var matcher = new EnPlaceMatcher<PlaceFields>(placeFields, c => c);
                        var tester = new FuzzyMatcherPerfTester<PlaceFields>(matcher, places);
                        Console.WriteLine($"Took {sw.Elapsed} to instantiate Place Matcher with {placeFields.Length} places.");

                        tester.Run(TimeSpan.FromMilliseconds(timeoutMilliseconds));
                        break;
                    }

                default:
                    throw new ArgumentException($"Type must be 'place' or 'contact'. Current value: {type}");
            }
        }
    }
}
