using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformsBase : MonoBehaviour
{
    public Vector2 platformVelocity { get; protected set; }
    [HideInInspector] public bool canBoost { get; protected set; }
    [HideInInspector] public bool shouldBoost {  get; protected set; }
    [HideInInspector] public Vector2 boostVector { get; protected set; }

    protected Rigidbody2D platformRb;

    public float superBoost;
    public float angleBoost;

    public readonly HashSet<PlatformRider> currentRiders = new HashSet<PlatformRider>();
    protected bool shouldRide;

    private void Awake()
    {
        platformRb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        platformVelocity = Vector2.zero;
        canBoost = false;
        shouldBoost = false;
        superBoost = 0f;
    }

    protected virtual void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector2 delta = MovePlatform() - platformRb.position;
        platformVelocity = delta / Time.fixedDeltaTime;
    }

    protected abstract Vector2 MovePlatform();

    public void RegisterRider(PlatformRider rider)
    {
        currentRiders.Add(rider);
        rider.NotifyRegistered(this);
    }
    public void UnregisterRider(PlatformRider rider)
    {
        currentRiders.Remove(rider);
        rider.NotifyUnregistered(this);
    }

    public void ForceDetachAllRiders()
    {
        foreach (var rider in new List<PlatformRider>(currentRiders))
        {
            UnregisterRider(rider);
        }
    }
}
