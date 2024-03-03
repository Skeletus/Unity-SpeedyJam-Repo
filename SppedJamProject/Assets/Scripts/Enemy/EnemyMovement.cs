using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 4f;

    private bool isStunned = false;
    private Transform target;

    private void Update()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            if (!isStunned)
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
    }

    public void SetStunned()
    {
        isStunned = true;
    }

    public void SetFreeMovement()
    {
        isStunned = false;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void UpdateTarget(Transform target)
    {
        this.target = target;
    }

    public void SetNullTarget()
    {
        this.target = null;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, target.position);
        }
    }
}
