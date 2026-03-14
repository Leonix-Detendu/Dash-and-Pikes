using System.Collections;
using UnityEngine;
public class TempPlatforms : MonoBehaviour
{
    private bool shouldDisappear = true;
    public float disappearDelay;
    public float reappearDelay;
    private PlayerController playerControllerScript;
    private SpriteRenderer[] childrenRenderers;
    private BoxCollider2D[] childrenColliders2D;
    private CompositeCollider2D platformCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        platformCollider = GetComponent<CompositeCollider2D>();
        childrenColliders2D = GetComponentsInChildren<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (shouldDisappear) StartCoroutine(DisappearRoutine());
            shouldDisappear = false;
        }
    }

    IEnumerator DisappearRoutine()
    {
        
        // mettre l'animation de disparition plus tard, je me contente pour le moment de faire disparaitre la plateforme 
        yield return new WaitForSeconds(disappearDelay);

        foreach ( SpriteRenderer childRenderer in childrenRenderers)
        {
            childRenderer.enabled = false;    
        }
        if (platformCollider != null) platformCollider.enabled = false;
        foreach ( BoxCollider2D childCollider in childrenColliders2D)
        {
            childCollider.enabled = false;
        }

        yield return new WaitForSeconds(reappearDelay);

        // mettre l'animation de réapparition plus tard, pour l'instant je la fais réapparaitre brutalement 
        foreach ( SpriteRenderer childRenderer in childrenRenderers)
        {
            childRenderer.enabled = true;
        }
        if ( platformCollider != null ) platformCollider.enabled = true;
        foreach (BoxCollider2D childCollider in childrenColliders2D)
        {
            childCollider.enabled = true;
        }

        shouldDisappear = true;
    }
}
