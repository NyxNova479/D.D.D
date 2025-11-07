using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBigLaserScript : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.3f);
        StartCoroutine("SecondTimer");
    }
    void Update()
    {
        transform.localScale -= new Vector3(1, 1, 0) * Time.deltaTime;
    }
    public IEnumerator SecondTimer()
    {        
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
