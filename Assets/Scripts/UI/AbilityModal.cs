using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class AbilityModal : MonoBehaviour
{
    public static AbilityModal Instance { get; private set; }

    [SerializeField] Image _abilityImage;
    [SerializeField] TMP_Text _abilityName;
    [SerializeField] TMP_Text _abilityDescription;
    [SerializeField] Image[] _keyIcons;
    [SerializeField] Sprite[] _keySprites; // Key1, Key2, Key3: the enchant symbols

    [Header("Plain Message")]
    [SerializeField] TMP_Text _headerText;
    [SerializeField] GameObject _comboLabel;

    int _openedFrame;
    float _closedTime = -1f;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (Time.frameCount == _openedFrame)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Hide();
    }

    public void Show(AbilitySO ability, string abilityName, string description, Sprite image)
    {
        if (!CanOpen())
            return;

        _headerText.text = "New Ability";
        _abilityName.text = abilityName;
        _abilityDescription.text = description;

        _abilityImage.enabled = image != null;
        _abilityImage.sprite = image;
        _abilityImage.color = Color.white;

        _comboLabel.SetActive(true);
        for (int i = 0; i < _keyIcons.Length; i++)
        {
            bool used = i < ability.AbilityKeys.Count;
            _keyIcons[i].enabled = used;
            if (used)
                _keyIcons[i].sprite = _keySprites[(int)ability.AbilityKeys[i]];
        }

        Open();
    }

    // Same modal, text only: no image, name or combo
    public void ShowMessage(string title, string message)
    {
        if (!CanOpen())
            return;

        _headerText.text = title;
        _abilityName.text = "";
        _abilityDescription.text = message;

        _abilityImage.enabled = false;
        _comboLabel.SetActive(false);
        foreach (Image icon in _keyIcons)
            icon.enabled = false;

        Open();
    }

    public void Hide()
    {
        _closedTime = Time.unscaledTime;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    // The E press that closed the modal must not open it again
    bool CanOpen()
    {
        return Time.unscaledTime - _closedTime >= 0.2f;
    }

    void Open()
    {
        _openedFrame = Time.frameCount;
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }
}
