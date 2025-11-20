using UnityEngine;

public class CoinRecolter : MonoBehaviour
{
    [Header("Statistiques du joueur")]
    public static int nombreDePieces = 0;
    void Awake()
    {
        nombreDePieces = 0;
    }

    public void SetCoins(int nbr)
    {
        nombreDePieces = nbr;
    }

    public void Pay(int price)
    {
        nombreDePieces -= price;
    }
}
