using UnityEditor;
using UnityEngine;

public abstract class PlatformsBase : MonoBehaviour
{
    public Vector2 platformVelocity { get; protected set; }
    [HideInInspector] public bool canBoost { get; protected set; }
    [HideInInspector] public bool shouldBoost {  get; protected set; }
    [HideInInspector] public Vector2 boostVector { get; protected set; }

    public float superBoost;
    public float angleBoost;
    
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

    protected virtual void FixedUpdate()
    {

    }
}
