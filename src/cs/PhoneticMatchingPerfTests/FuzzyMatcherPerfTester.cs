// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatchingPerfTests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.PhoneticMatching.Matchers;

    internal class FuzzyMatcherPerfTester<T>
    {
        private BaseMatcher<T> matcher;
        private TestElement<T>[] testSet;
        private double avgResultsCount = 0;
        private long totalTests = 0;

        public FuzzyMatcherPerfTester(BaseMatcher<T> matcher, TestElement<T>[] elements)
        {
            this.matcher = matcher;
            this.testSet = elements;
        }

        internal void Run(TimeSpan timeout, bool isAccuracyTest = false)
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var ct = tokenSource.Token;
                Stopwatch testStopwatch = new Stopwatch();

                Action testTaskAction;
                if (isAccuracyTest)
                {
                    testTaskAction = () => this.AccuracyTest(ct, testStopwatch, false);
                }
                else
                {
                    testTaskAction = () => this.AccuracyTest(ct, testStopwatch, true);
                }

                Console.WriteLine($"Running test loop for {timeout}...");
                var task = Task.Factory.StartNew(testTaskAction, ct);

                try
                {
                    testStopwatch.Start();
                    tokenSource.CancelAfter(timeout);
                    bool taskCompleted = task.Wait(timeout + TimeSpan.FromMilliseconds(1000));

                    if (!taskCompleted)
                    {
                        throw new Exception("Error! Test task wasn't cancelled as expected. A single test should not block for more than 1 second.");
                    }
                }
                catch (AggregateException ex)
                {
                    foreach (var inner in ex.InnerExceptions)
                    {
                        Console.WriteLine(inner.Message);
                    }
                }
                finally
                {
                    Console.WriteLine($"Test completed!");
                    this.WriteStats(testStopwatch);
                }
            }
        }

        private void AccuracyTest(CancellationToken ct, Stopwatch sw, bool isLoop)
        {
            // Were we already canceled?
            ct.ThrowIfCancellationRequested();

            long failedCount = 0;

            do
            {
                foreach (var test in this.testSet)
                {
                    foreach (var query in test.Queries)
                    {
                        foreach (var transcription in query.Transcriptions)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                Console.WriteLine("Timeout! Cancelling test task...");
                                if (!isLoop)
                                {
                                    this.WriteSuccessRate(failedCount);
                                }

                                ct.ThrowIfCancellationRequested();
                            }
                            else
                            {
                                var result = this.matcher.Find(transcription.Utterance);

                                if (!isLoop && !result.Contains(test.Element))
                                {
                                    ++failedCount;
                                }
                                
                                int currentCounts = result.Count;
                                long incTotalTests = this.totalTests + 1;
                                this.avgResultsCount = ((this.avgResultsCount * this.totalTests) / incTotalTests) + (((double)currentCounts) / incTotalTests);
                                this.totalTests = incTotalTests;

                                if (this.totalTests % 1000 == 0)
                                {
                                    this.WriteStats(sw);
                                }
                            }
                        }
                    }
                }
            }
            while (isLoop);

            this.WriteSuccessRate(failedCount);
        }

        private void WriteSuccessRate(long failedCount)
        {
            long passedCount = this.totalTests - failedCount;
            Console.WriteLine($"{passedCount}/{this.totalTests} tests passed. (accuracy={passedCount*100.0f/this.totalTests: 0.0}%)");
        }

        private void WriteStats(Stopwatch sw)
        {
            Console.WriteLine($"{this.totalTests} tests executed in {sw.Elapsed} (~{(float)sw.ElapsedMilliseconds / this.totalTests : 0.00}ms per test) - {this.avgResultsCount:0.000} results returned in average");
        }
    }
}
