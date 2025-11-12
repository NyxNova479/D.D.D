using UnityEngine;

public class UniversalItemGiverScript : MonoBehaviour
{
    public GameObject item;
    public GameObject playerItemHandler;
    public bool itemHasBeenGiven;

    void Start()
    {

    }

    void Update()
    {
        if (!itemHasBeenGiven)
        {
            if ((playerItemHandler.transform.position - transform.position).magnitude < 3)
            {
                GiveItem();
                itemHasBeenGiven = true;
            }
        }
    }

    void GiveItem()
    {
        var givenItem = Instantiate(item, transform.position, Quaternion.identity);
        givenItem.transform.SetParent(playerItemHandler.transform);
        playerItemHandler.GetComponent<ItemHandlerScript>().activeItemList.Add(givenItem);
        Destroy(gameObject);
    }
}
