using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeController : MonoBehaviour
{
    float time = 0f;
    public bool running = false;
    public Text timeDisplay;
    public Button spd1, spd2, spd3;
    public int SpeedIndex => speedIndex;
    int speedIndex;

    void Start()
    {
        speedIndex = 1;

        spd1.onClick.AddListener(() =>
        {
            SpeedChange(1f);
        });
        spd2.onClick.AddListener(() =>
        {
            SpeedChange(5f);
        });
        spd3.onClick.AddListener(() =>
        {
            SpeedChange(10f);
        });
    }

    void Update()
    {
        if (running)
        {
            time += (Time.deltaTime * speedIndex);
            DisplayTime(time);
        }
    }

    public void StartTimer()
    {
        if (!GameObject.FindObjectOfType<WorldController>().mazeRunning1 && !GameObject.FindObjectOfType<WorldController>().mazeRunning2)
        {
            ResetTimer();
        }
        running = true;
    }

    public void PauseTimer()
    {
        running = false;
        WorldController wc = GameObject.FindObjectOfType<WorldController>();
        wc.mazeRunning1 = wc.mazeRunning2 = false;
    }

    public void ResetTimer()
    {
        time = 0f;
        DisplayTime(time);

        try
        {
            Destroy(GameObject.Find("Maze 1").transform.Find("Final time").gameObject);
            Destroy(GameObject.Find("Maze 2").transform.Find("Final time").gameObject);
        }
        catch { }
    }

    void DisplayTime(float time)
    {
        float minute = Mathf.FloorToInt(time / 60);
        float second = Mathf.FloorToInt(time % 60);
        float milisecond = Mathf.FloorToInt(time % 1 * 100);

        timeDisplay.text = string.Format("{0:00}:{1:00}.{2:00}", minute, second, milisecond);
    }

    void SpeedChange(float speedValue)
    {
        speedIndex = Mathf.FloorToInt(speedValue);
    }
}
