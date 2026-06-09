using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    Vector3 lastMousePosition;

    public MazeController maze;
    public TimeController time;
    public UIController ui;

    [NonSerialized]
    public bool mazeRunning1 = false;
    [NonSerialized]
    public bool mazeRunning2 = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Starting and stopping timer if a maze is being solved
        if (mazeRunning1 || mazeRunning2)
        {
            time.StartTimer();
            ui.DisableFunctions();
        }

        if (!mazeRunning1 && !mazeRunning2)
        {
            time.PauseTimer();
            ui.ReanableFunctions();
        }



        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Camera Panning
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 difference = lastMousePosition - currentMousePosition;
            Camera.main.transform.Translate(difference);
        }

        //Camera Zoom
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (Camera.main.orthographicSize > 1)
                Camera.main.orthographicSize--;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            Camera.main.orthographicSize++;
        }

        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log("DEBUG: Mouse Position - " + lastMousePosition);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CenterCamera();
        }
    }

    public void CenterCamera()
    {
        if (!maze.compare)
        {
            Camera.main.transform.position = new Vector3(maze.MazeWidth / 2, maze.MazeHeight / 2 + maze.MazeHeight / 11 - 1, 1);
            if(maze.MazeWidth > maze.MazeHeight)
                Camera.main.orthographicSize = maze.MazeWidth / 2 + maze.MazeWidth / 5;
            else
                Camera.main.orthographicSize = (maze.MazeHeight + 3) / 2 + maze.MazeHeight / 5;
        }
        else
        {
            Camera.main.transform.position = new Vector3(maze.MazeWidth + 2, maze.MazeHeight / 2 + maze.MazeHeight / 11 -1, 1);
            if (maze.MazeWidth > maze.MazeHeight)
                Camera.main.orthographicSize = maze.MazeWidth / 2 + maze.MazeWidth / 5;
            else
                Camera.main.orthographicSize = (maze.MazeHeight + 3) / 2 + maze.MazeHeight / 5;
        }
    }
}
