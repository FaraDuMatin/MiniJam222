using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] CanvasGroup _settingsCanvasGroup;
    bool _isSettingsVisible = false;

    [SerializeField] Slider _generalVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;
    [SerializeField] Slider _uiVolumeSlider;

    public bool IsSettingsVisible => _isSettingsVisible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _settingsCanvasGroup.alpha = 0f;
        _settingsCanvasGroup.interactable = false;
        _settingsCanvasGroup.blocksRaycasts = false;
    }

    void Start()
    {
        _generalVolumeSlider.value = 0.5f;
        _musicVolumeSlider.value = 0.5f;
        _sfxVolumeSlider.value = 0.5f;
        _uiVolumeSlider.value = 0.5f;

        SetGeneralVolume(_generalVolumeSlider.value);
        SetMusicVolume(_musicVolumeSlider.value);
        SetSFXVolume(_sfxVolumeSlider.value);
        SetUIVolume(_uiVolumeSlider.value);
    }

    void OnEnable()
    {
        _generalVolumeSlider.onValueChanged.AddListener(SetGeneralVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        _uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);
    }

    void OnDisable()
    {
        _generalVolumeSlider.onValueChanged.RemoveListener(SetGeneralVolume);
        _musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        _uiVolumeSlider.onValueChanged.RemoveListener(SetUIVolume);
    }

    public void SetGeneralVolume(float volume)
    {
        SoundManager.Instance.SetVolume(SoundManager.MASTER_VOLUME, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SoundManager.Instance.SetVolume(SoundManager.MUSIC_VOLUME, volume);
    }
    
    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetVolume(SoundManager.SFX_VOLUME, volume);
    }

    public void SetUIVolume(float volume)
    {
        SoundManager.Instance.SetVolume(SoundManager.UI_VOLUME, volume);
    }

    public void ShowSettings(bool show)
    {
        _isSettingsVisible = show;
        _settingsCanvasGroup.alpha = show ? 1f : 0f;
        _settingsCanvasGroup.interactable = show;
        _settingsCanvasGroup.blocksRaycasts = show;
    }
}
