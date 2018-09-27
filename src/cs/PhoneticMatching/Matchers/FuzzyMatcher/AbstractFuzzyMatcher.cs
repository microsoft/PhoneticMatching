// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace PhoneticMatching.Matchers.FuzzyMatcher
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using PhoneticMatching.Distance;

    /// <summary>
    /// A fuzzy matcher. The fuzziness it determined by the provided distance function.
    /// </summary>
    /// <typeparam name="Target">The type of the returned matched object.</typeparam>
    /// <typeparam name="Extraction">The type of the query object.</typeparam>
    /// <typeparam name="Pronounceable">A type understood by the distance function. Note that Extraction == Pronounceable for the usual case.</typeparam>
    public abstract class AbstractFuzzyMatcher<Target, Extraction, Pronounceable> : FuzzyMatcherBase, IFuzzyMatcher<Target, Extraction>
    {
        private IList<Target> targets;
        private IList<Extraction> extractions;
        private IList<Pronounceable> pronounceables;
        private DistanceFunc distance;
        private Func<Target, Extraction> targetToExtraction;
        private Func<Extraction, Pronounceable> extractionToPronounceable;
        private Pronounceable currentQuery;
        private bool isAccelerated;

        /// <summary>
        /// We need to keep a local reference on the DistanceDelegate object to prevent the implicit copy from being garbage collected.
        /// </summary>
        private DistanceDelegate nativeDistanceDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFuzzyMatcher{Target,Extraction,Pronounceable}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance operator.</param>
        /// <param name="extractionToPronounceable">A mapping of the extraction type to a type understood by the distance function. Note that this is only required if Extraction != Pronounceable</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == Pronounceable for the usual case.</param>
        internal AbstractFuzzyMatcher(IList<Target> targets, IDistance<Pronounceable> distance, Func<Extraction, Pronounceable> extractionToPronounceable, Func<Target, Extraction> targetToExtraction = null)
            : this(targets, distance.Distance, extractionToPronounceable, targetToExtraction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFuzzyMatcher{Target,Extraction,Pronounceable}"/> class.
        /// </summary>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance delegate.</param>
        /// <param name="extractionToPronounceable">A mapping of the extraction type to a type understood by the distance function. Note that this is only required if Extraction != Pronounceable</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == Pronounceable for the usual case.</param>
        internal AbstractFuzzyMatcher(IList<Target> targets, DistanceFunc distance, Func<Extraction, Pronounceable> extractionToPronounceable, Func<Target, Extraction> targetToExtraction = null)
            : this(false, targets, distance, extractionToPronounceable, targetToExtraction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFuzzyMatcher{Target,Extraction,Pronounceable}"/> class.
        /// </summary>
        /// <param name="isAccelerated">Whether the fuzzy matcher is accelerated or linear.</param>
        /// <param name="targets">The set of objects that will be matched against. The order of equal targets is not guaranteed to be preserved.</param>
        /// <param name="distance">The distance delegate.</param>
        /// <param name="extractionToPronounceable">A mapping of the extraction type to a type understood by the distance function. Note that this is only required if Extraction != Pronounceable</param>
        /// <param name="targetToExtraction">A mapping of the input types to the query(extraction) type. Note that Extraction == Pronounceable for the usual case.</param>
        internal AbstractFuzzyMatcher(bool isAccelerated, IList<Target> targets, DistanceFunc distance, Func<Extraction, Pronounceable> extractionToPronounceable, Func<Target, Extraction> targetToExtraction = null)
            : base(isAccelerated, targets, distance, extractionToPronounceable, targetToExtraction)
        {
        }

        /// <summary>
        /// Distance function.
        /// </summary>
        /// <param name="first">first element</param>
        /// <param name="second">second element</param>
        /// <returns>Distance between first and second.</returns>
        public delegate double DistanceFunc(Pronounceable first, Pronounceable second);

        /// <summary>
        /// Gets the size of the matcher. The number of targets constructed with.
        /// </summary>
        public int Count
        {
            get
            {
                return this.targets.Count;
            }
        }

        /// <summary>
        /// Find the nearest element.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <returns>The closest match to target, or null if the initial targets list was empty.</returns>
        public Match<Target> FindNearest(Extraction target)
        {
            var matches = this.FindNearestWithin(target, double.MaxValue, 1);
            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return matches[0];
            }
        }

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target.</returns>
        public IList<Match<Target>> FindNearest(Extraction target, int count)
        {
            return this.FindNearestWithin(target, double.MaxValue, count);
        }

        /// <summary>
        /// Find the nearest element.
        /// </summary>
        /// <param name="target">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <returns>The closest match to target within limit, or null if no match is found.</returns>
        public Match<Target> FindNearestWithin(Extraction target, double limit)
        {
            var matches = this.FindNearestWithin(target, limit, 1);
            if (matches.Count == 0)
            {
                return null;
            }
            else
            {
                return matches[0];
            }
        }

        /// <summary>
        /// Find the __k__ nearest elements.
        /// </summary>
        /// <param name="query">The search target.</param>
        /// <param name="limit">The maximum distance to a match.</param>
        /// <param name="count">The maximum number of result to return.</param>
        /// <returns>The __k__ nearest matches to target within limit</returns>
        public IList<Match<Target>> FindNearestWithin(Extraction query, double limit, int count)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query can't be null");
            }

            this.SetCurrentQuery(query);

            // the number of targets is the maximum count
            count = Math.Max(Math.Min(count, this.Count), 1);

            int[] nearestIdxs = new int[count];
            for (int idx = 0; idx < count; ++idx)
            {
                nearestIdxs[idx] = -1;
            }

            double[] distances = new double[count];
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = this.NativeFindNearestWithin(this.Native, count, limit, nearestIdxs, distances, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });

            IList<Match<Target>> matches = new List<Match<Target>>();
            for (int idx = 0; idx < count; ++idx)
            {
                if (nearestIdxs[idx] == -1)
                {
                    // no more matches
                    break;
                }

                var match = new Match<Target>(
                    this.targets[nearestIdxs[idx]],
                    distances[idx]);

                matches.Add(match);
            }

            this.currentQuery = default(Pronounceable);
            return matches;
        }

        /// <summary>
        /// Makes the native call to FindNearestWithin method virtual so we can use normal or accelerated version.
        /// </summary>
        /// <param name="native">Pointer to the native FuzzyMatcher object</param>
        /// <param name="count">maximum number of elements to retrieve</param>
        /// <param name="limit">threshold under which we have a match using the fuzzy matcher</param>
        /// <param name="nearestIdxs">array in which result elements are stored</param>
        /// <param name="distances">array in which result distances are stored</param>
        /// <param name="buffer">buffer for error message</param>
        /// <param name="bufferSize">size of the buffer</param>
        /// <returns>The result of the native operation.</returns>
        protected virtual NativeResult NativeFindNearestWithin(IntPtr native, int count, double limit, int[] nearestIdxs, double[] distances, StringBuilder buffer, ref int bufferSize)
        {
            if (this.isAccelerated)
            {
                return FuzzyMatcherBase.AcceleratedFuzzyMatcher_FindNearestWithin(native, count, limit, nearestIdxs, distances, buffer, ref bufferSize);
            }
            else
            {
                return FuzzyMatcherBase.FuzzyMatcher_FindNearestWithin(native, count, limit, nearestIdxs, distances, buffer, ref bufferSize);
            }
        }

        /// <summary>
        /// Instantiate the native resource wrapped
        /// </summary>
        /// <param name="args">The parameter is not used.</param>
        /// <returns>A pointer to the native resource.</returns>
        protected override IntPtr CreateNativeResources(params object[] args)
        {
            if (args.Length != 5)
            {
                throw new ArgumentException("Fuzzy matcher needs parameters to instantiate native resource.");
            }

            this.isAccelerated = (bool)args[0];
            var targets = args[1] as IList<Target>;
            var distance = args[2] as DistanceFunc;
            var extractionToPronounceable = args[3] as Func<Extraction, Pronounceable>;
            var targetToExtraction = args[4] as Func<Target, Extraction>;

            // Managed object needs to be initialized *before* creating the native fuzzy matcher.
            this.InitializeManaged(targets, distance, targetToExtraction, extractionToPronounceable);

            int targetsCount = targets.Count;

            IntPtr native = IntPtr.Zero;

            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = FuzzyMatcherBase.FuzzyMatcher_Create(targetsCount, this.nativeDistanceDelegate, this.isAccelerated, out native, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });

            return native;
        }

        private void SetCurrentQuery(Extraction query)
        {
            if (this.extractionToPronounceable != null)
            {
                this.currentQuery = this.extractionToPronounceable(query);
            }
            else
            {
                try
                {
                    this.currentQuery = (Pronounceable)Convert.ChangeType(query, typeof(Pronounceable));
                }
                catch
                {
                    throw new InvalidOperationException(string.Format("Conversion delegate is required to retrieve extraction from Target type [{0}] to Extraction type [{1}].", typeof(Extraction), typeof(Pronounceable)));
                }
            }
        }

        private void InitializeManaged(IList<Target> targets, DistanceFunc distance, Func<Target, Extraction> targetToExtraction, Func<Extraction, Pronounceable> extractionToPronounceable)
        {
            this.targets = targets;
            this.distance = distance;
            this.targetToExtraction = targetToExtraction;
            this.extractionToPronounceable = extractionToPronounceable;
            this.nativeDistanceDelegate = this.Distance;

            if (targetToExtraction == null)
            {
                try
                {
                    this.extractions = (IList<Extraction>)targets;
                }
                catch
                {
                    throw new InvalidCastException(string.Format("Can't cast Target type [{0}] to Extraction type [{1}]. You must provide a conversion function 'targetToExtraction'.", typeof(Target), typeof(Extraction)));
                }
            }
            else
            {
                this.extractions = new Extraction[targets.Count];
            }

            if (extractionToPronounceable == null)
            {
                this.pronounceables = (IList<Pronounceable>)this.extractions;
            }
            else
            {
                this.pronounceables = new Pronounceable[targets.Count];
            }
        }

        private double Distance(int firstIdx, int secondIdx)
        {
            try
            {
                var first = this.GetPronounceableAt(firstIdx);
                var second = this.GetPronounceableAt(secondIdx);

                return this.distance(first, second);
            }
            catch (Exception ex)
            {
                ManagedCallback.LastError = ex;

                // this should throw back the exception to native code but this is not supported on Unix. Hence, we rely on <see cref="ManagedCallback.LastError" />
                return 0;
            }
        }

        private Pronounceable GetPronounceableAt(int idx)
        {
            Pronounceable pronounceable;
            if (idx == -1)
            {
                pronounceable = this.currentQuery;
            }
            else
            {
                if (idx >= this.targets.Count)
                {
                    throw new ArgumentOutOfRangeException(string.Format("index is out of bound : idx={0} - targets.Count={1}", idx, this.targets.Count));
                }

                // lazy initialize this.pronounceables[idx]
                if (this.pronounceables[idx] == null)
                {
                    // lazy initialize this.extractions[idx]
                    if (this.extractions[idx] == null)
                    {
                        if (this.targetToExtraction == null)
                        {
                            throw new InvalidOperationException(string.Format("Conversion delegate is required to retrieve extraction from Target type [{0}] to Extraction type [{1}].", typeof(Target), typeof(Extraction)));
                        }

                        this.extractions[idx] = this.targetToExtraction(this.targets[idx]);
                    }

                    // check if extraction and pronounceable points to the same object
                    if (this.pronounceables[idx] == null)
                    {
                        if (this.extractionToPronounceable == null)
                        {
                            throw new InvalidOperationException(string.Format("Conversion delegate is required to retrieve extraction from Target type [{0}] to Extraction type [{1}].", typeof(Extraction), typeof(Pronounceable)));
                        }

                        this.pronounceables[idx] = this.extractionToPronounceable(this.extractions[idx]);
                    }
                }

                pronounceable = this.pronounceables[idx];
            }

            return pronounceable;
        }
    }
}
