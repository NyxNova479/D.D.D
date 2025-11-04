using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpriteAnimator : MonoBehaviour
{
    void Start()
    {
        CoinManager manager = FindAnyObjectByType<CoinManager>();
        if (manager != null)
            manager.RegisterCoin(transform);
        else
            Debug.LogWarning("Aucun CoinManager trouvé dans la scène !");
    }
}
