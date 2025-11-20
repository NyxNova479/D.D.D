using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Camera mainCamera;

    [Header("Attraction")]
    public float distanceMax = 5f;
    public float attractionSpeed = 5f;

    [Header("Collecte")]
    public AudioClip coinClip;
    [Range(0f, 1f)] public float volumeMin = 0.9f;
    [Range(0f, 1f)] public float volumeMax = 1f;
    [Range(0.5f, 2f)] public float pitchMin = 0.95f;
    [Range(0.5f, 2f)] public float pitchMax = 1.05f;
    public float soundCooldown = 0.05f;

    private static float lastSoundTime = 0f;
    private readonly List<Transform> coins = new List<Transform>();

    private float distanceMaxSqr;

    [Header("More Optimisation")]
    public float distanceMaxView = 250f;


    void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        distanceMaxSqr = distanceMax * distanceMax;

        // Enregistre toutes les pièces actuelles
        foreach (var coin in GameObject.FindGameObjectsWithTag("Coin"))
            coins.Add(coin.transform);
    }

    void LateUpdate()
    {
        if (player == null || coins.Count == 0) return;

        Vector3 playerPos = player.position;
        Vector3 camPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;
        float delta = Time.deltaTime;
        float attract = attractionSpeed * delta;

        for (int i = coins.Count - 1; i >= 0; i--)
        {
            


            Transform coin = coins[i];
            if (coin == null)
            {
                coins.RemoveAt(i);
                continue;
            }

            Vector3 toPlayer = playerPos - coin.position;
            float sqrDist = toPlayer.sqrMagnitude;

            if (sqrDist > distanceMaxView * distanceMaxView)
            {
                
                if (coin.gameObject.activeSelf)
                    coin.gameObject.SetActive(false);
                continue;
            }
            else
            {
                
                if (!coin.gameObject.activeSelf)
                    coin.gameObject.SetActive(true);
            }

            // Ignore si trop loin
            float dist = 0f;
            bool nearPlayer = sqrDist <= distanceMaxSqr;

            if (nearPlayer)
            {
                dist = Mathf.Sqrt(sqrDist);
                coin.position = Vector3.MoveTowards(
                    coin.position,
                    playerPos,
                    attract * (1f + (distanceMax - dist))
                );
            }

            // Toujours orienter vers la caméra (même si loin)
            if (mainCamera != null)
            {
                Vector3 dir = camPos - coin.position;
                if (dir.sqrMagnitude > 0.001f)
                    coin.rotation = Quaternion.LookRotation(dir);
            }

            // Collecte seulement si proche
            if (nearPlayer && dist < 0.1f)
            {
                CoinRecolter.nombreDePieces++;
                PlaySound();
                Destroy(coin.gameObject);
                coins.RemoveAt(i);
            }

        }
    }

    void PlaySound()
    {
        if (coinClip == null) return;
        if (Time.time - lastSoundTime < soundCooldown) return;
        lastSoundTime = Time.time;

        GameObject audioObj = new GameObject("CoinSound");
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0.5f;
        source.pitch = Random.Range(pitchMin, pitchMax);
        source.volume = Random.Range(volumeMin, volumeMax);
        source.clip = coinClip;
        source.Play();
        Destroy(audioObj, coinClip.length + 0.05f);
    }

    public void RegisterCoin(Transform coin)
    {
        if (!coins.Contains(coin))
            coins.Add(coin);
    }
}
