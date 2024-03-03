using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float health;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        //Debug.Log("Current health: " + health);
        if (health <= 0)
        {
            health = 0;
            //Debug.Log("dead: " + health);
            Die();
        }

    }

    private void Die()
    {
        if (this.tag == "Player")
        {
            this.gameObject.SetActive(false);
        }
        if (this.tag == "Enemy")
        {
            GameManager.instance.AddEnemyDeadCount();
            Debug.Log("enemy dead counter: " + GameManager.instance.GetEnemyDeadCount());
            Destroy(gameObject);
        }
    }
}
