using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public MazeController mazeControl;
    public TimeController timeControl;

    public Scrollbar comparison, tileValues, mapType;
    public Canvas algoA, algoB;
    public Dropdown dropA, dropB;
    public Text lblAlgoA;
    public Button btnRandomize, btnPlay, btnPause, btnReset, btnSpd1, btnSpd2, btnSpd3;

    public bool mapTypeBool, sideOut;

    RectTransform sidebar;

    void Start()
    {
        btnRandomize.onClick.AddListener(() =>
        {
            ReanableFunctions();
            RegenerateMaze();
            timeControl.ResetTimer();
        });

        //Media button controls
        btnPlay.onClick.AddListener(() =>
        {
            timeControl.StartTimer();            
            GameObject.Find("Maze 1").GetComponent<MazeSolver>().SolveMaze();
            GameObject.Find("Maze 2").GetComponent<MazeSolver>().SolveMaze();
        });

        btnPause.onClick.AddListener(() =>
        {
            btnRandomize.interactable = true;
            btnReset.interactable = true;
            timeControl.PauseTimer();
            GameObject.Find("Maze 1").GetComponent<MazeSolver>().PauseSolving();
            GameObject.Find("Maze 2").GetComponent<MazeSolver>().PauseSolving();
        });

        btnReset.onClick.AddListener(() =>
        {
            timeControl.ResetTimer();
            GameObject.Find("Maze 1").GetComponent<MazeSolver>().ResetSolving();
            GameObject.Find("Maze 2").GetComponent<MazeSolver>().ResetSolving();
        });

        sidebar = GameObject.Find("Size Change Sidebar").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (sideOut && sidebar.anchoredPosition.x <  1685)
            sidebar.Translate(new Vector3(0.005f, 0f, 0f) * Camera.main.orthographicSize);
        else if (!sideOut && sidebar.anchoredPosition.x > 1552)
            sidebar.Translate(new Vector3(-0.005f, 0f, 0f) * Camera.main.orthographicSize);
    }
    public void CompareModeChange()
    {
        print("@UIController/CompareModeChange() - value: " + comparison.value);

        if (comparison.value > 0.5f)
        {
            algoB.gameObject.SetActive(true);
            algoA.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 374);
            lblAlgoA.text = "Left Maze Algorithm:";
            mazeControl.Compare = true;
            mazeControl.Setup = true;
            RegenerateMaze();
        }
        else
        {
            algoB.gameObject.SetActive(false);
            algoA.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 758);
            lblAlgoA.text = "Choose Algorithm to test:";
            mazeControl.Compare = false;
            mazeControl.Setup = true;
            RegenerateMaze();
        }
    }

    public void TileValueModeChange()
    {
        if (tileValues.value > 0.5f)
        {
            foreach (Transform t in GameObject.Find("Maze 1").transform)
            {
                if (t.childCount > 0)
                {
                    t.GetChild(0).gameObject.SetActive(true);
                }
            }

            foreach (Transform t in GameObject.Find("Maze 2").transform)
            {
                if (t.childCount > 0)
                {
                    t.GetChild(0).gameObject.SetActive(true);
                }
            }

        }
        else
        {
            foreach (Transform t in GameObject.Find("Maze 1").transform)
            {
                if (t.childCount > 0 && t.name != "Final time")
                {
                    t.GetChild(0).gameObject.SetActive(false);
                }
            }

            foreach (Transform t in GameObject.Find("Maze 2").transform)
            {
                if (t.childCount > 0 && t.name != "Final time")
                {
                    t.GetChild(0).gameObject.SetActive(false);
                }
            }

        }
    }

    public void RegenerateMaze()
    {
        tileValues.value = 0f;

        if (mapType.value > 0.5f)
        {
            Debug.Log("@UIController/RegenerateMaze() - TERRAIN");
            mazeControl.GenerateMap(true);
        }
        else
        {
            Debug.Log("@UIController/RegenerateMaze() - MAZE");
            mazeControl.GenerateMap(false);
        }
    }

    public void DisableFunctions()
    {        
        comparison.interactable = false;
        tileValues.interactable = false;
        mapType.interactable = false;

        btnRandomize.interactable = false;
        //btnPlay.interactable = false;
        //btnPause.interactable = false;
        btnReset.interactable = false;
        //btnSpd1.interactable = false;
        //btnSpd2.interactable = false;
        //btnSpd3.interactable = false;

        dropA.interactable = false;
        dropB.interactable = false;        
    }

    public void ReanableFunctions()
    {
        comparison.interactable = true;
        tileValues.interactable = true;
        mapType.interactable = true;

        btnRandomize.interactable = true;
        btnPlay.interactable = true;
        btnPause.interactable = true;
        btnReset.interactable = true;
        btnSpd1.interactable = true;
        btnSpd2.interactable = true;
        btnSpd3.interactable = true;

        dropA.interactable = true;
        dropB.interactable = true;
    }

    public void SideBarControl()
    {
        sideOut = !sideOut;
    }

    public void SliderValueCheck()
    {
        int heightValue, widthValue;

        widthValue = (int) GameObject.Find("Slider Width").GetComponent<Slider>().value;
        heightValue = (int) GameObject.Find("Slider Height").GetComponent<Slider>().value;

        //print("@UIController/SliderValueCheck - first value: width = " + widthValue + " height = " + heightValue);

        if (widthValue % 2 == 0)
        {
            widthValue += 1;
            GameObject.Find("Slider Width").GetComponent<Slider>().value = widthValue;
        }
        else if(heightValue % 2 == 0)
        {
            heightValue += 1;
            GameObject.Find("Slider Height").GetComponent<Slider>().value = heightValue;
        }
        //print("@UIController/SliderValueCheck - second value: width = " + widthValue + " height = " + heightValue);

        GameObject.Find("lblSliderWidth").GetComponent<Text>().text = "Width: " + widthValue;
        GameObject.Find("lblSliderHeight").GetComponent<Text>().text = "Height: " + heightValue;
    }
}
