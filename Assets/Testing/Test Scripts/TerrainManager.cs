using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Dictionary<string, Terrain> activeTerrain = new Dictionary<string, Terrain>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddTerrain(string terrainName, Terrain terrainObject)
    {
        if(!activeTerrain.ContainsKey(terrainName))
        {
            activeTerrain.Add(terrainName, terrainObject);
        }
    }
    public void RemoveTerrain(string terrainName)
    {
        if (activeTerrain.ContainsKey(terrainName))
        {
            activeTerrain.Remove(terrainName);
        }
    }
}
