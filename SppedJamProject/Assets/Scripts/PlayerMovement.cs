using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CameraController cameraController;
    private Rigidbody rb;
    public AudioSource movementSoundSource;
    public AudioClip movementClip;
    public AudioClip dieClip;


    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // getting keyboard input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        MoveBody(horizontalInput, verticalInput);
    }

    private void MoveBody(float horizontalInput, float verticalInput)
    {
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        isMoving = movement.magnitude > 0.0f;
        //Debug.Log("is moving?: " + isMoving);
        if (CanMove(movement))
        {
            transform.LookAt(transform.position + movement);
            transform.Translate(movement * Time.deltaTime * moveSpeed, Space.World);
        }
        if (isMoving)
        {
            movementSoundSource.clip = movementClip;
            if (!movementSoundSource.isPlaying)
            {
                movementSoundSource.Play();
            }
        }
        else
        {
            movementSoundSource.Stop();
        }
    }

    private bool CanMove(Vector3 direction)
    {
        float distance = 0.05f; // Distance to check for collision

        RaycastHit hit;
        if (rb.SweepTest(direction, out hit, distance) && hit.collider.gameObject.tag == "Column" && !hit.collider.isTrigger)
        {
            // Collision detected
            return false;
        }
        // No collision detected
        return true;
    }
    private void RotateBody()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Grid")
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
