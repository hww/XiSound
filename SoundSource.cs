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

using Plugins.VARP.DataStructures;
using Plugins.VARP.Sounds.Events;
using UnityEngine;

namespace Plugins.VARP.Sounds
{
    /// <summary>
    /// This object is connectet to 
    /// </summary>
    public class SoundSource
    {
        public const string InitialName = "FreeSound";
        
        public bool IsAttachedToTransform;
        public float DelayTime;
        public float Timer;
        public string ClipName;
        public string EventName;
        
        private bool DoFadeIn;
        private bool DoFadeOut;
        private float FadeInTime;
        private float FadeOutAtTime;
        private float FadeOutTime;
        private float ClipLen;

        // Link to free list or sound manager
        public readonly DLinkedListNode<SoundSource> Link;
        // Additional link for linking with the event group
        public readonly DLinkedListNode<SoundSource> linkForEvent;
            
        // ===============================================================================
        // Delegates
        // ===============================================================================
        
        public delegate void OnEndDelegate(SoundSource soundSourceSource);
        public OnEndDelegate OnChangeState;
        
        // ===============================================================================
        // Sound state
        // ===============================================================================
  
        private ESoundSourceState state;
        public ESoundSourceState State
        {
            get { return state; }
            set { state = value; if (OnChangeState != null) OnChangeState(this); }
        }
        public bool IsNotCompleted { get { return state != ESoundSourceState.Completed; } }
        public bool CanFadeOut { get { return state != ESoundSourceState.Completed && state != ESoundSourceState.FadeOut; } }
        
        // ===============================================================================
        // Constructor & Destructor
        // ===============================================================================
        
        public SoundSource()
        {
            var obj = new GameObject(InitialName);
            //obj.hideFlags = HideFlags.HideAndDontSave;
            Source = obj.AddComponent<AudioSource>();
            Link = new DLinkedListNode<SoundSource>(this);
            linkForEvent = new DLinkedListNode<SoundSource>(this);
        }

        public void Destroy()
        {
            if (Source != null)
                GameObject.Destroy(Source.gameObject);
            Link.Remove();
            linkForEvent.Remove();
        }

        // ===============================================================================
        // Play event with given clip name
        // ===============================================================================

        public void Play(AudioEvent audioEvent, string clipName = null, OnEndDelegate onChangeState = null) 
        {
            Debug.Assert(audioEvent != null);
            
            state = ESoundSourceState.Initial;
            OnChangeState = onChangeState;

            SoundSystem.Add(Link);
            audioEvent.Add(linkForEvent);

            Source.gameObject.SetActive(true);
            Source.outputAudioMixerGroup = audioEvent.MixerGroup;
            Source.priority = 0;
            Source.mute = false;
            Source.playOnAwake = false;
            Source.ignoreListenerPause = true;
            Source.rolloffMode = AudioRolloffMode.Linear;
            Source.minDistance = 0f;
            Source.maxDistance = 100000f;
            
            if (clipName == null)
                audioEvent.Play(Source, false); // do not play, just set clip, pitch, volume
            else
                audioEvent.Play(Source, clipName, false); // do not play, just set clip, pitch, volume
            
            EventName = audioEvent.name;
            ClipName = Source.clip.name;
            Name = $"SND:{EventName}#{ClipName}";
            ClipLen = Source.clip.length;

            var clipLen05 = ClipLen * 0.5f;

            Is3D = false;
            IsLooped = audioEvent.IsLooped;
            IsPausable = audioEvent.IsPausable;
            DelayTime = audioEvent.DelayTime.Random;
            FadeInTime = Mathf.Min(audioEvent.FadeInTime.Random, clipLen05);
            FadeOutTime = Mathf.Min(audioEvent.FadeOutTime.Random, clipLen05);

            DoFadeIn = FadeInTime > 0;
            DoFadeOut = FadeOutTime > 0;
            EventVolume = audioEvent.Volume.Random;
            FadeVolume = 0;
            FadeOutAtTime = ClipLen - FadeOutTime;
            Volume = 1f;
            Play();
        }
        
        // ===============================================================================
        // AudioSource API
        // ===============================================================================

        public readonly AudioSource Source;

        public string Name
        {
            get => Source.name; 
            set => Source.name = value; 
        }

        public Transform transform
        {
            get => Source.transform;
        }
        
        private float volume;
        public float Volume
        {
            get => volume; 
            set
            {
                volume = Mathf.Clamp01(value);
                Source.volume = volume * EventVolume * fadeVolume;
            }
        }

