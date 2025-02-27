using System.Threading.Tasks;
using UnityEngine;

public class TerrainDigger : MonoBehaviour
{
    public float brushSize = 10f;
    public float digDepth = 0.005f;

    async void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Get the Terrain component from the hit object.
                Terrain hitTerrain = hit.collider.GetComponent<Terrain>();
                if (hitTerrain != null)
                {
                    await ModifyTerrainAsync(hit.point, hitTerrain);
                }
            }
        }
    }

    async Task ModifyTerrainAsync(Vector3 hitPoint, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int heightmapRes = terrainData.heightmapResolution;

        // Convert the world position to normalized terrain coordinates.
        float normX = (hitPoint.x - terrainPos.x) / terrainData.size.x;
        float normZ = (hitPoint.z - terrainPos.z) / terrainData.size.z;
        int mapX = (int)(normX * heightmapRes);
        int mapZ = (int)(normZ * heightmapRes);

        // Convert brush size from world space to heightmap coordinates.
        int brushSizePixels = Mathf.RoundToInt((brushSize / terrainData.size.x) * heightmapRes);

        // Define the area of the heightmap to modify.
        int startX = Mathf.Clamp(mapX - brushSizePixels / 2, 0, heightmapRes);
        int startZ = Mathf.Clamp(mapZ - brushSizePixels / 2, 0, heightmapRes);
        int sizeX = Mathf.Clamp(mapX + brushSizePixels / 2, 0, heightmapRes) - startX;
        int sizeZ = Mathf.Clamp(mapZ + brushSizePixels / 2, 0, heightmapRes) - startZ;

        // Retrieve the current heights.
        float[,] heights = terrainData.GetHeights(startX, startZ, sizeX, sizeZ);

        // Offload the heavy height calculations to a background thread.
        float[,] modifiedHeights = await Task.Run(() =>
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    // Lower the height to simulate digging.
                    heights[z, x] = Mathf.Clamp01(heights[z, x] - digDepth);
                }
            }
            return heights;
        });

        // Apply the modified heights on the main thread.
        terrainData.SetHeights(startX, startZ, modifiedHeights);

        // --- Remove Grass (Detail Objects) in the Dug Area ---

        // The detail resolution can differ from the heightmap resolution.
        int detailResolution = terrainData.detailResolution;
        float detailNormX = (hitPoint.x - terrainPos.x) / terrainData.size.x;
        float detailNormZ = (hitPoint.z - terrainPos.z) / terrainData.size.z;
        int detailX = (int)(detailNormX * detailResolution);
        int detailZ = (int)(detailNormZ * detailResolution);

        int detailBrushSize = Mathf.RoundToInt((brushSize / terrainData.size.x) * detailResolution);

        int detailStartX = Mathf.Clamp(detailX - detailBrushSize / 2, 0, detailResolution);
        int detailStartZ = Mathf.Clamp(detailZ - detailBrushSize / 2, 0, detailResolution);
        int detailSizeX = Mathf.Clamp(detailX + detailBrushSize / 2, 0, detailResolution) - detailStartX;
        int detailSizeZ = Mathf.Clamp(detailZ + detailBrushSize / 2, 0, detailResolution) - detailStartZ;

        // Loop through each detail layer and remove grass by setting values to 0.
        for (int layer = 0; layer < terrainData.detailPrototypes.Length; layer++)
        {
            int[,] details = terrainData.GetDetailLayer(detailStartX, detailStartZ, detailSizeX, detailSizeZ, layer);
            for (int i = 0; i < detailSizeZ; i++)
            {
                for (int j = 0; j < detailSizeX; j++)
                {
                    details[i, j] = 0;
                }
            }
            terrainData.SetDetailLayer(detailStartX, detailStartZ, layer, details);
        }
    }
}
