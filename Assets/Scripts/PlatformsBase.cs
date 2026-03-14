using UnityEditor;
using UnityEngine;

public abstract class PlatformsBase : MonoBehaviour
{
    public Vector2 platformVelocity { get; protected set; }
    [Header("Gestion de l'inertie ( mettre la valeur 0 pour ne pas brider l'inertie )")]
    public bool takeInertia;
    public float inertiaSpeedMaxX;
    public float inertiaSpeedMaxY;
    
    protected virtual void Start()
    {
        platformVelocity = Vector2.zero;
        inertiaSpeedMaxX = 0f;
        inertiaSpeedMaxY = 0f;
    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }
}
