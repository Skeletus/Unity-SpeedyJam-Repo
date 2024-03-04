using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingPistol : MonoBehaviour
{
    [Header("Energy Ammo")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyRechargeRate = 5f;
    [SerializeField] private float energyConsumptionRate = 10f;

    [Header("Required Components")]
    [SerializeField] private EnergyBar energyBar;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Damage Area")]
    [SerializeField] private float aoeAngle = 45f;
    [SerializeField] private float aoeRadius = 5f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float damageAmount = 10f;

    private float currentEnergy;
    // list to save all detected enemy locations 
    private List<Vector3> enemyLocations = new List<Vector3>();

    private void Awake()
    {
        currentEnergy = maxEnergy;
        energyBar.SetEnergy(currentEnergy);

        if (energyBar == null)
        {
            Debug.LogError("reference not added.");
        }
    }

    private void Update()
    {
        DetectEnemyInAOE();

        RechargeEnergy();

        if (Input.GetButtonDown("Fire1") && currentEnergy >= energyConsumptionRate)
        {
            ShootRay();
        }
    }

    private void DetectEnemyInAOE()
    {
        enemyLocations.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = (collider.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

                if (angleToEnemy <= aoeAngle * 0.5f)
                {
                    enemyLocations.Add(collider.transform.position);
                    //Debug.Log("enemy detected");
                    Debug.DrawLine(transform.position, collider.transform.position, Color.red);
                }
            }
        }
    }

    private void RechargeEnergy()
    {
        if (playerMovement.IsMoving())
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += energyRechargeRate * Time.deltaTime;
                if (energyBar != null)
                {
                    energyBar.SetEnergy(currentEnergy / maxEnergy);
                }
            }
        }
    }

    private void ShootRay()
    {
        currentEnergy -= energyConsumptionRate;

        HitAndStunEnemy();
        //Debug.Log("pium pium!");


        if (energyBar != null)
        {
            energyBar.SetEnergy(currentEnergy / maxEnergy);
        }

        // routine to reactive enemy movement
        StartCoroutine(ReactivateEnemyMovement());
    }

    private void HitAndStunEnemy()
    {
        // stun all enemies detected
        foreach (Vector3 enemyLocation in enemyLocations)
        {
            Collider[] hitColliders = Physics.OverlapSphere(enemyLocation, 0.5f); // Ajusta el radio según tus necesidades

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // stop all detecte enemies movement and do damage
                    EnemyMovement enemyMovement = hitCollider.GetComponent<EnemyMovement>();
                    HealthSystem enemyHealth = hitCollider.GetComponent<HealthSystem>();
                    if (enemyMovement != null)
                    {
                        enemyMovement.SetStunned();
                    }
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damageAmount);
                    }
                }
            }
        }
    }

    // routine to reactivate enemy movent after x second of stun duration
    private IEnumerator ReactivateEnemyMovement()
    {
        yield return new WaitForSeconds(stunDuration);

        // reactive all enemies movement
        foreach (Vector3 enemyLocation in enemyLocations)
        {
            Collider[] hitColliders = Physics.OverlapSphere(enemyLocation, 0.5f);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    EnemyMovement enemyMovement = hitCollider.GetComponent<EnemyMovement>();
                    if (enemyMovement != null)
                    {
                        enemyMovement.SetFreeMovement();
                    }
                }
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

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * aoeRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * aoeRadius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
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
