using UnityEngine;
using System.Collections.Generic;

public class NPCPathPoint : MonoBehaviour, INPCPathPoint
{
    [SerializeField] List<NPCPathPoint> _pathNeighbors = new List<NPCPathPoint>();
    [SerializeField] List<Door> _doorNeighbors = new List<Door>();
    HashSet<INPCPathPoint> _neighbors = new HashSet<INPCPathPoint>();

    void Awake()
    {
        foreach (var door in _doorNeighbors)
        {
            AddNeighbor(door);
            door.AddNeighbor(this);
        }
    }

    void Start()
    {
        NPCPathManager.Instance.RegisterPathPoint(this);
    }

    public void Initialize()
    {
        foreach (var neighbor in _pathNeighbors)
        {
            AddNeighbor(neighbor);
            neighbor.AddNeighbor(this);
        }
    }

    public Vector3 GetPosition()
    {
        return this.transform.position;
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
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(GetPosition(), Vector3.one * 0.2f);
    }
}
