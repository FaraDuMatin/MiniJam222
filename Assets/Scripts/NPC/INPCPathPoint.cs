using UnityEngine;
using System.Collections.Generic;

public interface INPCPathPoint
{
    void Initialize();
    Vector3 GetPosition();

    GameObject GetGameObject();

    void AddNeighbor(INPCPathPoint neighbor);
    HashSet<INPCPathPoint> GetNeighbors();

    void EnterPosition(Transform character);
}
