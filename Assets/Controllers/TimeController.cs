using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeController : MonoBehaviour
{
    [SerializeField] MazeController mazeControl;
    [SerializeField] WorldController worldControl;
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
        if (!worldControl.mazeRunning1 && !worldControl.mazeRunning2)
            ResetTimer();
        running = true;
    }

    public void PauseTimer()
    {
        running = false;
        worldControl.mazeRunning1 = worldControl.mazeRunning2 = false;
    }

    public void ResetTimer()
    {
        time = 0f;
        DisplayTime(time);
        ClearFinalTime(mazeControl.Solver1);
        ClearFinalTime(mazeControl.Solver2);
    }
    void ClearFinalTime(MazeSolver solver)
    {
        if (solver == null) return;
        Transform ft = solver.transform.Find("Final Time");
        if (ft != null)
            Destroy(ft.gameObject);
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
