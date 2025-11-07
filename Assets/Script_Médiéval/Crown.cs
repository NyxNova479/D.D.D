using Unity.VisualScripting;
using UnityEngine;

public class Crown : MonoBehaviour
{

    public static int crown_goal = 5;

    [SerializeField]
    GameObject crown;

    public static bool take = false;
    
    [SerializeField]
    GameObject socle;

    [SerializeField]
    GameObject portal;

    [SerializeField]
    GameObject player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0.1f, 0));


        if ( crown_goal == 0 )
        {
            Destroy(crown);
        }
        
        
        if ( take == false)
        {
            crown.transform.position = new Vector3(socle.transform.position.x, 3, socle.transform.position.z);
        }
        

        if ((crown.transform.position.x - player.transform.position.x) <= 2)
        {
            if ((crown.transform.position.z - player.transform.position.z) <= 2)
            {
                if ((player.transform.position.x - crown.transform.position.x) <= 2)
                {
                    if ((player.transform.position.z - crown.transform.position.z) <= 2)
                    {
                        crown.transform.position = new Vector3(player.transform.position.x, player.transform.position.y+ 2, player.transform.position.z);
                        take = true;
                    }
                }
            }
        }
    }
}
