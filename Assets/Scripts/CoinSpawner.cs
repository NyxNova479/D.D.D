using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab; // ton prefab de pièce
    public float spawnDistance = 5f; // distance devant le joueur/caméra

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        if (coinPrefab == null) return;

        // Position devant la caméra
        Vector3 spawnPos = transform.position + transform.forward * spawnDistance;
        spawnPos.y = 0f; // sol

        // Instancie la pièce
        GameObject newCoin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

        // Si tu utilises CoinManager, on peut enregistrer directement
        CoinManager manager = FindAnyObjectByType<CoinManager>();
        if (manager != null)
            manager.RegisterCoin(newCoin.transform);
    }
}
