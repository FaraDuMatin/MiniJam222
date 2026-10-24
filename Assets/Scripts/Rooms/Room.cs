using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Room : MonoBehaviour
{
    [SerializeField] Collider2D _roomCollider;

    [Header("Exterior Renderers")]
    [SerializeField] List<SpriteRenderer> _exteriorRenderers;
    [SerializeField] float _fadeSpeed = 2f;
    float _exteriorFadeValue;
    Coroutine _exteriorFadeCoroutine;

    void Awake()
    {
        _roomCollider.isTrigger = true;
        _exteriorFadeValue = 1f;
        HandleFadeExterior(_exteriorFadeValue);
    }

    void HandleFadeExterior(float value)
    {
        _exteriorFadeValue = value;
        foreach(var renderer in _exteriorRenderers)
        {
            Color color = renderer.color;
            color.a = value;
            renderer.color = color;
        }
    }

    IEnumerator Coroutine_ExteriorFade(bool fadeIn)
    {
        float end = fadeIn ? 1f : 0f;
        
        if(fadeIn)
        {
            while(_exteriorFadeValue < end)
            {
                _exteriorFadeValue += Time.deltaTime * _fadeSpeed;
                HandleFadeExterior(_exteriorFadeValue);
                yield return null;
            }
        }
        else
        {
            while(_exteriorFadeValue > end)
            {
                _exteriorFadeValue -= Time.deltaTime * _fadeSpeed;
                HandleFadeExterior(_exteriorFadeValue);
                yield return null;
            }
        }

        HandleFadeExterior(end);
        _exteriorFadeCoroutine = null;
    }

    public void ShowExterior()
    {
        if(_exteriorFadeCoroutine != null)
            StopCoroutine(_exteriorFadeCoroutine);
        _exteriorFadeCoroutine = StartCoroutine(Coroutine_ExteriorFade(true));
    }
    
    public void HideExterior()
    {
        if(_exteriorFadeCoroutine != null)
            StopCoroutine(_exteriorFadeCoroutine);
        _exteriorFadeCoroutine = StartCoroutine(Coroutine_ExteriorFade(false));
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.attachedRigidbody.gameObject.CompareTag("Player"))
            HideExterior();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.attachedRigidbody.gameObject.CompareTag("Player"))
            ShowExterior();
    }
}
