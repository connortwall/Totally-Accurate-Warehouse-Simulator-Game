using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelveAssets : MonoBehaviour
{
    public float chanceOfDecorAsset = 0.4f;
    public float chanceOfEmptyAsset = 0.3f;
    public int minimumGrabbableAssets = 6;
    public int maximumGrabbableAssets = 7;
    private int grabbableAssetsSpawned = 0;
    public Transform[] assetSpawnPositions;
    public GameObject[] grabbableAssetPrefab;
    public GameObject[] decorAssetPrefab;   


    void Start()
    {
        //PopulateShelves();
    }

    //ALLOW GAME DESIGNERS TO SUBMIT MIN AND MAX THROUGH GAME MANAGER
    public void PopulateShelves(int min, int max){
        minimumGrabbableAssets = min;
        maximumGrabbableAssets = max;
        PopulateShelves();
    }

    public void PopulateShelves()
    {
        // In the Inspector, remember to assign all shelvePosition (as Transforms) and grabbableAssets prefebs
        int amountOfShelvePositions = assetSpawnPositions.Length;
        int amountOfGrabbableAssetPrefabs = grabbableAssetPrefab.Length;
        int amountOfDecorAssetPrefabs = decorAssetPrefab.Length;
        GameObject randomAsset;

        // Instead of always starting at i = 0 and ending at i = 8, a random starting index is preferable (see explanation further down)
        // For example, we might start at i = 5 and end at i = 4 instead (Again, see explanation further down)
        int randomStartingIndex = Random.Range(0,amountOfShelvePositions);

        // Choose a random item prefab for each ShelvePosition
        for (int i = 0; i < amountOfShelvePositions; i++)
        {
            // Decide whether or not the next asset should be spawned as a grabbableAsset
            if (DecideIfGrabbable(amountOfShelvePositions - i))
            {
                // Randomly choose a *GRABBABLE* asset to spawn on the shelf
                int randomInt = Random.Range(0, amountOfGrabbableAssetPrefabs);
                randomAsset = grabbableAssetPrefab[randomInt];
                grabbableAssetsSpawned += 1;
            }
            else if (DecideIfDecor())
            {
                // Randomly choose a *DECOR* asset to spawn on the shelf
                randomAsset = decorAssetPrefab[Random.Range(0,amountOfDecorAssetPrefabs)];
            }
            else
            {
                // Asset is neither grabbable nor a decor, and therefore nothing should be spawned
                continue;
            }
            // i = (0, 1, 2) refers to the three spawn points on the top shelf, i = (3, 4, 5) refers to spawn points on middle shelf, etc.
            // This means that in a normal for-loop, we would always start in the top left corner of a shelf and end in bottom right corner.
            // However, this is not preferable due to the fact that we might have to forcefully spawn in grabbableItems if the minimumGrabbableItems threshhold is not met.
            // It looks akward if these forcefully-spawned grabbableAssets always appear near the bottom right corner of a shelf, which is why we randomize which "i" we start at.
            // For example, we randomly decide which "i" to start at, such as i=(5,6,7,8,0,1,2,3,4) instead of i=(0,1,2,3,4,5,6,7,8).
            int shelveIndex = (i + randomStartingIndex) % amountOfShelvePositions;
            Transform assetSpawnPoint = assetSpawnPositions[shelveIndex];

            // Spawn in "randomAsset" at "assetSpawnPoint"
            GameObject instantiatedAsset = Instantiate(randomAsset, assetSpawnPoint.position, assetSpawnPoint.rotation);

            // The UI panel that displays which items are part of the current order uses the name of the GameObject.
            // However, when instantiating a prefab, Unity automatically sets the name as [prefabname](CLONE), which must be reverted.
            instantiatedAsset.name = randomAsset.name;
        }
    }

    private bool DecideIfGrabbable(int assetSpawnsRemaining)
    {
        // Returns "true" or "false" based on whether or not a given asset should be spawned as a grabbableAsset (as oppossed to being a decorAsset)

        // Each shelf has a "maximumGrabbableAssets".
        // This if-statement ensures that the threshhold mentioned above is not surpassed
        if (maximumGrabbableAssets == grabbableAssetsSpawned)
        {
            return false;
        }
        // Each shelf has a "minimumGrabbableAssets" - this requirement must be met during the generation.
        // If the remaining unspawned assets MUST be grabbable in order to reach this threshhold, forcefully spawn a grabbableAsset
        else if ((grabbableAssetsSpawned + assetSpawnsRemaining) == minimumGrabbableAssets)
        {
            return true;
        }
        else
        {
            // Randomly decide whether or not the asset should be grabbable based on chanceOfDecor variable
            float randomVal = Random.value;
            float chanceOfGrabbableAsset = 1 - chanceOfDecorAsset - chanceOfEmptyAsset;
            return (randomVal < chanceOfGrabbableAsset);
        }
    }

    private bool DecideIfDecor()
    {
        // Math is hard: The following probability is the chance of a *NON-GRABBABLE* asset spawning as a decorAsset (as oppossed to an empty asset)
        // This is different from "chanceOfDecorAsset", which is the chance of *ANY* asset spawning as a decorAsset (with ANY asset being both grabbableAssets and decorAssets)
        float adjustedChanceOfDecorAsset = chanceOfDecorAsset / (chanceOfDecorAsset + chanceOfEmptyAsset);
        // Randomly decide whether or not the *NON-GRABBABLE* asset should be spawned in as a decorAsset (as oppossed to the shelf spot simply being left empty)
        float randomVal = Random.value;
        return (randomVal < adjustedChanceOfDecorAsset);
    }
}
