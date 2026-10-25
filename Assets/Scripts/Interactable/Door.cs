using UnityEngine;
using System.Collections.Generic;

public class Door : Interactable, INPCPathPoint
{
    Door _door;
    [SerializeField] Vector2 _teleportOffset;

    HashSet<INPCPathPoint> _neighbors = new HashSet<INPCPathPoint>();

    public Vector3 TeleportPosition => this.transform.position + (Vector3)_teleportOffset;

    protected override void Start()
    {
        base.Start();
        NPCPathManager.Instance.RegisterPathPoint(this);
    }

    public void LinkDoor(Door door)
    {
        _door = door;
    }

    protected override void CompleteInteract()
    {
        m_interactingCharacter.transform.position = _door.TeleportPosition;
        StopInteract();
    }

    NPCPathPoint _doorPathPoint;

    public NPCPathPoint CreateDoorPathPoint()
    {
        // We can't have the other door directly as a NPC path point, otherwise when entering we will tp back
        // So we create a separate path point for the door's teleport position
        GameObject doorPath = new GameObject($"{name}_DoorPathPoint");
        doorPath.transform.position = TeleportPosition;
        _doorPathPoint = doorPath.AddComponent<NPCPathPoint>();

        foreach (var neighbor in new List<INPCPathPoint>(_neighbors))
        {
            _doorPathPoint.AddNeighbor(neighbor);
            neighbor.AddNeighbor(_doorPathPoint);
        }

        return _doorPathPoint;
    }

    public void LinkToLinkedDoorPathPoint(NPCPathPoint linkedDoorStandIn)
    {
        // Walking through this door lands exactly at the linked door's stand-in
        AddNeighbor(linkedDoorStandIn);
    }

    public void Initialize()
    {
        
    }

    public Vector3 GetPosition()
    {
        return TeleportPosition;
    }

    public void AddNeighbor(INPCPathPoint neighbor)
    {
        // Doors may not have neighbors in this context
        if (!_neighbors.Contains(neighbor))
        {
            _neighbors.Add(neighbor);
        }
    }

    public HashSet<INPCPathPoint> GetNeighbors()
    {
        return _neighbors;
    }

    public void EnterPosition(Transform character)
    {
        character.position = _door.TeleportPosition;
    }
    
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(TeleportPosition, Vector3.one * 0.25f);
    }
}
