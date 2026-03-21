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

    // Ajout d'un filtre pour vérifier les collisions
    private ContactFilter2D contactFilter;

    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        childrenRenderers = GetComponentsInChildren<SpriteRenderer>();
        platformCollider = GetComponent<CompositeCollider2D>();
        childrenColliders2D = GetComponentsInChildren<BoxCollider2D>();

        contactFilter = ContactFilter2D.noFilter;
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
        // 1. Délai avant disparition
        yield return new WaitForSeconds(disappearDelay);

        // 2. On cache le visuel
        foreach (SpriteRenderer childRenderer in childrenRenderers)
        {
            childRenderer.enabled = false;
        }

        // 3. Au lieu de désactiver le collider, on le passe en Trigger (Fantôme)
        SetCollidersTriggerState(true);

        // 4. Délai avant réapparition
        yield return new WaitForSeconds(reappearDelay);

        // 5. On réactive le visuel (le joueur voit la plateforme réapparaître)
        foreach (SpriteRenderer childRenderer in childrenRenderers)
        {
            childRenderer.enabled = true;
        }

        // 6. LA MAGIE EST ICI : La coroutine se met en pause et attend que la zone soit vide !
        yield return new WaitUntil(() => !IsSomethingInside());

        // 7. La voie est libre, on redonne sa solidité à la plateforme
        SetCollidersTriggerState(false);

        shouldDisappear = true;
    }

    // --- MÉTHODES UTILITAIRES ---

    private void SetCollidersTriggerState(bool isTrigger)
    {
        // Comme on l'a vu, si on a un Composite, c'est lui le "maître"
        if (platformCollider != null)
        {
            platformCollider.isTrigger = isTrigger;
        }
        else
        {
            // Sécurité au cas où le Composite est retiré un jour
            foreach (BoxCollider2D childCollider in childrenColliders2D)
            {
                childCollider.isTrigger = isTrigger;
            }
        }
    }

    private bool IsSomethingInside()
    {
        Collider2D[] results = new Collider2D[1];

        if (platformCollider != null)
        {
            // Vérifie si un objet physique touche le CompositeCollider
            return platformCollider.Overlap(contactFilter, results) > 0;
        }

        // Sécurité si pas de CompositeCollider
        foreach (BoxCollider2D childCollider in childrenColliders2D)
        {
            if (childCollider.Overlap(contactFilter, results) > 0)
            {
                return true;
            }
        }

        return false;
    }
}