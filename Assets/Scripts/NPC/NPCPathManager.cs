using UnityEngine;
using System.Collections.Generic;

public class NPCPathManager : MonoBehaviour
{
    public static NPCPathManager Instance { get; private set; }

    [SerializeField] PlayerCharacter _playerCharacter;

    HashSet<INPCPathPoint> _pathPoints = new HashSet<INPCPathPoint>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterPathPoint(INPCPathPoint pathPoint)
    {
        _pathPoints.Add(pathPoint);
        pathPoint.Initialize();
    }


    public INPCPathPoint GetClosestPathPoint(Vector3 position)
    {
        INPCPathPoint closest = null;
        float closestDist = float.MaxValue;

        foreach (var point in _pathPoints)
        {
            float dist = Vector3.Distance(point.GetPosition(), position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = point;
            }
        }

        return closest;
    }

    public List<INPCPathPoint> FindPath(INPCPathPoint start, INPCPathPoint target)
    {
        Dictionary<INPCPathPoint, float> distances = new Dictionary<INPCPathPoint, float>();
        Dictionary<INPCPathPoint, INPCPathPoint> previous = new Dictionary<INPCPathPoint, INPCPathPoint>();
        HashSet<INPCPathPoint> visited = new HashSet<INPCPathPoint>();
        List<INPCPathPoint> unvisited = new List<INPCPathPoint>(_pathPoints);

        foreach (var point in unvisited)
            distances[point] = float.MaxValue;
        distances[start] = 0f;

        while (unvisited.Count > 0)
        {
            INPCPathPoint current = null;
            float currentDist = float.MaxValue;
            foreach (var point in unvisited)
            {
                if (distances[point] < currentDist)
                {
                    currentDist = distances[point];
                    current = point;
                }
            }

            if (current == null)
                break;

            unvisited.Remove(current);
            visited.Add(current);

            if (current == target)
                break;

            foreach (var neighbor in current.GetNeighbors())
            {
                if (visited.Contains(neighbor))
                    continue;

                float newDist = distances[current] + Vector3.Distance(current.GetPosition(), neighbor.GetPosition());
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                }
            }
        }

        if (!previous.ContainsKey(target) && start != target)
            return null; // no path found

        List<INPCPathPoint> path = new List<INPCPathPoint>();
        INPCPathPoint step = target;
        path.Add(step);
        while (step != start)
        {
            step = previous[step];
            path.Add(step);
        }
        path.Reverse();

        return path;
    }

    public List<INPCPathPoint> GetPathTo(Vector3 npcPosition, INPCPathPoint target)
    {
        INPCPathPoint start = GetClosestPathPoint(npcPosition);
        return FindPath(start, target);
    }

    public List<INPCPathPoint> GetPathToPlayer(Vector3 npcPosition)
    {
        INPCPathPoint start = GetClosestPathPoint(npcPosition);
        INPCPathPoint target = GetClosestPathPoint(_playerCharacter.transform.position);

        return FindPath(start, target);
    }


    void OnDrawGizmos()
    {
        Color color = Color.green;
        foreach (var pathPoint in _pathPoints)
        {
            HashSet<INPCPathPoint> neighbors = pathPoint.GetNeighbors();
            foreach (var neighbor in neighbors)
            {
                color = Color.green;
                color.a = 1;
                Gizmos.color = color;
                Vector3 start = pathPoint.GetPosition();
                Vector3 end = neighbor.GetPosition();
                Vector3 mid = start + (end - start) / 2f;
                Gizmos.DrawLine(start, mid);

                // color = Color.yellow;
                // color.a = 0.5f;
                // Gizmos.color = color;
                // Gizmos.DrawLine(mid, end);
            }
        }
    }
}
