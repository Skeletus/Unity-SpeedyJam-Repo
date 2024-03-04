using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private float nextActionTime = 0;
    private float lightningOffTimeAbs = 0;
    private float lightningOffTimeRel = 0.7f;
    private float period = 0.1f;
    public LineRenderer ElectricityRenderer;
    private Vector3 playerPosition;

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
                playerPosition = collider.gameObject.transform.position;
                playerPosition.y += 1f;
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
        StrikeLightening();
    }

    public void StrikeLightening()
    {
        if (Time.time < lightningOffTimeAbs)
        {
            ElectricityRenderer.enabled = true;
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                Vector3 targetPosition = playerPosition;
                Vector3 startPosition = transform.position;
                float distance = Vector3.Distance(targetPosition, startPosition);
                float sectionDistance = 1f;

                int sectionsCount = (int)(distance / sectionDistance);
                float inc = 1.0f / (float)sectionsCount;
                float maxDiff = inc / 3;
                float maxYdif = 1f;
                var vertices = Enumerable.Range(0, sectionsCount).ToDictionary(i => i,
                    i =>
                    {
                        var incSkew = (i > 0 && i < sectionsCount - 1) ? Random.Range(-maxDiff, maxDiff) : 0f;
                        var zero = Vector3.Lerp(startPosition, targetPosition, i * inc + incSkew);
                        if (i > 0 && i < sectionsCount - 1) zero.y = zero.y + Random.Range(-maxYdif, maxYdif);
                        return zero;
                    });

                ElectricityRenderer.positionCount = sectionsCount;
                foreach (var vertice in vertices)
                {
                    ElectricityRenderer.SetPosition(vertice.Key, vertice.Value);
                }
            }
        }
        else
        {
            ElectricityRenderer.enabled = false;
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
                lightningOffTimeAbs = Time.time + lightningOffTimeRel;
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
