using UnityEngine;

public class Door : Interactable
{
    Door _door;
    [SerializeField] Vector2 _teleportOffset;

    public Vector3 TeleportPosition => this.transform.position + (Vector3)_teleportOffset;

    public void LinkDoor(Door door)
    {
        _door = door;
    }

    protected override void CompleteInteract()
    {
        m_interactingCharacter.transform.position = _door.TeleportPosition;
        StopInteract();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(TeleportPosition, Vector3.one * 0.2f);
    }
}
