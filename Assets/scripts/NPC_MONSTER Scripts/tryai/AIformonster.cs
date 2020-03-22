using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIformonster : MonoBehaviour {

   // public Transform goalttansform;
    private NavMeshAgent agent;

    // Use this for initialization
    void Start () {

            agent = GetComponent<NavMeshAgent>();
       
		
	}

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {

                agent.SetDestination(hitInfo.point);

            }
        }
    }
}
