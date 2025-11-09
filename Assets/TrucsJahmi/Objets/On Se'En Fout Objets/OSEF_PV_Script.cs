using UnityEngine;

public class OSEF_PV_Script : MonoBehaviour
{
    public float multiplicativeHPBonus;
    public bool hasBeenApplied;
    public GameObject player;
    void Start()
    {
        player = GetComponentInParent<ItemHandlerScript>().player;
        ApplyItemEffect();
    }

    void ApplyItemEffect()
    {
        if (!hasBeenApplied)
        {
            //player.GetComponent<PlayerStatsScript>().healthPointMultiplier += multiplicativeHPBonus;
            hasBeenApplied = true;
        }
    }
}
