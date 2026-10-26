using UnityEngine;

public enum SoundState
{
    Idle,
    Playing,
    Paused,
    InDestruction
}

[RequireComponent(typeof(AudioSource))]
public class SoundObject : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] private SoundSO _soundSO;
    private SoundState _state = SoundState.Idle;

    int _clipIndex = -1; // Start at -1 to indicate no clip has been played yet

    private void Awake()
    {
        _audioSource.playOnAwake = false;
        SelfSetSoundSO();
        if (SoundManager.Instance != null)
            SoundManager.Instance.RegisterSoundObject(this);
    }

    private void OnDestroy()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.UnregisterSoundObject(this);
    }

    void Update()
    {
        HandleLifeTime();
    }

    #region Initialization

    public void SetSoundSO(SoundSO soundSO)
    {
        _soundSO = soundSO;

        _audioSource.spatialBlend = _soundSO.SpatialBlend;
        _audioSource.outputAudioMixerGroup = _soundSO.OutputGroup;
        _audioSource.ignoreListenerPause = _soundSO.IgnorePause; // UI sounds keep playing during the pause
        _audioSource.loop = _soundSO.Loop && !_soundSO.IsPlaylist; // Single looping clip


        _clipIndex = -1; // Reset the clip index when setting a new SoundSO
        bool hasNextClip = _soundSO.TryGetNextClip(_clipIndex, out _clipIndex, out AudioClip nextClip);
        if (hasNextClip)
            _audioSource.clip = nextClip;

        if (_soundSO.PlayOnAwake)
            Play();
    }

    void SelfSetSoundSO()
    {
        if (_soundSO == null)
            return;
        SetSoundSO(_soundSO);
    }

    #endregion Initialization

    void HandleLifeTime()
    {
        if (_state != SoundState.Playing) return; // never started, or paused or stopped
        if (_audioSource.isPlaying) return; // Currently playing

        if (_soundSO.OneShot)
        {
            // We destroy the object
            _state = SoundState.InDestruction;
            Destroy(gameObject);
        }
        else // Loop or a playlist
        {
            if (_soundSO.TryGetNextClip(_clipIndex, out _clipIndex, out AudioClip nextClip))
            {
                _audioSource.clip = nextClip;
                Play();
            }
            else
                _state = SoundState.Idle;
        }
    }

    #region Audio Controls

    public void Play()
    {
        if (_state == SoundState.InDestruction) // Prevent further interaction if is going to be destroyed by it's clock
            return;

        // Get a new pitch
        _audioSource.pitch = _soundSO.Pitch;

        _audioSource.Play();
        _state = SoundState.Playing;
    }

    public void Stop()
    {
        if (_state == SoundState.InDestruction) // Prevent further interaction if is going to be destroyed by it's clock
            return;

        _audioSource.Stop();

        if (_soundSO != null && _soundSO.OneShot)
        {
            _state = SoundState.InDestruction;
            Destroy(gameObject);
            return;
        }
        _state = SoundState.Idle;
    }

    public void Pause()
    {
        if (_state == SoundState.InDestruction) // Prevent further interaction if is going to be destroyed by it's clock
            return;

        _audioSource.Pause();
        _state = SoundState.Paused;
    }

    public void Resume()
    {
        if (_state != SoundState.Paused)
            return;

        _audioSource.UnPause();
        _state = SoundState.Playing;
    }

    public void SetVolume(float volume)
    {
        _audioSource.volume = volume;
    }

    #endregion Audio Controls
}
