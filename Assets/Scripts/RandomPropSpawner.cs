using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RandomPropSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PropData
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float spawnChance = 1f;
        public float radius = 0.5f; // rayon de sécurité pour éviter le chevauchement
    }

    [Header("Props à générer")]
    public List<PropData> props = new List<PropData>();

    [Header("Paramètres de génération")]
    public int maxTries = 200;
    public int maxProps = 50;
    public LayerMask groundLayer;

    [Header("Zones interdites")]
    public LayerMask forbiddenZones; // LayerMask pour les colliders d’exclusion

    [Header("Facteur d’échelle global")]
    public float scaleFactor = 0.2f; // divise la taille par 5

    private MeshRenderer meshRenderer;
    private List<Vector3> placedPositions = new List<Vector3>();

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        GenerateProps();
    }

    void GenerateProps()
    {
        Bounds bounds = meshRenderer.bounds;
        int spawnedCount = 0;

        for (int i = 0; i < maxTries && spawnedCount < maxProps; i++)
        {
            PropData selected = GetRandomProp();
            if (selected == null) continue;

            Vector3 randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 5f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // Raycast vers le bas pour trouver le sol
            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 20f, groundLayer))
            {
                Vector3 position = hit.point;

                // Vérifie si la position est dans une zone interdite
                if (Physics.CheckSphere(position, selected.radius, forbiddenZones))
                    continue;

                // Vérifie qu’on ne chevauche pas un autre prop
                if (!IsPositionFree(position, selected.radius))
                    continue;

                // Instanciation
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                GameObject instance = Instantiate(selected.prefab, position, rotation, transform);
                instance.transform.localScale *= scaleFactor;

                placedPositions.Add(position);
                spawnedCount++;
            }
        }

        Debug.Log($"Spawned {spawnedCount} props sur {maxTries} tentatives.");
    }

    PropData GetRandomProp()
    {
        List<PropData> valid = new List<PropData>();
        foreach (var p in props)
        {
            if (Random.value <= p.spawnChance)
                valid.Add(p);
        }

        if (valid.Count == 0) return null;
        return valid[Random.Range(0, valid.Count)];
    }

    bool IsPositionFree(Vector3 pos, float radius)
    {
        foreach (var p in placedPositions)
        {
            if (Vector3.Distance(pos, p) < radius * 2f)
                return false;
        }
        return true;
    }
}
