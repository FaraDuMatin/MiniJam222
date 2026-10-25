using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private SoundObject _soundObjectPrefab;
    HashSet<SoundObject> _soundObjects = new HashSet<SoundObject>();

    // Audio mixer for controlling global sound volumes
    // Steps to create sound categories : 
    // 1. Open the Audio Mixer window (Window -> Audio -> Audio Mixer).
    // 2. Create a new Audio Mixer or select an existing one.
    // 3. Add groups for different sound categories (e.g., Music, SFX, Voice).
    // 4. Expose the volume parameters of each group to script control. (Right click on a Volume field -> Expose 'Volume (of category)' to script).
    // 5. Top right of the Audio Mixer window, open Exposed Parameters and rename it if needed.
    [SerializeField] private AudioMixer _audioMixer;

    public const string MASTER_VOLUME = "MasterVolume";
    public const string MUSIC_VOLUME = "MusicVolume";
    public const string SFX_VOLUME = "SFXVolume";
    public const string UI_VOLUME = "UIVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        RegisterSoundObjectsInAwake();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
            
        UnregisterAllSoundObjects();
    }

    #region Sound Registration

    void RegisterSoundObjectsInAwake()
    {
        SoundObject[] soundObjects = FindObjectsByType<SoundObject>(FindObjectsSortMode.None);
        foreach (var soundObject in soundObjects)
        {
            _soundObjects.Add(soundObject);
        }
    }

    public void RegisterSoundObject(SoundObject soundObject)
    {
        _soundObjects.Add(soundObject);
    }

    public void UnregisterSoundObject(SoundObject soundObject)
    {
        _soundObjects.Remove(soundObject);
    }

    void UnregisterAllSoundObjects()
    {
        _soundObjects.Clear();
    }

    #endregion Sound Registration

    /// <summary>
    /// Create a new SoundObject instance.
    /// </summary>
    /// <returns>The newly created SoundObject instance.</returns>
    public SoundObject CreateSound(SoundSO soundSO)
    {
        SoundObject soundObject = Instantiate(_soundObjectPrefab);
        soundObject.SetSoundSO(soundSO);
        // We aren't registering the sounds in this method for constitency between registering and unregistering (both called in Awake and OnDestroy)
        return soundObject;
    }

    public void SetVolume(string parameter, float volume)
    {
        // Sounds are in dB and not a linear 0-1 volume scale so we need to change dB to a linear 0-1
        float decibels = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        _audioMixer.SetFloat(parameter, decibels);
    }
}
