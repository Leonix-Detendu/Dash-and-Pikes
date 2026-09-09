using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levelPatterns;
    [SerializeField] private List<GameObject> levelsRemaining = new List<GameObject>();
    public GameObject firstLevel;
    public bool shouldChangeLevel;
    private GameObject currentLevel;

    private void Start()
    { 
        LevelData firstLevelData = firstLevel.GetComponent<LevelData>();

        fillLevelsList();

        levelsRemaining.Remove(firstLevel); 

        Vector3 startLocalPos = firstLevelData.startPos.localPosition;

        currentLevel = Instantiate(firstLevel, -startLocalPos, firstLevel.transform.rotation);

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
        GameObject nextLevelPrefab;

        if (levelsRemaining.Count == levelPatterns.Length)
        {
            nextLevelPrefab = firstLevel;
        }
        else
        {
            int indexOfNextLevel = Random.Range(0, levelsRemaining.Count);

            nextLevelPrefab = levelsRemaining[indexOfNextLevel];
        }

        levelsRemaining.Remove(nextLevelPrefab);

        LevelData currentLevelData = currentLevel.GetComponent<LevelData>();
        Vector3 currentFinalPos = currentLevelData.endPos.position;

        LevelData nextLevelData = nextLevelPrefab.GetComponent<LevelData>();
        Vector3 nextStartLocalPos = nextLevelData.startPos.localPosition;

        Vector3 spawnPosition = currentFinalPos - nextStartLocalPos;

        GameObject nextLevel = Instantiate(nextLevelPrefab, spawnPosition, nextLevelPrefab.transform.rotation);

        Destroy(currentLevel);
        currentLevel = nextLevel;

        if (levelsRemaining.Count == 0)
        {
            fillLevelsList();
        }
    }

    private void fillLevelsList()
    {
        foreach (GameObject level in levelPatterns)
        {
            levelsRemaining.Add(level);
        }
    }
}
