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


using XiCore.DataStructures;

namespace XiSound
{
    public partial class SoundSystem
    {
        private static int SOUND_SOURCES_COUNT = 64;
        
        private static readonly DLinkedList<SoundSource> FreeList = new DLinkedList<SoundSource>();

        private static readonly SoundSource[] AllSources = new SoundSource[SOUND_SOURCES_COUNT];
            
        private static void InitPool()
        {
            for (var i = 0; i < SOUND_SOURCES_COUNT; i++)
            {
                var source = new SoundSource();
                source.Handle = new SoundHandle((ushort)(i+1),0);
                AllSources[i] = source;
                FreeList.AddFirst(source.Link);
            }
        }
        
        private static void DeinitPool()
        {
            for (var i = 0; i < SOUND_SOURCES_COUNT; i++)
            {
                AllSources[i].Destroy();
                AllSources[i] = null;
            }
        }

        public static SoundSource GetSource(SoundHandle handle)
        {
            if (handle.Identifier == 0) return null;
            var source = AllSources[handle.Identifier - 1];
            if (source.Handle == handle)
                return source;
            return null;
        }
        
        public static bool IsExist(SoundHandle handle)
        {
            if (handle.Identifier == 0) return false;
            var source = AllSources[handle.Identifier - 1];
            if (source.Handle == handle)
                return true;
            return false;
        }
        
        public static SoundSource CreateSoundObject()
        {
            if (FreeList.Count > 0)
            {
                var soundSource = FreeList.First.Value;
                soundSource.Link.Remove();
                return soundSource;
            }
            return null;
        }
    
        public  static void ReleaseSoundObject(SoundSource soundSourceSource)
        {
            soundSourceSource.Handle.UID++; // make this handle new
            FreeList.AddFirst(soundSourceSource.Link);
        }
    
        public static int TotalSoundObjesInMemory => SOUND_SOURCES_COUNT;
    }
}