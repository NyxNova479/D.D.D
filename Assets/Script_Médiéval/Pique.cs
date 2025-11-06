using System.Drawing;
using UnityEngine;

public class Pique : MonoBehaviour
{

    public static bool activate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if( activate == false)
        {
            transform.position = new Vector3(transform.position.x, 0.19f, transform.position.z);

        }

        else
        {
            transform.position = new Vector3(transform.position.x, 6.5f, transform.position.z);
        }

    }
}