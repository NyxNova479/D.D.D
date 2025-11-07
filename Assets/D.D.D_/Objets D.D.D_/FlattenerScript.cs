using UnityEngine;

public class FlattenerScript : MonoBehaviour
{
    
    public GameObject[] objectsToFlatten;

    void Start()
    {
        //objectsToFlatten = new GameObject[15];
        for (int i = 0; i < objectsToFlatten.Length; i++)
        {
            objectsToFlatten[i].transform.localScale = new Vector3(objectsToFlatten[i].transform.localScale.x, 0.001f, objectsToFlatten[i].transform.localScale.z);
        }
    }
}
