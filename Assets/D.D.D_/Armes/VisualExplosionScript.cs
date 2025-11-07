using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class VisualExplosionScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("SecondTimer");
    }
    void Update()
    {
        //transform.localScale -= new Vector3(1, 1, 0) * Time.deltaTime;
    }
    public IEnumerator SecondTimer()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
