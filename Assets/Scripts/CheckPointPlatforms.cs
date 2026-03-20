using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class CheckPointPlatforms : PlatformsBase
{
    public Vector2 respawnPos;
    public bool isFinal;
    private CompositeCollider2D compCollider;
    public LevelManager levelManager;

    protected override void Start()
    {
        base.Start();
        levelManager = GameObject.Find("SpawnManager").GetComponent<LevelManager>();
        compCollider = GetComponent<CompositeCollider2D>();
        compCollider.GenerateGeometry();
        respawnPos = (Vector2)compCollider.bounds.center + new Vector2(0, 1.5f);
    }

}
