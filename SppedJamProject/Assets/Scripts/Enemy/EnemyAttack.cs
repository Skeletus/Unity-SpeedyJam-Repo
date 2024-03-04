using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Damage Area")]
    [SerializeField] private float aoeAngle = 45f;
    [SerializeField] private float aoeRadius = 5f;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float cooldownTime = 2f;

    private Transform target;
    private bool canAttack = true;
    private bool didAttack = false;
    private float timer;

    private void Awake()
    {
        timer = cooldownTime;
    }

    private void Update()
    {
        DetectPlayerInAOE();
        if (didAttack)
        {
            InitiateCooloDownTimer();
        }

    }

    private void DetectPlayerInAOE()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Vector3 directionToEnemy = (collider.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

                if (angleToEnemy <= aoeAngle * 0.5f)
                {
                    target = collider.transform;
                    AttackPlayer(collider);
                    didAttack = true;
                    Debug.DrawLine(transform.position, collider.transform.position, Color.red);
                }
            }
        }
    }

    private void InitiateCooloDownTimer()
    {
        canAttack = false;
        timer -= Time.deltaTime;
        //Debug.Log(timer);
        if (timer <= 0)
        {
            timer = cooldownTime;
            canAttack = true;
        }
    }

    private void AttackPlayer(Collider collider)
    {
        HealthSystem healthSystem = collider.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            if (canAttack)
            {
                healthSystem.TakeDamage(damageAmount);
            }
        }
    }

    private void DrawAOE()
    {
        Vector3 pistolForward = transform.forward;
        pistolForward.y = 0f;

        float halfAngle = aoeAngle * 0.5f;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * pistolForward;

        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
        Vector3 rightRayDirection = rightRayRotation * pistolForward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * aoeRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * aoeRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, leftRayDirection * aoeRadius);
        Gizmos.DrawRay(transform.position, rightRayDirection * aoeRadius);
        Gizmos.DrawSphere(transform.position, 0.1f); 
        Gizmos.DrawWireSphere(transform.position, aoeRadius); 
    }

    private void OnDrawGizmos()
    {
        DrawAOE();
    }
}
