using UnityEditor;
using UnityEngine;

public abstract class PlatformsBase : MonoBehaviour
{
    public Vector2 platformVelocity { get; protected set; }
    
    protected virtual void Start()
    {
        platformVelocity = Vector2.zero;
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }
}
