using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject centralPoint;
    public float wideOfDeadZone;
    public float heightOfDeadZone;

    private float rightBound, leftBound, upBound, downBound;

    public float cameraSpeed;

    public GameObject player;
    private Rigidbody2D playerRb;

    private void Start()
    {
        rightBound = centralPoint.transform.position.x + wideOfDeadZone / 2;
        leftBound = centralPoint.transform.position.x - wideOfDeadZone / 2;
        upBound = centralPoint.transform.position.x + heightOfDeadZone / 2;
        downBound = centralPoint.transform.position.x - heightOfDeadZone / 2;
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (playerRb.position.x >= rightBound)
        {
            Vector2 targetPosition = new Vector2(rightBound, centralPoint.transform.position.y);
            centralPoint.transform.position = Vector2.MoveTowards(centralPoint.transform.position, targetPosition, playerRb.linearVelocityX * Time.deltaTime);
            rightBound = centralPoint.transform.position.x + wideOfDeadZone / 2;
            leftBound = centralPoint.transform.position.x - wideOfDeadZone / 2;
        }
        if (playerRb.position.x <= leftBound)
        {
            Vector2 targetPosition = new Vector2(leftBound, centralPoint.transform.position.y);
            centralPoint.transform.position = Vector2.MoveTowards(centralPoint.transform.position, targetPosition, -playerRb.linearVelocityX * Time.deltaTime);
            rightBound = centralPoint.transform.position.x + wideOfDeadZone / 2;
            leftBound = centralPoint.transform.position.x - wideOfDeadZone / 2;
        }
        if (playerRb.position.y >= upBound)
        {
            Vector2 targetPosition = new Vector2(centralPoint.transform.position.x, upBound);
            centralPoint.transform.position = Vector2.MoveTowards(centralPoint.transform.position, targetPosition, playerRb.linearVelocityY * Time.deltaTime);
            upBound = centralPoint.transform.position.y + heightOfDeadZone / 2;
            downBound = centralPoint.transform.position.y - heightOfDeadZone / 2;
        }
        if (playerRb.position.y <= downBound)
        {
            Vector2 targetPosition = new Vector2(centralPoint.transform.position.x, downBound);
            centralPoint.transform.position = Vector2.MoveTowards(centralPoint.transform.position, targetPosition, -playerRb.linearVelocityY * Time.deltaTime);
            upBound = centralPoint.transform.position.y + heightOfDeadZone / 2;
            downBound = centralPoint.transform.position.y - heightOfDeadZone / 2;
        }
    }
}
