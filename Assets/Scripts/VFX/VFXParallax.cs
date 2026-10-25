using UnityEngine;

public class VFXParallax : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector2 _parallaxFactor = new Vector2(0.5f, 0.5f);
    [SerializeField] float _pixelsPerUnit = 0f;

    Vector3 _startPosition;
    Vector3 _targetStartPosition;

    void Start()
    {
        _startPosition = transform.position;
        _targetStartPosition = _target.position;
    }

    void LateUpdate()
    {
        Vector3 targetDelta = _target.position - _targetStartPosition;

        Vector3 newPosition = _startPosition + new Vector3(
            targetDelta.x * _parallaxFactor.x,
            targetDelta.y * _parallaxFactor.y,
            0f);

        if (_pixelsPerUnit > 0f)
        {
            newPosition.x = Mathf.Round(newPosition.x * _pixelsPerUnit) / _pixelsPerUnit;
            newPosition.y = Mathf.Round(newPosition.y * _pixelsPerUnit) / _pixelsPerUnit;
        }

        transform.position = newPosition;
    }
}
