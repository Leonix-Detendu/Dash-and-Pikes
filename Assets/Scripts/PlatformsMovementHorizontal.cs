using UnityEngine;

public class PlatformsMovementHorizontal : PlatformsBase
{

    public float rangeLeft;
    public float rangeRight;
    public float speed;
    [Range(0,2)] public float startOffset;
    private float initialTime;

    private float defaultXPosition;
    private float totalRange;
    private float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        defaultXPosition = transform.position.x; 
        totalRange = rangeRight + rangeLeft;
        offset = startOffset * totalRange;
        initialTime = Time.time;   
    }

    // Update is called once per frame
    protected override Vector2 MovePlatform()
    { 
        float progress = Mathf.PingPong(((Time.fixedTime - initialTime) * speed) + offset, totalRange);

        float leftBound = defaultXPosition - rangeLeft;

        float nextPositionX = leftBound + progress;

        Vector2 nextPos = new Vector2(nextPositionX, platformRb.position.y);

        platformRb.MovePosition(nextPos);

        return nextPos;
    }
}
