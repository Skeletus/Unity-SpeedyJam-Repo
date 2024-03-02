using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{

    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float forgetTime = 2f;

    private float forgetTimer;
    private Transform target;

    private void Awake()
    {
        forgetTimer = forgetTime;
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        UpdateForgetTimer();
        UpdatePlayerPosition();
        MoveTowardsTarget();
    }

    private void UpdatePlayerPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionRange);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= visionAngle * 0.5f)
                {
                    Debug.Log("player detected");
                    forgetTimer = forgetTime;
                    target = collider.transform;
                    Debug.Log("target: " + target.position);
                }
            }
        }
    }

    private void UpdateForgetTimer()
    {
        forgetTimer -= Time.deltaTime;
        if (forgetTimer <= 0)
        {
            forgetTimer = 0;
            target = null;
            Debug.Log("target lost");
        }
        if ( target != null )
        {
            Debug.Log("last seen targe at: " + target.position);
        }
        Debug.Log(forgetTimer);
    }

    private void MoveTowardsTarget()
    {
        if ( target!= null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            transform.Translate(directionToTarget * speed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else
        {
            transform.Translate(Vector3.zero);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        // Dibujar el ángulo de visión en el editor de Unity
        float halfFOV = visionAngle * 0.5f;
        Quaternion leftRayRotation = Quaternion.Euler(0f, -halfFOV, 0f);
        Quaternion rightRayRotation = Quaternion.Euler(0f, halfFOV, 0f);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(transform.position, leftRayDirection * visionRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * visionRange);

        // Dibujar el área de visión en el editor de Unity
        Gizmos.DrawWireSphere(transform.position, visionRange);

        if (target != null )
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, target.position);
        }
    }
}
