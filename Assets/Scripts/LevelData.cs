using UnityEngine;

public class LevelData : MonoBehaviour
{

    public Transform startPos;
    public Transform endPos;
    public GameObject deadZone;
    private BoxCollider2D deadZoneCollider;

    private void Start()
    {
        deadZoneCollider = deadZone.GetComponent<BoxCollider2D>();
    }
}