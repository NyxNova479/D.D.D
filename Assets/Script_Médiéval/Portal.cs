using TMPro;
using UnityEngine;

public class Portal : MonoBehaviour
{

    [SerializeField]
    GameObject crown;

    [SerializeField]
    GameObject portal;

    [SerializeField]
    GameObject pique;

    [SerializeField]
    Color point;

    [SerializeField]
    Color piquer;

    [SerializeField]
    Color boutons;

    public static int portal_color = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                Instantiate(pique, new Vector3(j*5-70, -4.19f, i*5-70), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(UnityEngine.KeyCode.P))
        {
            portal_color += 1;
        }

        if ( Crown.crown_goal > 0 )
        {
            if ( portal_color > 3 )
            {
                portal_color -= 3;
            }

            if (portal_color == 1)
            {
                GetComponent<MeshRenderer>().material.color = point;

                if ((portal.transform.position.x - crown.transform.position.x) <= 6)
                {
                    if ((portal.transform.position.z - crown.transform.position.z) <= 6)
                    {
                        if ((crown.transform.position.x - portal.transform.position.x) <= 6)
                        {
                            if ((crown.transform.position.z - portal.transform.position.z) <= 6)
                            {

                                Crown.take = false;
                                Crown.crown_goal -= 1;

                            }
                        }
                    }
                }
            }

            if (portal_color == 2)
            {
                GetComponent<MeshRenderer>().material.color = piquer;

                if ((portal.transform.position.x - crown.transform.position.x) <= 6)
                {
                    if ((portal.transform.position.z - crown.transform.position.z) <= 6)
                    {
                        if ((crown.transform.position.x - portal.transform.position.x) <= 6)
                        {
                            if ((crown.transform.position.z - portal.transform.position.z) <= 6)
                            {

                                Crown.take = false;
                                Pique.activate = (false == Pique.activate);

                            }
                        }
                    }
                }
            }

            if (portal_color == 3)
            {
                GetComponent<MeshRenderer>().material.color = boutons;

                if ((portal.transform.position.x - crown.transform.position.x) <= 6)
                {
                    if ((portal.transform.position.z - crown.transform.position.z) <= 6)
                    {
                        if ((crown.transform.position.x - portal.transform.position.x) <= 6)
                        {
                            if ((crown.transform.position.z - portal.transform.position.z) <= 6)
                            {

                                Crown.take = false;
                                

                            }
                        }
                    }
                }
            }
        }
    }
}
