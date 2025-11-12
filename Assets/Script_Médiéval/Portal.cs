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

        transform.position = new Vector3(Mathf.Pow(-1, Random.Range(-1, 1)) * 55, 0.6f, Mathf.Pow(-1, Random.Range(-1, 1)) * 55);

        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                Instantiate(pique, new Vector3(j*5-70+ Random.Range(-4, 1), -4.19f, i*5-70 + Random.Range(-4, 1)), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(0, 0.1f, 0));

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

                if ((portal.transform.position.x - crown.transform.position.x) <= 3)
                {
                    if ((portal.transform.position.z - crown.transform.position.z) <= 3)
                    {
                        if ((crown.transform.position.x - portal.transform.position.x) <= 3)
                        {
                            if ((crown.transform.position.z - portal.transform.position.z) <= 3)
                            {

                                Crown.take = false;
                                Crown.crown_goal -= 1;
                                transform.position = new Vector3(Mathf.Pow(-1, Random.Range(-1, 1)) * 55, 0.6f, Mathf.Pow(-1, Random.Range(-1, 1)) * 55);

                            }
                        }
                    }
                }
            }

            if (portal_color == 2)
            {
                GetComponent<MeshRenderer>().material.color = piquer;

                if ((portal.transform.position.x - crown.transform.position.x) <= 3)
                {
                    if ((portal.transform.position.z - crown.transform.position.z) <= 3)
                    {
                        if ((crown.transform.position.x - portal.transform.position.x) <= 3)
                        {
                            if ((crown.transform.position.z - portal.transform.position.z) <= 3)
                            {

                                Crown.take = false;
                                Pique.activate = (false == Pique.activate);
                                transform.position = new Vector3( Mathf.Pow(-1,Random.Range(-1,1))*55, 0.6f, Mathf.Pow(-1, Random.Range(-1, 1))*55);

                            }
                        }
                    }
                }
            }

            if (portal_color == 3)
            {
                GetComponent<MeshRenderer>().material.color = boutons;

                if ((portal.transform.position.x - crown.transform.position.x) <= 3)
                {
                    if ((portal.transform.position.z - crown.transform.position.z) <= 3)
                    {
                        if ((crown.transform.position.x - portal.transform.position.x) <= 3)
                        {
                            if ((crown.transform.position.z - portal.transform.position.z) <= 3)
                            {

                                Crown.take = false;
                                transform.position = new Vector3(Mathf.Pow(-1, Random.Range(-1, 1)) * 55, 0.6f, Mathf.Pow(-1, Random.Range(-1, 1)) * 55);

                            }
                        }
                    }
                }
            }
        }
    }
}
