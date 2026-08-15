using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levelPatterns;
    public GameObject firstLevel;
    public bool shouldChangeLevel;
    private GameObject currentLevel;

    private void Start()
    { 
        LevelData firstLevelData = firstLevel.GetComponent<LevelData>();

        Vector3 startLocalPos = firstLevelData.startPos.localPosition;

        firstLevel = Instantiate(firstLevel, -startLocalPos, firstLevel.transform.rotation);

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
        Debug.Log(indexOfNextLevel);
        GameObject nextLevelPrefab = levelPatterns[indexOfNextLevel];

        LevelData currentLevelData = currentLevel.GetComponent<LevelData>();
        Vector3 currentFinalPos = currentLevelData.endPos.position;

        LevelData nextLevelData = nextLevelPrefab.GetComponent<LevelData>();
        Vector3 nextStartLocalPos = nextLevelData.startPos.localPosition;

        Vector3 spawnPosition = currentFinalPos - nextStartLocalPos;

        GameObject nextLevel = Instantiate(nextLevelPrefab, spawnPosition, nextLevelPrefab.transform.rotation);

        Destroy(currentLevel);
        currentLevel = nextLevel;
    }
}