        private float fadeVolume = 1f;
        public float FadeVolume
        {
            get => fadeVolume; 
            set
            {
                fadeVolume = value;
                Source.volume = volume * EventVolume * fadeVolume;
            }
        }

        public bool IsLooped
        {
            get => Source.loop;
            set => Source.loop = value;
        }

        public float Pitch
        {
            get => Source.pitch;
            set => Source.pitch = value;
        }

        public bool IsPausable
        {
            get => Source.ignoreListenerPause;
            set => Source.ignoreListenerPause = value; 
        }

        public bool Is3D
        {
            get => Source.spatialBlend == 1; 
            set => Source.spatialBlend = value ? 1 : 0; 
        }

        public SoundHandle Handle;

        // ===============================================================================
        // SndSource main API
        // ===============================================================================
        
        // SoundHandler not valid after this call
        public void Stop()
        {
            if (DoFadeOut)
            {
                string message = $"Stop (FadeOut): {EventName} {ClipName}";
                if (SoundSystem.Verbose)
                    Debug.Log("[SoundManager] " + message);

                if (Source.isPlaying)
                {
                    if (!IsLooped)
                        // make fadeout time minimum possible
                        FadeOutTime = Mathf.Min(FadeOutTime, Source.clip.length - Source.time);

                    Timer = 0;
                    State = ESoundSourceState.FadeOut;
                }
                else
                {
                    OnEnd();
                }
            }
            else
            {
                OnEnd();
            }
        }

        public void Play()
        {
            if (state == ESoundSourceState.Initial)
            {
                if (Source.clip == null)
                {
                    OnEnd();
                }
                else
                {
                    if (DelayTime > 0)
                    {
                        State = ESoundSourceState.Delayed;
                    }
                    else if (DoFadeIn)
                    {
                        Source.Play();
                        State = ESoundSourceState.FadeIn;
                    }
                    else
                    {
                        Source.Play();
                        State = ESoundSourceState.Playing;
                    }
                }

            }
            else
            {
                Source.Play();
            }
        }
        
        private float EventVolume;

        public void OnUpdate(float deltaTime)
        {
            switch (State)
            {
                case ESoundSourceState.Delayed:

                    Timer += deltaTime;
                    if (Timer > DelayTime)
                    {
                        if (DoFadeIn)
                        {
                            Timer = 0;
                            Source.Play();
                            State = ESoundSourceState.FadeIn;
                        }
                        else
                        {
                            Timer = 0;
                            Source.Play();
                            State = ESoundSourceState.Playing;
                        }
                    }
                    break;

                case ESoundSourceState.FadeIn:
                    Timer += deltaTime;
                    var normal1 = Mathf.Clamp01(Timer / FadeInTime);
                    FadeVolume = Mathf.Lerp(FadeVolume, 1f, normal1);
                    if (Timer > FadeInTime)
                        State = ESoundSourceState.Playing;
                    break;

                case ESoundSourceState.Playing:
                    // just update volume
                    FadeVolume = 1f;

                    if (DoFadeOut)
                    {
                        if (!Source.isPlaying || Source.time > FadeOutAtTime)
                        {
                            Timer = 0;
                            State = ESoundSourceState.FadeOut;
                        }
                    }
                    else
                    {
                        if (!Source.isPlaying)
                            OnEnd();
                    }
                    break;

                case ESoundSourceState.FadeOut:
                    Timer += deltaTime;
                    var normal2 = Mathf.Clamp01(Timer / FadeOutTime);
                    FadeVolume = Mathf.Lerp(FadeVolume, 0, normal2);
                    if (Timer > FadeOutTime)
                        OnEnd();
                    break;
            }
        }

        void OnEnd()
        {
            string message = $"OnEnd: {Name}";
            if (SoundSystem.Verbose)
                Debug.Log("[SoundManager] " + message);
            Name = InitialName;
            // ---- first mark it as completed ----
            State = ESoundSourceState.Completed;
            // ---- destroy sound object ----------
            Source.Stop();
            Source.clip = null;
            Source.transform.parent = null;
            Source.transform.position = new Vector3(0, 0, 0);
            Source.gameObject.SetActive(false);
            Link.Remove();
            linkForEvent.Remove();
            SoundSystem.ReleaseSoundObject(this);
        }

        public void FadeOut()
        {
            if (Source == null) return;
            if (CanFadeOut)
            {
                Timer = 0;
                State = ESoundSourceState.FadeOut;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0:0.00}] [{1,8}] {2}#{3}\n", Source.volume, State.ToString(), EventName, ClipName);
        }
    }
    
    public enum ESoundSourceState
    {
        Initial,
        Delayed,
        FadeIn,
        Playing,
        FadeOut,
        Completed
    }
}