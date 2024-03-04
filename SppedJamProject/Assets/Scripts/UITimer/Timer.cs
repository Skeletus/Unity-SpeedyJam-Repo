using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    [SerializeField] private TextMeshProUGUI timerText;
    private float startTime;
    private float endTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("more than one instance");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startTime = Time.time;
        PlayerPrefs.DeleteKey("GameplayTimeCount");
    }

    private void Update()
    {
        float elapsedTime = Time.time - startTime;
        endTime = elapsedTime;
        DisplayTime(elapsedTime);
    }

    private void DisplayTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

        string timeString = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        timerText.text = "Time: " + timeString;
    }

    public void SaveGameTime(float gameTimeElapsed)
    {
        // get actual number of records and add +1
        int currentRecordCount = PlayerPrefs.GetInt("GameplayTimeCount", 0);
        currentRecordCount++;

        Debug.Log("currentRecordCount: " + currentRecordCount);
        // save new record and time elapsed
        PlayerPrefs.SetInt("GameplayTimeCount", currentRecordCount);
        PlayerPrefs.SetFloat("GameplayTime_" + currentRecordCount, gameTimeElapsed);
        PlayerPrefs.Save();
    }

    public float GetEndTime()
    {
        return endTime;
    }
}
