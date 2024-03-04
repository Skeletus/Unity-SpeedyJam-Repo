using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeRecords : MonoBehaviour
{
    public static TimeRecords instance;

    [SerializeField] private TextMeshProUGUI recordsText;
    [SerializeField] private GameObject leaderBoard;

    private List<float> savedTimes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            savedTimes = new List<float>();
        }
        else
        {
            Debug.Log("more than one instance");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        leaderBoard.SetActive(false);
    }

    public void DisplayTimeRecords()
    {
        leaderBoard.SetActive(true);
        int recordCount = PlayerPrefs.GetInt("GameplayTimeCount", 0);

        for (int i = 1; i <= recordCount; i++)
        {
            float savedTime = PlayerPrefs.GetFloat("GameplayTime_" + i, 0f);
            savedTimes.Add(savedTime);
        }

        // sort minor to mayor
        savedTimes.Sort();

        // show sorted time record
        recordsText.text = "Time Records:\n";

        for (int i = 0; i < savedTimes.Count; i++)
        {
            int rank = i + 1;
            string timeString = FormatTime(savedTimes[i]);
            recordsText.text += rank + ". " + timeString + "\n";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
