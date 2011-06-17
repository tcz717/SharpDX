﻿// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Runtime.InteropServices;
using SharpDX.Diagnostics;

namespace SharpDX
{
    /// <summary>
    /// Root IUnknown class to interop with COM object
    /// </summary>
    public class ComObject : CppObject, IUnknown
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComObject"/> class.
        /// </summary>
        /// <param name="pointer">Pointer to Cpp Object</param>
        public ComObject(IntPtr pointer) : base(pointer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComObject"/> class.
        /// </summary>
        protected ComObject()
        {
        }

        /// <summary>
        ///   Query Interface for a particular GUID.
        /// </summary>
        /// <param name = "guid">GUID query interface</param>
        /// <param name = "outPtr">output object associated with this GUID, IntPtr.Zero in interface is not supported</param>
        public virtual void QueryInterface(Guid guid, out IntPtr outPtr)
        {
            var result = ((IUnknown) this).QueryInterface(ref guid, out outPtr);
            result.CheckError();
        }

        ///<summary>
        /// Query Interface for a particular interface support.
        ///</summary>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public virtual T QueryInterface<T>() where T : ComObject
        {
            IntPtr parentPtr;
            this.QueryInterface(typeof (T).GUID, out parentPtr);
            return FromPointer<T>(parentPtr);
        }

        ///<summary>
        /// Query Interface for a particular interface support and attach to the given instance.
        ///</summary>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        internal void QueryInterfaceFrom<T>(T fromObject) where T : ComObject
        {
            IntPtr parentPtr;
            fromObject.QueryInterface(typeof(T).GUID, out parentPtr);
            NativePointer = parentPtr;
        }

        /// <summary>
        /// Instantiate a ComObject from a native pointer.
        /// </summary>
        /// <typeparam name="T">The ComObject class that will be returned</typeparam>
        /// <param name="comObjectPtr">The native pointer to a com object.</param>
        /// <returns>An instance of T binded to the native pointer</returns>
        public static T FromPointer<T>(IntPtr comObjectPtr) where T : ComObject
        {
            return (comObjectPtr == IntPtr.Zero) ? null : (T) Activator.CreateInstance(typeof (T), comObjectPtr);
        }

        Result IUnknown.QueryInterface(ref Guid guid, out IntPtr comObject)
        {
            return Marshal.QueryInterface(NativePointer, ref guid, out comObject);              
        }

        int IUnknown.AddReference()
        {
            if (NativePointer == IntPtr.Zero) throw new InvalidOperationException("COM Object pointer is null");
            return Marshal.AddRef(NativePointer);            
        }

        int IUnknown.Release()
        {
            if (NativePointer == IntPtr.Zero) throw new InvalidOperationException("COM Object pointer is null");
            return Marshal.Release(NativePointer);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            // Only dispose non-zero object
            if (NativePointer != IntPtr.Zero)
            {
                ((IUnknown) this).Release();
                ObjectTracker.UnTrack(this);
            }
            base.Dispose(disposing);
        }

        protected override void NativePointerUpdating()
        {
            if (Configuration.EnableObjectTracking)
                ObjectTracker.UnTrack(this);
        }

        protected override void NativePointerUpdated(IntPtr oldNativePointer)
        {
            if (Configuration.EnableObjectTracking)
                ObjectTracker.Track(this);
        }
    }
}