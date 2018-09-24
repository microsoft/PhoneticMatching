// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingPerfTests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using PhoneticMatching.Matchers;

    internal class FuzzyMatcherPerfTester<T>
    {
        private BaseMatcher<T> matcher;
        private TestElement<T>[] testSet;
        private Stopwatch testStopwatch;
        private bool isStopped = false;
        private double avgResultsCount = 0;
        private long totalTests = 0;

        public FuzzyMatcherPerfTester(BaseMatcher<T> matcher, TestElement<T>[] elements)
        {
            this.matcher = matcher;
            this.testSet = elements;
        }

        internal void Run(TimeSpan timeout, bool isAccuracyTest = false)
        {
            this.isStopped = false;

            Thread testThread;
            if (isAccuracyTest)
            {
                testThread = new Thread(this.AccuracyTestLoop);
            }
            else
            {
                testThread = new Thread(this.PerformanceTestLoop);
            }

            Console.WriteLine($"Running test loop for {timeout}...");
            testThread.Start();
            Thread.Sleep(timeout);
            Console.WriteLine("Timeout! Stopping test thread.");
            this.isStopped = true;
            var sw = new Stopwatch();
            sw.Start();
            bool isJoin = testThread.Join(1000);
            sw.Stop();
            if (isJoin)
            {
                Console.WriteLine($"Test thread stopped successfully after {sw.Elapsed}.");
            }
            else
            {
                Console.WriteLine("Test thread didn't stopped as expected. Maybe increase stopping timeout would help.");
            }

            Console.WriteLine($"Test completed!");
            this.WriteStats();
        }

        private void PerformanceTestLoop()
        {
            this.testStopwatch = new Stopwatch();
            this.testStopwatch.Start();
            while (true)
            {
                foreach (var test in this.testSet)
                {
                    foreach (var query in test.Queries)
                    {
                        foreach (var transcription in query.Transcriptions)
                        {
                            if (this.isStopped)
                            {
                                this.testStopwatch.Stop();
                                Console.WriteLine("Test thread is stopping.");
                                return;
                            }
                            else
                            {
                                int currentCounts = this.matcher.Find(transcription.Utterance).Count;
                                long incTotalTests = this.totalTests + 1;
                                this.avgResultsCount = ((this.avgResultsCount * this.totalTests) / incTotalTests) + (((double)currentCounts) / incTotalTests);
                                this.totalTests = incTotalTests;

                                if (this.totalTests % 2000 == 0)
                                {
                                    this.WriteStats();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AccuracyTestLoop()
        {
            this.testStopwatch = new Stopwatch();
            this.testStopwatch.Start();

            foreach (var test in this.testSet)
            {
                foreach (var query in test.Queries)
                {
                    foreach (var transcription in query.Transcriptions)
                    {
                        if (this.isStopped)
                        {
                            this.testStopwatch.Stop();
                            Console.WriteLine("Test thread is stopping.");
                            return;
                        }
                        else
                        {
                            var result = this.matcher.Find(transcription.Utterance);
                            
                            if (result.Any(x => x.))
                            ++this.totalTests;

                            if (this.totalTests % 2000 == 0)
                            {
                                this.WriteStats();
                            }
                        }
                    }
                }
            }
        }

        private void WriteStats()
        {
            Console.WriteLine($"{this.totalTests} tests executed in {this.testStopwatch.Elapsed} (~{(float)this.testStopwatch.ElapsedMilliseconds / this.totalTests : 0.00}ms per test) - {this.avgResultsCount:0.000} results returned in average");
        }
    }
}
