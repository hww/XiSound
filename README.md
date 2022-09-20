# XiSound _Tiny Sound System for Unity 3D_

![](https://img.shields.io/badge/unity-2018.3%20or%20later-green.svg)
[![⚙ Build and Release](https://github.com/hww/XiSound/actions/workflows/ci.yml/badge.svg)](https://github.com/hww/XiSound/actions/workflows/ci.yml)
[![openupm](https://img.shields.io/npm/v/com.hww.xisound?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.hww.xisound/)
[![](https://img.shields.io/github/license/hww/XiSound.svg)](https://github.com/hww/XiSound/blob/master/LICENSE)
[![semantic-release: angular](https://img.shields.io/badge/semantic--release-angular-e10079?logo=semantic-release)](https://github.com/semantic-release/semantic-release)

The tiny sound library for Unity 3D designed by [hww](https://github.com/hww)

## Introduction

This is an extremely minimalist version similar to FMOD but much simpler and more compact. Good for small game projects Match3, VR, AR etc.

The system was used for one commercial project which sells worldwide. 

## Install

The package is available on the openupm registry. You can install it via openupm-cli.

```bash
openupm add com.hww.xisound
```
You can also install via git url by adding this entry in your manifest.json

```bash
"com.hww.xisound": "https://github.com/hww/XiSound.git#upm"
```

## Usage

Create the set of audio events in the project with RMB/Create/XiSound/SimpleAudioEvent 
Then rename the event and edit it with Unity inspector (see image bellow)

![Audio Event Image](https://raw.githubusercontent.com/hww/XiSound/master/Assets/XiSound/Documentation/AudioEvent.gif)

Each event could have a collection of sounds. Those sounds could be played with valious ways.

- Single _First sound only_
- Sequence _The incremental mode_
- Random _The random order_

## API

The first step is to create the SoundSystem and the MusicManager

```C#
// Assign the AudioEvent with one or more music files
public AudioEvent musicEvent;

// Create the sound system
soundSystem = new SoundSystem(null, null);
// Initialize the music manager
musicManager = new MusicManager(null, musicEvent);
// Initialize the sound system
SoundSystem.PreInitialize();
```

Then the sound system sould be updated every frame.

```C#
void Update()
{
   SoundSystem.OnUpdate(); // Update the sound system
}
```

The various of music methods below

```C#
SoundHandle MusicManager.PlayMusic();
SoundHandle MusicManager.PlayMusic(string clipName);
MusicManager.StopMusic();
MusicManager.MusicVolume = 0.5f;
```

For FX sounds there are this methods in the SoundSystem class.

``` C#
// Play the next sound once and call delegate at the end
SoundHandle Play(AudioEvent audioEvent, SoundSource.OnEndDelegate onChangeState = null)
// Play the sound at the position and call delegate at the end.
// Play the next when clipName is null 
SoundHandle Play(AudioEvent audioEvent, string clipName, Vector3 position, SoundSource.OnEndDelegate onChangeState = null)
// Control the sound
void StopAllSounds()
void StopAllSound(string eventName)
void FadeOutAllSounds()
void FadeOutAllSounds(string eventName)
```

## SoundHandle

When the sound is created and the handle is stored to a variable, the various methods with this sound instance is possible. 

``` C#
// Check if the sound is still exists
bool IsExisting
// Get the Unity sound source
SoundSource GetSource()
// Stop the sound
void Stop()
// Fadeout the sound
void FadeOut()
```

