using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levelPatterns;
    public GameObject firstLevel;
    public bool shouldChangeLevel;
    private GameObject currentLevel;
    private void Start()
    {
        firstLevel = Instantiate(firstLevel, Vector3.zero, firstLevel.transform.rotation);

        LevelData firstLevelData = firstLevel.GetComponent<LevelData>();

        Vector3 startLocalPos = firstLevelData.startPos.localPosition;

        firstLevel.transform.position = startLocalPos;

        currentLevel = firstLevel;
    }

    private void Update()
    {
        if (shouldChangeLevel)
        {
            GenerateNewLevel();
            shouldChangeLevel = false;
        }
    }

    private void GenerateNewLevel()
    {
        int indexOfNextLevel = Random.Range(0, levelPatterns.Length);
        GameObject nextLevel = levelPatterns[indexOfNextLevel];

        LevelData currentLevelData = currentLevel.GetComponent<LevelData>();
        Vector3 currentFinalPos = currentLevelData.endPos.position;

        nextLevel = Instantiate(nextLevel, Vector3.zero, nextLevel.transform.rotation);
        
        LevelData nextLevelData = nextLevel.GetComponent<LevelData>();
        Vector3 nextStartLocalPos = nextLevelData.startPos.localPosition;

        nextLevel.transform.position = currentFinalPos - nextStartLocalPos;

        Destroy(currentLevel);
        currentLevel = nextLevel;
    }
}
