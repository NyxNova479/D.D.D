using System.Drawing;
using UnityEngine;

public class Pique : MonoBehaviour
{

    public static bool activate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if( activate == false)
        {
            transform.position = new Vector3(transform.position.x, -0.65f, transform.position.z);

        }

        else
        {
            transform.position = new Vector3(transform.position.x, 0.96f, transform.position.z);
        }

    }
}