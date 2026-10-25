using UnityEngine;

public class DoorLinker : MonoBehaviour
{
    [SerializeField] Door _door1;
    [SerializeField] Door _door2;

    public void Awake()
    {
        if(_door1 == null || _door2 == null)
        {
            Debug.LogError("Both doors must be assigned in the DoorLinker.", this.gameObject);
            return;
        }

        _door1.LinkDoor(_door2);
        _door2.LinkDoor(_door1);
    }

    void Start()
    {
        if(_door1 == null || _door2 == null)
            return;

        // Runs in Start (not Awake) because it depends on every NPCPathPoint's Awake()
        // having already run to reciprocally register itself as a neighbor of its linked doors
        NPCPathPoint standIn1 = _door1.CreateDoorPathPoint();
        NPCPathPoint standIn2 = _door2.CreateDoorPathPoint();

        _door1.LinkToLinkedDoorPathPoint(standIn2);
        _door2.LinkToLinkedDoorPathPoint(standIn1);

        Destroy(this.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if(_door1 != null && _door2 != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_door1.transform.position, _door2.transform.position);
        }
    }
}
