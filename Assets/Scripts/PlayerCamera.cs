using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _heightOffset = 2f;
    [SerializeField] float _followSpeed = 5f;
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] float _teleportDistance = 100f;

    void LateUpdate()
    {
        Vector3 targetPosition = _playerCharacter.transform.position + Vector3.up * _heightOffset;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _followSpeed);

        if(Vector2.Distance(targetPosition, transform.position) > _teleportDistance)
            transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }
}
