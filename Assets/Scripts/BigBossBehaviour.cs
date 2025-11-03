using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BigBossBehaviour : MonoBehaviour
{

    [SerializeField]
    GameObject target;

    private NavMeshAgent _agent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("Nav Mesh agent is null");
        }
    }
    // Update is called once per frame
    void Update()
    {
        _agent.destination = target.transform.position;
    }


}



  