// =============================================================================
// MIT License
// 
// Copyright (c) 2018 Valeriya Pudova (hww.github.io)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// =============================================================================

using System;
using XiCore.DataStructures.Unions;

namespace XiSound
{
    /// <summary>
    /// This is the handle of sound source 
    /// </summary>
    public struct SoundHandle : IEquatable<SoundHandle>
    {
        public static SoundHandle NullHandle = new SoundHandle(0,0);
        
        /// <summary>
        /// Lo 16 bits keeps index of the sound
        /// Hi 16 bits keeps unique identifier
        /// </summary>
        private uint handle;

        // ==============================================================================
        // Constructors
        // ==============================================================================
        
        public SoundHandle(uint handle)
        {
            this.handle = handle;
        }
        
        public SoundHandle(ushort id, ushort uid)
        {
            handle = id | ((uint)uid << 16);
        }

        // ==============================================================================
        // Properties
        // ==============================================================================
        
        public ushort Identifier
        {
            get => (ushort)handle;
            set => handle = handle & 0xFFFF0000 | (uint)value;
        }
        
        public ushort UID
        {
            get => (ushort)(handle >> 16);
            set => handle = handle & 0x0000FFFF | ((uint)value) << 16;
        }
        
        // ==============================================================================
        // Comparisong
        // ==============================================================================
        
        public bool Equals(SoundHandle other)
        {
            return handle == other.handle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SoundHandle && Equals((SoundHandle) obj);
        }

        public override int GetHashCode()
        {
            return (int) handle;
        }
        
        // ==============================================================================
        // Casting
        // ==============================================================================

        public static implicit operator bool(SoundHandle value)
        {
            return value.handle != 0;
        }
        public static implicit operator Variant(SoundHandle value)
        {
            return (Variant)value.handle;
        }
        public static implicit operator SoundHandle(Variant value)
        {
            return new SoundHandle((uint)value.AsInteger);
        }
        public static implicit operator SoundHandle(SoundSource value)
        {
            return value.Handle;
        }
        public static implicit operator SoundSource(SoundHandle value)
        {
            return SoundSystem.GetSource(value);
        }

        // ==============================================================================
        // SoundSystem
        // ==============================================================================

        public bool IsExisting
        {
            get => SoundSystem.IsExist(this);
        }

        public SoundSource GetSource()
        {
            return SoundSystem.GetSource(this);
        }

        // ==============================================================================
        // SoundSource
        // ==============================================================================

        public void Stop()
        {
            var source = SoundSystem.GetSource(this);
            source?.Stop();
        }
        
        public void FadeOut()
        {
            var source = SoundSystem.GetSource(this);
            source?.FadeOut();
        }
        
        // ==============================================================================
        // Debugging
        // ==============================================================================
        
        public override string ToString()
        {
            return "SoundHandle[" + handle.ToString("x8") + "]";
        }
    }
}