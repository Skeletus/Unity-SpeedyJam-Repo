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
        // Después de atacar, inicia el temporizador para el próximo ataque
        //StartCoroutine(StartAttackCooldown(cooldownTime));
    }

    private IEnumerator StartAttackCooldown(float cooldownTime)
    {
        canAttack = false;
        //Debug.Log("enemy attack on cd");
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
        //Debug.Log("enemy attack ready");
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
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * aoeRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * aoeRadius);

        // Dibuja el arco del cono
        Gizmos.color = Color.blue;
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
