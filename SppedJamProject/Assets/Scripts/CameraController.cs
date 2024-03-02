using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTransform;
    [SerializeField] float turnSpeed = 20f;

    private void LateUpdate()
    {
        transform.position = followTransform.position;
    }

    public void AddYawInput(float amount)
    {
        transform.Rotate(Vector3.up, amount * Time.deltaTime * turnSpeed);
    }
}
