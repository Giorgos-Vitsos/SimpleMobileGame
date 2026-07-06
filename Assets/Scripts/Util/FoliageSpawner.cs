using UnityEngine;

public class FoliageSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] foliagePrefabs;

    [Header("Settings")]
    public int spawnCount = 20;
    public float innerPadding = 1f;

    [Header("Size Settings (Height Only)")]
    public float minScale = 1.5f;
    public float maxScale = 2.5f;

    [ContextMenu("Scatter Foliage Now")]
    public void ScatterFoliage()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Bounds bounds = meshRenderer.bounds;

        float minX = bounds.min.x + innerPadding;
        float maxX = bounds.max.x - innerPadding;
        float minZ = bounds.min.z + innerPadding;
        float maxZ = bounds.max.z - innerPadding;
        float surfaceY = bounds.max.y;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), surfaceY, Random.Range(minZ, maxZ));
            GameObject randomPrefab = foliagePrefabs[Random.Range(0, foliagePrefabs.Length)];
            
            GameObject spawnedObject = Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
            spawnedObject.transform.SetParent(transform);
            spawnedObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Height-only randomization
            float randomHeightScale = Random.Range(minScale, maxScale);
            Vector3 parentStretch = transform.lossyScale; 

            spawnedObject.transform.localScale = new Vector3(
                1f / parentStretch.x, 
                randomHeightScale / parentStretch.y, 
                1f / parentStretch.z
            );
        }
    }
}