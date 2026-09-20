using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformRider : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }
    public PlatformsBase currentPlatform { get; private set; }

    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    public void NotifyRegistered(PlatformsBase platform)
    {
        currentPlatform = platform;
    }
    public void NotifyUnregistered(PlatformsBase platform)
    {
        if (currentPlatform == platform) currentPlatform = null;
    }
}