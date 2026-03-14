using UnityEngine;

public class PlatformsMovementVertical : PlatformsBase
{
    private Rigidbody2D rb;

    public float rangeUp;
    public float rangeDown;
    public float speed;
    [Range(0, 2)] public float startOffset;

    private float defaultYPosition;
    private float totalRange;
    private float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultYPosition = rb.position.y;
        totalRange = rangeUp + rangeDown;
        offset = startOffset * totalRange;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float progress = Mathf.PingPong((Time.fixedTime * speed) + offset, totalRange);

        float downBound = defaultYPosition - rangeDown;

        float nextPositionY = downBound + progress;

        platformVelocity = new Vector2(platformVelocity.x, (nextPositionY - rb.position.y) / Time.fixedDeltaTime );

        rb.MovePosition(new Vector2(rb.position.x, downBound + progress));
    }
}
