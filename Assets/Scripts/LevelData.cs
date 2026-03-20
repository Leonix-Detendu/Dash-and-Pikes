using UnityEngine;

public class LevelData : MonoBehaviour
{
    public GameObject startingPlatform;
    public GameObject endingPlatform;

    private CompositeCollider2D startingCollider;
    private CompositeCollider2D endingCollider;

    public Transform startPos;
    public Transform endPos;

    private void Awake()
    {
        startingCollider = startingPlatform.GetComponent<CompositeCollider2D>();
        endingCollider = endingPlatform.GetComponent<CompositeCollider2D>();

        startingCollider.GenerateGeometry();
        endingCollider.GenerateGeometry();

        startPos.position = startingCollider.bounds.center;
        endPos.position = endingCollider.bounds.center;
    }
}
