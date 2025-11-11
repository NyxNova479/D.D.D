using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ItemHandlerScript : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> activeItemList;
    void Start()
    {
        
    }

    void Update()
    {
        UseItems();
    }

    void UseItems()
    {
        /*for (int i = 0; i < activeItemList.Count; i++)
        {
            activeItemList[i].GetComponent<ApplyItemEffect();
        }*/
        player.GetComponent<PlayerStatsScript>().UpdateStats();
    }
}
