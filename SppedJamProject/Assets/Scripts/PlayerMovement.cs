using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CameraController cameraController;

    private bool isMoving = false;

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // getting keyboard input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput  != 0 || verticalInput != 0)
        {
            cameraController.AddYawInput(horizontalInput);
        }

        MoveBody(horizontalInput, verticalInput);

        RotateBody();
    }

    private void MoveBody(float horizontalInput, float verticalInput)
    {
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        isMoving = movement.magnitude > 0.0f;
        //Debug.Log("is moving?: " + isMoving);

        transform.Translate(movement * Time.deltaTime * moveSpeed, Space.World);
    }

    private void RotateBody()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(targetPosition);
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
