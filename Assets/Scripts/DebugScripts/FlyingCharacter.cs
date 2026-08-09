using UnityEngine;

public class FlyingCharacter : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    void Update()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(moveInputX, moveInputY).normalized;

        transform.position += (Vector3) direction * moveSpeed * Time.deltaTime;
    }
}
