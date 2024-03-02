using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody rigidBody;
    private Camera mainCamera;
    private CameraController cameraController;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        cameraController = FindObjectOfType<CameraController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // getting keyboard input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        MoveBody(horizontalInput, verticalInput);

        RotateBody();

        if (horizontalInput != 0 || verticalInput != 0)
        {
            cameraController.AddYawInput(horizontalInput);
        }
    }

    private void MoveBody(float horizontalInput, float verticalInput)
    {
        Vector3 rightDirection = mainCamera.transform.right;
        Vector3 forwardDirection = mainCamera.transform.forward;

        Vector3 moveDirection = (rightDirection * horizontalInput + forwardDirection * verticalInput).normalized;
        transform.Translate(moveDirection * Time.deltaTime * moveSpeed, Space.World);
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
}
