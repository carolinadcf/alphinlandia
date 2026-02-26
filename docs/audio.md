# Audio System

## Context

Audio is split into two responsibilities: **sound effects** (one-shot clips spawned at a world position) and **volume mixing** (master, SFX, and music channels exposed to UI sliders). Both are decoupled from gameplay logic so any script can trigger a sound without knowing about the audio setup.

---

## `SoundFXManager` — `Scripts/Managers/SoundFXManager.cs`

Singleton. Responsible for playing one-shot sound effects at a position in the world.

### How It Works

When a sound is triggered, `SoundFXManager` instantiates `SoundFXObject.prefab` at the given world position, assigns the clip and volume, plays it, then destroys the GameObject after the clip finishes. No persistent AudioSource is kept on the caller.

### Methods

```csharp
// Play a specific clip
public void PlaySoundFXClip(AudioClip clip, Transform transform, float volume)

// Pick a random clip from an array and play it
public void PlayRandomSoundFXClip(AudioClip[] clips, Transform transform, float volume)
```

### Usage

```csharp
// From any MonoBehaviour:
SoundFXManager.instance.PlaySoundFXClip(myClip, transform, 1f);
SoundFXManager.instance.PlayRandomSoundFXClip(mySoundArray, transform, 0.8f);
```

### `SoundFXObject.prefab`

A minimal GameObject with an `AudioSource` component. Instantiated at the sound's world position and self-destroyed when playback ends. Do not place this prefab in the scene manually — it is managed entirely by `SoundFXManager`.

---

## `SoundMixerManager` — `Scripts/Managers/SoundMixerManager.cs`

Not a singleton. Attached to a GameObject in the scene and wired to UI sliders in the settings menu.

Converts a linear `float` value (0–1, from a UI slider) to decibels and sets it on the `AudioMixer`.

### Methods

```csharp
public void SetMasterVolume(float volume)   // → "masterVolume" mixer parameter
public void SetSFXVolume(float volume)      // → "soundFXVolume" mixer parameter
public void SetMusicVolume(float volume)    // → "musicVolume" mixer parameter
```

### AudioMixer Setup

The `AudioMixer` must expose three float parameters with exactly these names:

| Parameter | Channel |
|-----------|---------|
| `masterVolume` | Master |
| `soundFXVolume` | SFX group |
| `musicVolume` | Music group |

Wire each settings slider's `OnValueChanged` event to the corresponding method on `SoundMixerManager`.

---

## Rules

- `SoundFXManager` is a singleton — only one instance per scene.
- Do not call `SoundFXManager` before it has been initialized (i.e. from `Awake()` on another object at the same frame). Use `Start()` or later.
- Volume values passed to `SoundMixerManager` must be in the range `(0, 1]`. Passing `0` to `Mathf.Log10` returns `-Infinity`, which will mute the channel permanently until the mixer is reset.

---

← [Back to Overview](overview.md)
