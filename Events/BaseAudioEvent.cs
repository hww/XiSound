using UnityEngine;

namespace XiSound.Events
{
	/// <summary>
	/// Single audio event. 
	/// </summary>
	public abstract class BaseAudioEvent : ScriptableObject {
		
		public abstract bool Play(AudioSource source, bool playSound = true);
		public abstract bool Play(AudioSource source, string clipName, bool playSound = true);
		
	}	
}

