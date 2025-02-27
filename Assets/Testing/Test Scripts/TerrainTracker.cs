using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTracker : MonoBehaviour
{
    public TerrainManager terrainManager;
    public bool parent = false;
    void OnEnable()
    {
        if (parent) return;
        if (gameObject.name == "Draft Terrain") return;
        if (terrainManager == null) terrainManager = transform.parent.parent.GetComponent<TerrainTracker>().terrainManager;
        terrainManager.AddTerrain(transform.parent.gameObject.name, GetComponent<Terrain>());
    }
    void OnDisable()
    {
        if(parent) return;
        if (gameObject.name == "Draft Terrain") return;
        terrainManager.RemoveTerrain(transform.parent.gameObject.name);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
