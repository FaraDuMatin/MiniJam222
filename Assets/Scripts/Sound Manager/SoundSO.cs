using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundSO_", menuName = "Scriptable Objects/SoundSO")]
public class SoundSO : ScriptableObject
{
    [SerializeField] private List<AudioClip> _audioClips;
    [SerializeField] AudioMixerGroup _outputGroup;

    [SerializeField] private bool _isPlaylist = false;
    [SerializeField] private bool _randomClipInPlaylist = false; // Option for playlist, instead of sequential order

    [SerializeField] private bool _playOnAwake = true;
    [SerializeField] private bool _loop = false; // Repeating the sound indefinitely if true
    [SerializeField] private bool _oneshot = false; // The object will be destroyed after playing if true
    [SerializeField] private Vector2 _pitchRange = new Vector2(1f, 1f);

    [Tooltip("0 = 2D, 1 = 3D")]
    [SerializeField, Range(0f, 1f)] private float _spatialBlend = 0f;

    [Tooltip("Keeps playing while the game is paused (UI sounds).")]
    [SerializeField] private bool _ignorePause = false;

    public AudioMixerGroup OutputGroup => _outputGroup;
    public bool IsPlaylist => _isPlaylist;

    public bool PlayOnAwake => _playOnAwake;
    public float Pitch => RandomPitch();
    public float SpatialBlend => _spatialBlend;
    public bool OneShot => _oneshot;
    public bool IgnorePause => _ignorePause;

    public bool TryGetNextClip(int currentIndex, out int nextIndex, out AudioClip nextClip)
    {
        nextIndex = -1;
        nextClip = null;

        // A single sound only plays again if it loops
        if (!_isPlaylist)
        {
            // Are we looping and if yes is our index superior or equal to 0 (already played)
            if (!_loop && currentIndex >= 0) 
                return false;

            nextIndex = 0;
            nextClip = _audioClips[0];
            return true;
        }

        if (_randomClipInPlaylist && _audioClips.Count > 1)
        {
            do nextIndex = Random.Range(0, _audioClips.Count);
            while (nextIndex == currentIndex);

            nextClip = _audioClips[nextIndex];
            return true;
        }

        nextIndex = currentIndex + 1;

        // Clips still left in the list
        if (nextIndex < _audioClips.Count)
        {
            nextClip = _audioClips[nextIndex];
            return true;
        }

        // Reached the end, so start again only if the playlist loops
        if (!_loop)
        {
            nextIndex = -1;
            return false;
        }

        nextIndex = 0;
        nextClip = _audioClips[0];
        return true;
    }

    float RandomPitch()
    {
        return Random.Range(_pitchRange.x, _pitchRange.y);
    }
    
    public SoundObject Play()
    {
        return SoundManager.Instance.CreateSound(this);
    }
}
