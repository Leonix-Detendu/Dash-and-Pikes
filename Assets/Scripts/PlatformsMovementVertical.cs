using UnityEngine;

public class PlatformsMovementVertical : PlatformsBase
{

    public float rangeUp;
    public float rangeDown;
    public float speed;
    [Range(0, 2)] public float startOffset;
    private float initialTime;

    private float defaultYPosition;
    private float totalRange;
    private float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        defaultYPosition = transform.position.y;
        totalRange = rangeUp + rangeDown;
        offset = startOffset * totalRange;
        initialTime = Time.time;
    }

    // Update is called once per frame
    protected override Vector2 MovePlatform()
    {

        float progress = Mathf.PingPong(((Time.fixedTime - initialTime) * speed) + offset, totalRange);

        float downBound = defaultYPosition - rangeDown;

        float nextPositionY = downBound + progress;

        Vector2 nextPos = new Vector2(platformRb.position.x, nextPositionY);

        platformRb.MovePosition(nextPos); 

        return nextPos;
    }
}
