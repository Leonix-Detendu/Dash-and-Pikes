using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class CheckPointPlatforms : PlatformsBase
{
    public Vector2 respawnPos;
    public bool isBeginning;
    private CompositeCollider2D compCollider;

    protected override void Start()
    {
        base.Start();
        compCollider = GetComponent<CompositeCollider2D>();
        compCollider.GenerateGeometry();
        respawnPos = (Vector2)compCollider.bounds.center + new Vector2(0, 1);
    }

}
