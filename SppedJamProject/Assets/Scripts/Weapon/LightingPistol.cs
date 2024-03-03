using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    private float currentEnergy;
    // Lista para almacenar las ubicaciones de los enemigos detectados
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
        // Limpiar la lista de ubicaciones antes de cada detección
        enemyLocations.Clear();

        // Obtener todos los colliders dentro del radio del área de efecto
        Collider[] colliders = Physics.OverlapSphere(transform.position, aoeRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = (collider.transform.position - transform.position).normalized;
                float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

                if (angleToEnemy <= aoeAngle * 0.5f)
                {
                    // Agregar la ubicación del enemigo a la lista
                    enemyLocations.Add(collider.transform.position);
                    //Debug.Log("enemy detected");

                    // Dibujar un raycast hacia el enemigo detectado en el editor
                    Debug.DrawLine(transform.position, collider.transform.position, Color.red);
                }
            }
        }

        RechargeEnergy();

        if (Input.GetButtonDown("Fire1") && currentEnergy >= energyConsumptionRate)
        {
            ShootRay();
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

        // Detener a todos los enemigos detectados
        foreach (Vector3 enemyLocation in enemyLocations)
        {
            Collider[] hitColliders = Physics.OverlapSphere(enemyLocation, 0.5f); // Ajusta el radio según tus necesidades

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Detener el movimiento del enemigo (puedes ajustar esto según la lógica de tu enemigo)
                    EnemyMovement enemyMovement = hitCollider.GetComponent<EnemyMovement>();
                    if (enemyMovement != null)
                    {
                        enemyMovement.SetStunned();
                    }
                }
            }
        }
        Debug.Log("Rayo disparado!");


        if (energyBar != null)
        {
            energyBar.SetEnergy(currentEnergy / maxEnergy);
        }

        // Puedes iniciar una corrutina para reactivar el movimiento después de cierto tiempo
        StartCoroutine(ReactivateEnemyMovement());
    }

    // Corrutina para reactivar el movimiento de los enemigos después del tiempo de aturdimiento
    private IEnumerator ReactivateEnemyMovement()
    {
        yield return new WaitForSeconds(stunDuration);

        // Reactivar el movimiento de todos los enemigos
        foreach (Vector3 enemyLocation in enemyLocations)
        {
            Collider[] hitColliders = Physics.OverlapSphere(enemyLocation, 0.5f); // Ajusta el radio según tus necesidades

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Reactivar el movimiento del enemigo (puedes ajustar esto según la lógica de tu enemigo)
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
        // Calcula la dirección del frente de la pistola en el plano horizontal
        Vector3 pistolForward = transform.forward;
        pistolForward.y = 0f; // Ignora la componente vertical

        // Calcula la mitad del ángulo del cono
        float halfAngle = aoeAngle * 0.5f;

        // Calcula la dirección izquierda del cono
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * pistolForward;

        // Calcula la dirección derecha del cono
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);
        Vector3 rightRayDirection = rightRayRotation * pistolForward;

        // Dibuja el cono usando líneas en el editor
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * aoeRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * aoeRadius);

        // Dibuja el arco del cono
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f); // Rojo con transparencia
        Gizmos.DrawRay(transform.position, leftRayDirection * aoeRadius);
        Gizmos.DrawRay(transform.position, rightRayDirection * aoeRadius);
        Gizmos.DrawSphere(transform.position, 0.1f); // Punto central del cono
        Gizmos.DrawWireSphere(transform.position, aoeRadius); // Límite del cono
    }

    private void OnDrawGizmos()
    {
        DrawAOE();
    }
}
