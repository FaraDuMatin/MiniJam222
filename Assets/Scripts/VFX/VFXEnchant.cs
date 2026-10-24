using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class VFXEnchant : MonoBehaviour
{
    [Header("Large Renderer")]
    [SerializeField] SpriteRenderer _largeRenderer;
    [SerializeField] float _largeDisplayDuration = 0.3f;
    [SerializeField] float _largeFadeDuration = 0.3f;
    [Space]
    [SerializeField] Sprite _largeKey1;
    [SerializeField] Sprite _largeKey2;
    [SerializeField] Sprite _largeKey3;
    Coroutine _useKeyCoroutine;

    [Header("Small Renderers")]
    [SerializeField] SpriteRenderer[] _smallRenderers;
    [Space]
    [SerializeField] Sprite _smallKey1;
    [SerializeField] Sprite _smallKey2;
    [SerializeField] Sprite _smallKey3;

    [Header("Enchantment VFX")]
    [SerializeField] Animator _enchantmentAnimator;

    void Awake()
    {
        Color color = _largeRenderer.color;
        color.a = 0f;
        _largeRenderer.color = color;

        foreach (var renderer in _smallRenderers)
        {
            renderer.gameObject.SetActive(false);
        }
    }

    IEnumerator Coroutine_UseKey()
    {
        Color color = _largeRenderer.color;
        color.a = 1f;
        _largeRenderer.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < _largeDisplayDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(_largeDisplayDuration);

        elapsedTime = 0f;
        while (elapsedTime < _largeFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / _largeFadeDuration);
            _largeRenderer.color = color;
            yield return null;
        }

        color.a = 0f;
        _largeRenderer.color = color;

        _useKeyCoroutine = null;
    }

    public void VFXUseKey(AbilityKey key)
    {
        _largeRenderer.sprite = key switch
        {
            AbilityKey.Key1 => _largeKey1,
            AbilityKey.Key2 => _largeKey2,
            AbilityKey.Key3 => _largeKey3,
            _ => _largeRenderer.sprite
        };

        if (_useKeyCoroutine != null)
            StopCoroutine(_useKeyCoroutine);
        _useKeyCoroutine = StartCoroutine(Coroutine_UseKey());
    }

    public void VFXKeyList(List<AbilityKey> keys)
    {
        for (int i = 0; i < _smallRenderers.Length; i++)
        {
            if (i < keys.Count)
            {
                _smallRenderers[i].gameObject.SetActive(true);
                _smallRenderers[i].sprite = keys[i] switch
                {
                    AbilityKey.Key1 => _smallKey1,
                    AbilityKey.Key2 => _smallKey2,
                    AbilityKey.Key3 => _smallKey3,
                    _ => _smallRenderers[i].sprite
                };
            }
            else
            {
                _smallRenderers[i].gameObject.SetActive(false);
            }
        }
    }

    public void PlayEnchantmentVFX(AbilitySO ability)
    {
        _enchantmentAnimator.SetTrigger("Enchant");
        if(_useKeyCoroutine != null)
            StopCoroutine(_useKeyCoroutine);
        _useKeyCoroutine = null;
        _largeRenderer.color = new Color(_largeRenderer.color.r, _largeRenderer.color.g, _largeRenderer.color.b, 0f);
    }
}
