using Unity.VisualScripting;
using UnityEngine;

public class Oeuf : MonoBehaviour
{
    [SerializeField]
    GameObject oeuf;
    GameObject[] oeufs;

    [SerializeField]
    GameObject pierre;

    public static int boite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oeufs = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            oeufs[i] = Instantiate(oeuf, new Vector3(Random.Range(0, 500), 0, Random.Range(0, 500)), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}