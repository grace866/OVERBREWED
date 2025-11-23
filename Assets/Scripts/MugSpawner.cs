using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MugSpawner : MonoBehaviour
{
    public string prefabFolderPath = "Prefabs";  // Path to prefabs in Resources folder (relative to "Assets/Resources/")
    // private Dictionary<int, GameObject> prefabDictionary = new();
    public List<int> Servable = new();

    [SerializeField] GameObject mug;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int maxMugs = 1;
    public List<GameObject> activeMugs = new();

    void Start()
    {
        LoadAllPrefabs();
    }

    public GameObject RespawnMug()
    {
        if(!mug)
        {
           Debug.LogWarning("didn't put in the mug prefab yet");
           return null;
        }

        if (activeMugs.Count >= maxMugs)
        {
            Debug.Log("max mugs reached");
            return null;
        }
        var position = spawnPoint.position;
        var rotation = spawnPoint.rotation;
        Debug.Log("spawned successfully");
        var newMug = Instantiate(mug, position, rotation);
        activeMugs.Add(newMug);
        return newMug;
    }

    public void UnregisterMug(GameObject mugInstance) => activeMugs.Remove(mugInstance);

    
    // Load all prefabs from Resources/Prefabs folder
    void LoadAllPrefabs(){
        /*
        // Load all prefabs in the specified folder
        GameObject[] prefabs = Resources.LoadAll<GameObject>(prefabFolderPath);
        
        // Iterate through each prefab and store relevant data
        foreach (var prefab in prefabs)
        {
            // Check if prefab has ItemData (or any component storing data)
            var item = prefab.GetComponent<ItemScript>();
            if (item != null)
            {
                // Store prefab in dictionary with ID as key
                prefabDictionary[item.ItemID] = prefab;
                if (item.IsServable) {
                    Servable.Add(item.ItemID);
                }
            }
            else
            {
                Debug.LogWarning($"Prefab {prefab.name} does not have a valid ItemScript component.");
            }
        }
    }

    // Example of spawning an item at a given position
    public void SpawnItem(int itemID)
    {
        if (prefabDictionary.TryGetValue(itemID, out GameObject prefab))
        {
            Instantiate(prefab);
        }
        else
        {
            Debug.LogError($"No prefab found for item ID {itemID}");
        }*/
}
}
