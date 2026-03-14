using UnityEngine;
using System.Collections;

public class LifeAndDeathSystem : MonoBehaviour
{
    public float initialHealth;
    private float healthBar;

    public bool gameOver;

    public Vector2 respawnPos;
    private bool isImmune;

    private Rigidbody2D rb;
    
    void Start()
    {
        gameOver = false;
        rb = GetComponent<Rigidbody2D>();
        healthBar = initialHealth;
        isImmune = false;
        Debug.Log(healthBar);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isImmune)
        {
            return;
        }

        if (collision.CompareTag("DamageEntity"))
        {
            TakeDamage();

            StartCoroutine(ResetInvulnerability());
        }
    }

    IEnumerator ResetInvulnerability()
    {
        yield return new WaitForSeconds(0.2f);

        isImmune = false;

    }

    private void TakeDamage()
    {
        healthBar--;
        isImmune = true;

        Debug.Log(healthBar);

        rb.position = respawnPos;
        rb.linearVelocity = Vector2.zero;

        if (healthBar == 0)
        {
            gameOver = true;
            Debug.Log("game over");
        }
    }
}
