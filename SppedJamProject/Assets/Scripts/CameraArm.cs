using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour
{
    [SerializeField] float armLenght;
    [SerializeField] Transform child;

    // Update is called once per frame
    void Update()
    {
        child.position = transform.position - child.forward * armLenght;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(child.position, transform.position);
    }
}
