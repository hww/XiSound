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

using Code.Subsystems;
using Plugins.VARP.Sounds.Events;
using UnityEngine;

namespace Plugins.VARP.Sounds
{
	public class MusicManager : BaseSystem
	{
		public static MusicManager Instance;
    
		private static SoundHandle musicHandle;
		private static bool autoPlayNext;
		private static float musicVolume;
		private static AudioEvent MusicListAudioEvent;
	
		public void SndMusicManager(BaseSystem parent, AudioEvent audioEvent)
		{
			Instance = this;
			parent.AddChild(this);
			MusicListAudioEvent = audioEvent;
		}
	
		public static SoundHandle PlayMusic()
		{
			StopMusic();
			autoPlayNext = true;
			musicHandle = SoundSystem.Play(MusicListAudioEvent, onChangeState: OnMusicChangeState);
			var soundSource = SoundSystem.GetSource(musicHandle);
			Instance.PostMessage(ESustemMessage.MusicManagerChangeMusic, soundSource.EventName, soundSource.ClipName);
			return musicHandle;
		}

		public static SoundHandle PlayMusic(string clipName)
		{
			Debug.Assert(clipName != null);
			StopMusic();
			autoPlayNext = true;
			musicHandle = SoundSystem.Play(MusicListAudioEvent, onChangeState: OnMusicChangeState);
			var soundSource = SoundSystem.GetSource(musicHandle); 
			Instance.PostMessage(ESustemMessage.MusicManagerChangeMusic, soundSource.EventName, soundSource.ClipName);
			return musicHandle;
		}

		public static void StopMusic()
		{
			autoPlayNext = false;
			var soundSource = SoundSystem.GetSource(musicHandle);
			if (soundSource != null && soundSource.IsNotCompleted)
			{
				Instance.PostMessage(ESustemMessage.MusicManagerStopMusic, soundSource.EventName, soundSource.ClipName);
				soundSource.FadeOut();
			}
		}

		private static void OnMusicChangeState(SoundSource source)
		{
			if (source.State == ESoundSourceState.FadeOut)
			{
				if (autoPlayNext)
					PlayMusic(null);
			}
		}

		public static float MusicVolume
		{
			get => musicVolume;
			set
			{
				musicVolume = value;
				var soundSource = SoundSystem.GetSource(musicHandle);
				if (soundSource != null && soundSource.IsNotCompleted)
					soundSource.Volume = musicVolume;
			}
		}
	}
}

