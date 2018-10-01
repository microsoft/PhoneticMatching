// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.PhoneticMatching
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Native resource wrapper. Makes sure native resources are dispose at the end of the wrapper life cycle.
    /// </summary>
    public abstract class NativeResourceWrapper
    {
        /// <summary>
        /// Initial buffer size to retrieve strings from native code.
        /// </summary>
        private const int InitialBufferSize = 128;

        /// <summary>
        /// Maximum buffer size if native code requires bigger buffer. This should be configurable.
        /// </summary>
        private const int MaximumBufferSize = 4096;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeResourceWrapper"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="args">Arguments required to create native resources.</param>
        public NativeResourceWrapper(params object[] args)
        {
            this.Native = this.CreateNativeResources(args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeResourceWrapper"/> class.
        /// Copy constructor.
        /// </summary>
        /// <param name="ptr">Create a wrapper around the native resource specified. Resource will be deleted on dispose.</param>
        public NativeResourceWrapper(IntPtr ptr)
        {
            this.Native = ptr;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="NativeResourceWrapper"/> class.
        /// NOTE: Leave out the finalizer altogether if this class doesn't 
        /// own unmanaged resources itself, but leave the other methods
        /// exactly as they are. 
        /// </summary>
        ~NativeResourceWrapper()
        {
            // free native resources here if there are any
            this.DisposeNativeResource();
        }

        /// <summary>
        /// Possible results returned by the native bindings.
        /// </summary>
        protected enum NativeResult
        {
            Success = 0,
            InvalidParameter = 1,
            InternalError = 2,
            BufferTooSmall = 3,
        }

        /// <summary>
        /// Gets the pointer to the native representation of this object.
        /// </summary>
        protected internal IntPtr Native { get; private set; }

        /// <summary>
        /// Gets or sets the current size of the buffer used to retrieved strings from native code.
        /// </summary>
        protected static int BufferSize { get; set; } = InitialBufferSize;

        /// <summary>
        /// Call a native function passed as parameter using a temporary buffer. Process the result for any error and return the buffer.
        /// </summary>
        /// <param name="nativeFunc">A native function using a buffer for error message.</param>
        /// <returns>The buffer, containing the last error message or any information returned by the native library.</returns>
        protected static StringBuilder CallNative(Func<StringBuilder, NativeResult> nativeFunc)
        {
            ManagedCallback.LastError = null;
            StringBuilder buffer = new StringBuilder(NativeResourceWrapper.BufferSize);
            var result = nativeFunc(buffer);

            if (result == NativeResult.BufferTooSmall)
            {
                if (NativeResourceWrapper.BufferSize > MaximumBufferSize)
                {
                    throw new InsufficientMemoryException(string.Format("Native library requires a buffer of {0} characters. Maximum capacity is {1}.", NativeResourceWrapper.BufferSize, MaximumBufferSize));
                }

                buffer.Capacity = NativeResourceWrapper.BufferSize;
                result = nativeFunc(buffer);
            }

            if (ManagedCallback.LastError != null)
            {
                throw ManagedCallback.LastError;
            }

            if (result != NativeResult.Success)
            {
                if (result == NativeResult.InvalidParameter)
                {
                    throw new ArgumentException(buffer.ToString());
                }

                throw new Exception(string.Format("Unhandled exception in native code: {0}", buffer));
            }

            return buffer;
        }

        /// <summary>
        /// Delete the native pointer using the type specified in native bindings.
        /// </summary>
        /// <param name="native">Pointer to the native object.</param>
        /// <param name="buffer">Buffer for any error message</param>
        /// <param name="bufferSize">Size of the buffer, to be adjusted if error doesn't fit the current size.</param>
        /// <returns>The result code from native library.</returns>
        protected abstract NativeResult NativeDelete(IntPtr native, StringBuilder buffer, ref int bufferSize);
                
        /// <summary>
        /// Instantiate the native resource wrapped
        /// </summary>
        /// <param name="args">Arguments needed to instantiate the resource if any.</param>
        /// <returns>A pointer to the native resource.</returns>
        protected abstract IntPtr CreateNativeResources(params object[] args);
        
        private void DisposeNativeResource()
        {
            NativeResourceWrapper.CallNative((buffer) =>
            {
                int bufferSize = NativeResourceWrapper.BufferSize;
                var result = this.NativeDelete(this.Native, buffer, ref bufferSize);
                NativeResourceWrapper.BufferSize = bufferSize;
                return result;
            });
            this.Native = IntPtr.Zero;
        }
    }
}
