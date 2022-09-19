using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiSound.Events;

namespace XiSound
{
    public class XiSoundDemoScene : MonoBehaviour
    {
        public float repeatSound = 5f;
        public AudioEvent audioEvent;
        public AudioEvent musicEvent;
        public MusicManager musicManager;

        SoundSystem soundSystem;
        float playAt;

        // Start is called before the first frame update
        void Start()
        {
            // Create the sound system
            soundSystem = new SoundSystem(null, null);
            musicManager = new MusicManager(null, musicEvent);
            // Initialize the sound system
            SoundSystem.PreInitialize();
            MusicManager.PlayMusic();
            // Start the timer
            playAt = Time.time + repeatSound;
        }

        // Update is called once per frame
        void Update()
        {
            // Verify the timer and restart sound
            if (Time.time > playAt)
            {
                playAt = Time.time + repeatSound;
                SoundSystem.Play(audioEvent);
                Debug.Log("Play Sound");
            }
            // Update the sound system
            SoundSystem.OnUpdate();
        }
    }
}
