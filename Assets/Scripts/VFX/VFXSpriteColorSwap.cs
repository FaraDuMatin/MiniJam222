using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class VFXSpriteColorSwap : MonoBehaviour
{
    public Color from1 = Color.white;
    public Color from2 = Color.white;
    public Color from3 = Color.white;

    static readonly int From1ID = Shader.PropertyToID("_From1");
    static readonly int From2ID = Shader.PropertyToID("_From2");
    static readonly int From3ID = Shader.PropertyToID("_From3");

    public Color to1 = Color.white;
    public Color to2 = Color.white;
    public Color to3 = Color.white;

    static readonly int To1ID = Shader.PropertyToID("_To1");
    static readonly int To2ID = Shader.PropertyToID("_To2");
    static readonly int To3ID = Shader.PropertyToID("_To3");

    SpriteRenderer _spriteRenderer;
    MaterialPropertyBlock _materialPropertyBlock;

    void OnValidate() => Apply();
    void Awake() => Apply();

    public void Apply()
    {
        if (_spriteRenderer == null) 
            _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_materialPropertyBlock == null) 
            _materialPropertyBlock = new MaterialPropertyBlock();
        _spriteRenderer.GetPropertyBlock(_materialPropertyBlock);

        _materialPropertyBlock.SetColor(From1ID, from1);
        _materialPropertyBlock.SetColor(From2ID, from2);
        _materialPropertyBlock.SetColor(From3ID, from3);

        _materialPropertyBlock.SetColor(To1ID, to1);
        _materialPropertyBlock.SetColor(To2ID, to2);
        _materialPropertyBlock.SetColor(To3ID, to3);

        _spriteRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}