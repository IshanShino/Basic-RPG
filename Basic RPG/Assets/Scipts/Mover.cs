using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{   
    [SerializeField] Transform target;
    void Update()
    {   
        if (Input.GetMouseButtonDown(0))
        {
            MoveToCursor();
        }
    }
    void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        
        if(hasHit == true)
        {
            GetComponent<NavMeshAgent>().destination = hit.point;
        }
    }
}
