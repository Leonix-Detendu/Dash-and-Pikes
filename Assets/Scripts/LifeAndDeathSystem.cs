using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal.Internal;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            CheckPointPlatforms currentCheckpoint = collision.gameObject.GetComponent<CheckPointPlatforms>();

            if (currentCheckpoint != null)
            {
                ContactPoint2D contact = collision.GetContact(0);

                if (contact.normal.y > 0.5f)
                {
                    newCheckPoint(currentCheckpoint);
                }
            }
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

        if (healthBar <= 0)
        {
            gameOver = true;
            Debug.Log("game over");
        }
    }

    private void newCheckPoint(CheckPointPlatforms CheckPoint)
    {
        respawnPos = CheckPoint.respawnPos;

        if (CheckPoint.isFinal)
        {
            CheckPoint.levelManager.shouldChangeLevel = true;
        }

        Debug.Log("Nouveau point de respawn enregistré : " + respawnPos);
    }
}
