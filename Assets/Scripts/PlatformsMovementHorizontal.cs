using UnityEngine;

public class PlatformsMovementHorizontal : PlatformsBase
{
    private Rigidbody2D rb;

    public float rangeLeft;
    public float rangeRight;
    public float speed;
    [Range(0,2)] public float startOffset;

    private float defaultXPosition;
    private float totalRange;
    private float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultXPosition = transform.position.x; 
        totalRange = rangeRight + rangeLeft;
        offset = startOffset * totalRange;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    { 
        base.FixedUpdate();

        float progress = Mathf.PingPong((Time.fixedTime * speed) + offset, totalRange);

        float leftBound = defaultXPosition - rangeLeft;

        float nextPositionX = leftBound + progress;

        platformVelocity = new Vector2((nextPositionX - rb.position.x) / Time.fixedDeltaTime, platformVelocity.y);

        rb.MovePosition(new Vector2(leftBound + progress, rb.position.y));
    }
}
