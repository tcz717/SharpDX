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

namespace SharpDX.Direct3D11
{
    public partial class DeviceContext
    {
        public partial class InputAssemblerStage
        {
            /// <summary>
            ///   Binds a single vertex buffer to the input assembler.
            /// </summary>
            /// <param name = "slot">Index of the slot to which to bind the vertex buffer.</param>
            /// <param name = "vertexBufferBinding">A binding for the input vertex buffer.</param>
            public void SetVertexBuffers(int slot, VertexBufferBinding vertexBufferBinding)
            {
                SetVertexBuffers(slot, new[] {vertexBufferBinding});
            }

            /// <summary>
            ///   Binds an array of vertex buffers to the input assembler.
            /// </summary>
            /// <param name = "firstSlot">Index of the first input slot to use for binding. The first vertex buffer is explicitly bound to the start slot; this causes each additional vertex buffer in the array to be implicitly bound to each subsequent input slot. There are 16 input slots.</param>
            /// <param name = "vertexBufferBindings">An array of bindings for input vertex buffers.</param>
            public void SetVertexBuffers(int firstSlot, params VertexBufferBinding[] vertexBufferBindings)
            {
                Buffer[] vertexBuffers = new Buffer[vertexBufferBindings.Length];
                int[] strides = new int[vertexBufferBindings.Length];
                int[] offsets = new int[vertexBufferBindings.Length];
                for (int i = 0; i < vertexBufferBindings.Length; i++)
                {
                    vertexBuffers[i] = vertexBufferBindings[i].Buffer;
                    strides[i] = vertexBufferBindings[i].Stride;
                    offsets[i] = vertexBufferBindings[i].Offset;
                }
                SetVertexBuffers(firstSlot, vertexBuffers, strides, offsets);
            }

            /// <summary>
            /// Binds an array of vertex buffers to the input assembler.
            /// </summary>
            /// <param name="slot">Index of the first input slot to use for binding. The first vertex buffer is explicitly bound to the start slot; this causes each additional vertex buffer in the array to be implicitly bound to each subsequent input slot. There are 16 input slots.</param>
            /// <param name="vertexBuffers">The vertex buffers.</param>
            /// <param name="stridesRef">The strides.</param>
            /// <param name="offsetsRef">The offsets.</param>
            public void SetVertexBuffers(int slot, SharpDX.Direct3D11.Buffer[] vertexBuffers, int[] stridesRef, int[] offsetsRef)
            {
                unsafe
                {
                    IntPtr* vertexBuffersOut_ = (IntPtr*)0;
                    if (vertexBuffers != null)
                    {
                        IntPtr* vertexBuffersOut__ = stackalloc IntPtr[vertexBuffers.Length];
                        vertexBuffersOut_ = vertexBuffersOut__;
                        for (int i = 0; i < vertexBuffers.Length; i++)
                            vertexBuffersOut_[i] = (vertexBuffers[i] == null) ? IntPtr.Zero : vertexBuffers[i].NativePointer;
                    }
                    fixed (void* stridesRef_ = stridesRef)
                        fixed (void* offsetsRef_ = offsetsRef)
                            SharpDX.Direct3D11.LocalInterop.Callivoid(_nativePointer, slot, vertexBuffers != null?vertexBuffers.Length:0, vertexBuffersOut_, stridesRef_, offsetsRef_, ((void**)(*(void**)_nativePointer))[18]);
                }
            }
        }
    }
}