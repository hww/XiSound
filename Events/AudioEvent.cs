using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using VARP.DataStructures;
using VARP.Fields;

namespace VARP.Sounds.Events
{
    [CreateAssetMenu(menuName = "Rocket/Audio Events/SimpleAudioEvent")]
    public class AudioEvent : BaseAudioEvent
    {
        public enum RandomMode
        {
            Single,
            Sequence,
            Random,
        }
        
        [Range(0,2)]
        public RandomFloat Volume = new RandomFloat(1,1);
        [Range(0,2)]
        public RandomFloat Pitch = new RandomFloat(1,1);
        public bool IsLooped;
        public bool IsPausable;
        public RandomFloat DelayTime = new RandomFloat(0,0);
        public RandomFloat FadeInTime = new RandomFloat(0,0);
        public RandomFloat FadeOutTime = new RandomFloat(0,0);
		
        [InfoBox("Mixer settins")]
        public AudioMixerGroup MixerGroup; 
        public AudioClip[] Clips;
        public RandomMode SequenceMode;
        public int CurentClip;

        public override bool Play(AudioSource source, bool playSound = true)
        {
            if (Clips.Length == 0) return false;
            source.clip = Clips[GetRandomIndex()];
            if (source.clip == null) return false;
            source.volume = Volume.Random;
            source.pitch = Pitch.Random;
            if (playSound)
                source.Play();
            return true;
        }
        
        public override bool Play(AudioSource source, string clipName, bool playSound = true)
        {
            if (Clips.Length == 0) return false;
            source.clip = Clips[GetNamedClipIndex(clipName)];
            if (source.clip == null) return false;
            source.volume = Volume.Random;
            source.pitch = Pitch.Random;
            if (playSound)
                source.Play();
            return true;
        }

        private int GetRandomIndex()
        {
            switch (SequenceMode)
            {
                case RandomMode.Single:
                    return Clamp(CurentClip, 0, Clips.Length);
                case RandomMode.Sequence:
                    return Clamp(CurentClip++, 0, Clips.Length);
                case RandomMode.Random:
                    return UnityEngine.Random.Range(0,Clips.Length);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private int Clamp(int value, int min, int max) { return value < min ? min : (value>max ? max : value); }
        
        private int GetNamedClipIndex(string clipName)
        {
            for (var i = 0; i < Clips.Length; i++)
            {
                var clip = Clips[i];
                if (clip != null && clip.name == clipName)
                    return i;
            }
            return -1;
        }

        // =========================================================================
        // LIST OF SOUND SOURCES
        // =========================================================================

        // Limit sound sources
        public int limitSources = 0;

        // List of available sources
        private DLinkedList<SoundSource> listOfSources = new DLinkedList<SoundSource>();

        public void Add(DLinkedListNode<SoundSource> link) { listOfSources.AddLast(link); }
        public void Remove(DLinkedListNode<SoundSource> link) { listOfSources.Remove(link); }
        public int SndSourcesCount { get { return listOfSources.Count; } }
        public bool IsReachLimit { get { return limitSources > 0 && listOfSources.Count >= limitSources; } }

        public void StopAllSounds()
        {
            var curent = listOfSources.First;
            while (curent != null)
            {
                var next = curent.Next;
                var sound = curent.Value;
                sound.Stop();
                curent = next;
            }
        }

        public void StopAllSoundsOfTransform(Transform transform)
        {
            var curent = listOfSources.First;
            while (curent != null)
            {
                var next = curent.Next;
                var sound = curent.Value;
                if (sound.transform == transform) 
                    sound.Stop();
                curent = next;
            }
        }

        
    }
}
