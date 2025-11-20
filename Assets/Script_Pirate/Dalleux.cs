using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Dalleux : MonoBehaviour
{
    [SerializeField]
    GameObject Dalle_M;

    [SerializeField]
    GameObject Dalle_L;

    [SerializeField]
    GameObject Dalle_K;

    bool M = true;
    bool L = true;
    bool K = true;

    bool Mm = false;
    bool Ll = false;
    bool Kk = false;

    float temp_M = 5;
    float temp_L = 8;
    float temp_K = 11;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool Mm = false;
        bool Ll = false;
        bool Kk = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Tempo

        if (Mm)
        {
            Dalle_M.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        else
        {
            Dalle_M.GetComponent<MeshRenderer>().material.color = Color.red;

            if ( M )
            {
                 Dalle_M.GetComponent<MeshRenderer>().enabled = true;

                 if (temp_M <= 0)
                 {
                      M = false;
                
                 }

                else
                {
                
                     temp_M -= Time.deltaTime*10;
                }
            }

            else
            {
                Dalle_M.GetComponent<MeshRenderer>().enabled = false;

                if (temp_M >= 5)
                {
                    M = true;
                }

                else
                {
                    temp_M += Time.deltaTime;
                }
            }
        }

        if(Ll)
        {
            Dalle_L.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        else
        {
            Dalle_L.GetComponent<MeshRenderer>().material.color = Color.blue;

            if (L)
            {
                Dalle_L.GetComponent<MeshRenderer>().enabled = true;

                if (temp_L <= 0)
                {
                    L = false;
                }

                else
                {
                    temp_L -= Time.deltaTime*10;
                }
            }

            else
            {
                Dalle_L.GetComponent<MeshRenderer>().enabled = false;

                if (temp_L >= 8)
                {
                    L = true;
                }

                else
                {
                    temp_L += Time.deltaTime;
                }
            }
        }
        
        if (Kk)
        {
            Dalle_K.GetComponent<MeshRenderer>().material.color = Color.black;
        }

        else
        {
            Dalle_K.GetComponent<MeshRenderer>().material.color = Color.yellow;

            if (K)
            {
                Dalle_K.GetComponent<MeshRenderer>().enabled = true;


                if (temp_K <= 0)
                {
                    K = false;
                }

                else
                {
                    temp_K -= Time.deltaTime*10;
                }
            }

            else
            {
                Dalle_K.GetComponent<MeshRenderer>().enabled = false;


                if (temp_K >= 11)
                {
                    K = true;
                }

                else
                {
                    temp_K += Time.deltaTime;
                }
            }
        }
        
        // Check commandes

        if (Input.GetKeyDown(UnityEngine.KeyCode.L))
        {
            if (M)
            {
                Mm = true;
            }

            else
            {
                Ll = false;
                Kk = false;
            }
        }

        if (Input.GetKeyDown(UnityEngine.KeyCode.K))
        {
            if (L)
            {
                Ll = true;
            }

            else
            {
                Mm = false;
                Kk = false;
            }
        }

        if (Input.GetKeyDown(UnityEngine.KeyCode.J))
        {
            if (K)
            {
                Kk = true;
            }

            else
            {
                Ll = false;
                Mm = false;
            }
        }

        if (Mm && Ll && Kk)
        {
            // recompenses?
            Debug.Log("fait ton boulot Jahmi !!!");
        }
    }
}