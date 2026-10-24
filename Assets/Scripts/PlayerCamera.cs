using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] float _followSpeed = 5f;
    [SerializeField] PlayerCharacter _playerCharacter;
    [SerializeField] float _teleportDistance = 100f;

    void LateUpdate()
    {
        Vector3 targetPosition = _playerCharacter.transform.position;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _followSpeed);

        Vector2 targetPosition2D = _playerCharacter.transform.position;
        Vector2 selfPosition2D = transform.position;
        if(Vector2.Distance(targetPosition2D, selfPosition2D) > _teleportDistance)
            transform.position = new Vector3(targetPosition2D.x, targetPosition2D.y, transform.position.z);
    }
}
