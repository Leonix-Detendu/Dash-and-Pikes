using UnityEngine;

public class RiderDetectionZone : MonoBehaviour
{
    [SerializeField] private PlatformsBase owner;

    private void Awake()
    {
        owner = GetComponentInParent<PlatformsBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlatformRider rider = collision.GetComponent<PlatformRider>();
        if (rider != null && owner != null) owner.RegisterRider(rider);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlatformRider rider = collision.GetComponent<PlatformRider>();
        if (rider != null && owner != null) owner.UnregisterRider(rider);
    }
}
