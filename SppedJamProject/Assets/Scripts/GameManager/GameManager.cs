using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<HealthSystem> enemyList;

    private int enemiesDeadCount = 0;
    private bool isGamePaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            enemyList = new List<HealthSystem>();
        }
        else
        {
            Debug.Log("more than one instance");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindAllEnemies();
    }

    private void Update()
    {
        CheckAllEnemiesDead();
    }

    private void CheckAllEnemiesDead()
    {
        if (enemyList.Count == enemiesDeadCount && !isGamePaused)
        {
            isGamePaused = true;
            PauseGame();
        }
    }

    private void FindAllEnemies()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemyObjects)
        {
            HealthSystem enemyHealth = enemyObject.GetComponent<HealthSystem>();

            if (enemyHealth != null)
            {
                enemyList.Add(enemyHealth);
            }
        }

        //Debug.Log("Number of enemies: " + enemyList.Count);
    }

    public void AddEnemyDeadCount()
    {
        enemiesDeadCount++;
    }

    public int GetEnemyDeadCount()
    {
        return enemiesDeadCount;
    }

    private void PauseGame()
    {
        float gameTimeElapsed = Timer.instance.GetEndTime();

        Time.timeScale = 0f;
        Timer.instance.SaveGameTime(gameTimeElapsed);
        //Debug.Log("Game Paused - All enemies eliminated");
        TimeRecords.instance.DisplayTimeRecords();
    }
}
