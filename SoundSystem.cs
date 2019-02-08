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

using VARP.Subsystems;
using VARP.DataStructures;
using VARP.Sounds.Events;
using UnityEngine;

namespace VARP.Sounds
{
    public partial class SoundSystem : BaseSystem
    {
        // =================================================================================================================
        // Constructor
        // =================================================================================================================
    
        /// <summary>SoundSystem constructor</summary>
        public SoundSystem(BaseSystem parent, object gameData)
        {
            parent.AddChild(this);
        }
    
        // =================================================================================================================
        // Singleton
        // =================================================================================================================

        /// <summary>Initialize system</summary>
        public static void InitA()
        {
            InitPool();
        }

        /// <summary>De initialize system</summary>
        public static void DeInit()
        {
            DeinitPool();
        }
        
        // =================================================================================================================
        // MonoBehaviour
        // =================================================================================================================

        /// <summary>Update system</summary>
        public static void OnUpdate()
        {
            var deltaTime = Time.deltaTime;
            var curent = soundSourcesList.First;
            while (curent != null)
            {
                var next = curent.Next;
                curent.Value.OnUpdate(deltaTime);
                curent = next;
            }
        }

        // =================================================================================================================
        // SOUND SOURCES
        // =================================================================================================================

        private const int LIMIT_SOURCES = 64;
        
        /// <summary></summary>
        private static readonly DLinkedList<SoundSource> soundSourcesList = new DLinkedList<SoundSource>();
        /// <summary></summary>
        public static void Add(DLinkedListNode<SoundSource> link) { soundSourcesList.AddLast(link); }
        /// <summary></summary>
        public static void Remove(DLinkedListNode<SoundSource> link) { soundSourcesList.Remove(link); }
        /// <summary></summary>
        public static int SndSourcesCount => soundSourcesList.Count;
        /// <summary></summary>
        public static bool IsReachLimit => soundSourcesList.Count >= LIMIT_SOURCES;

        /// <summary></summary>
        public static SoundHandle Play(AudioEvent audioEvent, SoundSource.OnEndDelegate onChangeState = null)
        {
            var soundSource = CreateSoundObject();
            if (soundSource != null)
            {
                soundSource.Play(audioEvent, null, onChangeState);
      
                return soundSource.Handle;
            }
            else
            {
                string message = $"Can't create sound source for event {audioEvent.name}";
                if (Verbose)
                    Debug.Log("[SoundManager] " + message);
                PrintSoundManager();
                return SoundHandle.NullHandle;
            }
        }

        /// <summary></summary>
        public static SoundHandle Play(AudioEvent audioEvent, string clipName, Vector3 position, SoundSource.OnEndDelegate onChangeState = null)
        {
            var soundSource = CreateSoundObject();
            if (soundSource != null)
            {
                soundSource.Play(audioEvent, clipName, onChangeState);
                return soundSource.Handle;
            }
            else
            {
                string message = $"Can't create sound source for event {audioEvent.name}";
                if (Verbose)
                    Debug.Log("[SoundManager] " + message);
                PrintSoundManager();
                return SoundHandle.NullHandle;
            }
        }
        
        /// <summary></summary>
        public static void StopAllSounds()
        {
            var curent = soundSourcesList.First;
            while (curent != null)
            {
                var next = curent.Next;
                var sound = curent.Value;
                sound.Stop();
                curent = next;
            }
        }
        
        /// <summary></summary>
        public static void StopAllSound(string eventName)
        {
            var curent = soundSourcesList.First;
            while (curent != null)
            {
                var next = curent.Next;
                var sound = curent.Value;
                if (sound.EventName == eventName) 
                    sound.Stop();
                curent = next;
            }

        }
        
        /// <summary></summary>
        public static void FadeOutAllSounds()
        {
            var curent = soundSourcesList.First;
            while (curent != null)
            {
                var next = curent.Next;
                var sound = curent.Value;
                sound.FadeOut();
                curent = next;
            }
        }
        
        /// <summary></summary>
        public static void FadeOutAllSounds(string eventName)
        {
            var curent = soundSourcesList.First;
            while (curent != null)
            {
                var next = curent.Next;
                var sound = curent.Value;
                if (sound.EventName == eventName)
                    sound.FadeOut();
                curent = next;
            }
        }

        public static bool Verbose;
    }
}
