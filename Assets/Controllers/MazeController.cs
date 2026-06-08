using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class MazeController : MonoBehaviour
{
    [SerializeField] WorldController worldController;
    [SerializeField] TimeController timeController;
    [SerializeField] Dropdown dropA, dropB;
    int mazeWidth = 31;
    int mazeHeight = 31;

    public Sprite[] mazeFloors;
    public Sprite mazeStart, mazeEnd, mazeWall, mazeVisited, mazeFinalPath, mazeCurrent;

    public Maze[] mazeData;
    GameObject tilesContainer;
    GameObject maze1;
    GameObject maze2;

    public Color colorWall = new Color32(81, 45, 56, 245);
    Color colorFloor = new Color32(255, 233, 243, 245);
    Color colorCurrent = new Color32(178, 112, 146, 230);
    Color colorVisited = new Color32(244, 191, 219, 230);
    Color colorPath = new Color32(135, 186, 171, 230);
    Color colorStart = new Color32(135, 186, 171, 230);
    Color colorEnd = new Color32(255, 233, 243, 230);

    public Font font;

    public bool compare = false;
    bool setup = true;
    public bool Setup { get => setup; set => setup = value; }
    public bool Compare { get => compare; set => compare = value; }
    public int MazeWidth { get => mazeWidth; set => mazeWidth = value; }
    public int MazeHeight { get => mazeHeight; set => mazeHeight = value; }

    void Start()
    {
        mazeData = new Maze[2];
        print("@MazeController/Start - " + mazeData.Length);

        if (!(mazeWidth < 1) && !(mazeHeight! < 1))
        {
            mazeData[0] = new Maze(mazeWidth, mazeHeight);
            mazeData[1] = new Maze(mazeWidth, mazeHeight);
        }
        else
            return;

        GenerateMap(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            print("pressed M");
            Destroy(maze1);
            Destroy(maze2);
            TilesContainer();
            mazeData[0].GenerateTileData();
            mazeData[0].GenerateMaze();
            RenderTiles();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            print("pressed N");
            Destroy(maze1);
            Destroy(maze2);
            TilesContainer();
            mazeData[0].GenerateTileData();
            mazeData[0].GenerateNoWalls();
            RenderTiles();
        }
    }

    void RenderTiles()
    {
        tilesContainer = maze1;
        for (int x = 0; x < mazeData[0].Width; x++)
        {
            for (int y = 0; y < mazeData[0].Height; y++)
            {
                //Creating Tiles
                Tile tile_data = mazeData[0].GetTileAt(x, y);

                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;

                tile_go.transform.position = new Vector3(tilesContainer.transform.position.x + x, tilesContainer.transform.position.y + y, 1);
                tile_go.transform.SetParent(tilesContainer.transform, true);

                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();

                switch (tile_data.Type)
                {
                    case Tile.TileType.Floor:
                        tile_go.GetComponent<SpriteRenderer>().color = colorFloor;
                        break;
                    case Tile.TileType.Wall:
                        tile_go.GetComponent<SpriteRenderer>().color = colorWall;
                        break;
                    case Tile.TileType.Start:
                        tile_go.GetComponent<SpriteRenderer>().color = colorStart;
                        break;
                    case Tile.TileType.End:
                        tile_go.GetComponent<SpriteRenderer>().color = colorEnd;
                        break;
                }

                //Creating Tiles Text (hidden by default)
                if (tile_data.Type != Tile.TileType.Wall)
                {
                    GameObject tile_text = new GameObject();

                    tile_text.name = "Text";
                    tile_text.transform.position = tile_go.transform.position;
                    tile_text.transform.SetParent(tile_go.transform);
                    tile_text.SetActive(false);

                    TextMesh tile_tm = tile_text.AddComponent<TextMesh>();

                    string distance;
                    if (tile_data.Distance == int.MaxValue)
                        distance = "inf";
                    else
                        distance = tile_data.Distance.ToString();

                    tile_go.transform.GetChild(0).GetComponent<TextMesh>().text = "C: " + tile_data.Cost + "\nD: " + distance + "\nR: " + tile_data.RootDistance + "\nM: " + tile_data.ManDistance + "\nV:" + tile_data.Visited;
                    tile_tm.characterSize = 0.1f;
                    tile_tm.anchor = TextAnchor.MiddleCenter;
                    tile_tm.font = font;
                    tile_tm.color = Color.black;

                }

                //Event Listeners for Data changes within the tiles
                OnTileTypeChanged(tile_data, tile_go);

                tile_data.RegisterTileDistanceChangedCallback((tile) => { UpdateTileText(tile, tile_go); });
                tile_data.RegisterTileManDistanceChangedCallback((tile) => { UpdateTileText(tile, tile_go); });
                tile_data.RegisterTileRootDistanceChangedCallback((tile) => { UpdateTileText(tile, tile_go); });
                tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
                tile_data.RegisterTileVisitedChangedCallback((tile) => { OnTileVisitedChanged(tile, tile_go); });
            }
        }

        if (compare)
        {
            tilesContainer = maze2;
            for (int x = 0; x < mazeData[1].Width; x++)
            {
                for (int y = 0; y < mazeData[1].Height; y++)
                {
                    Tile tile_data = mazeData[1].GetTileAt(x, y);

                    GameObject tile_go = new GameObject();
                    tile_go.name = "Tile_" + x + "_" + y;

                    tile_go.transform.position = new Vector3(tilesContainer.transform.position.x + x, tilesContainer.transform.position.y + y, 1);

                    tile_go.transform.SetParent(tilesContainer.transform, true);

                    SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                    tile_sr.color = new Color(234, 172, 139, 1);

                    //Creating Tiles Text (hidden by default)
                    if (tile_data.Type != Tile.TileType.Wall)
                    {
                        GameObject tile_text = new GameObject();

                        tile_text.name = "Text";
                        tile_text.transform.position = tile_go.transform.position;
                        tile_text.transform.SetParent(tile_go.transform);
                        tile_text.SetActive(false);

                        TextMesh tile_tm = tile_text.AddComponent<TextMesh>();

                        string distance;
                        if (tile_data.Distance == int.MaxValue)
                            distance = "inf";
                        else
                            distance = tile_data.Distance.ToString();

                        tile_go.transform.GetChild(0).GetComponent<TextMesh>().text = "C: " + tile_data.Cost + "\nD: " + distance + "\nR: " + tile_data.RootDistance + "\nM: " + tile_data.ManDistance + "\nV:" + tile_data.Visited;
                        tile_tm.characterSize = 0.1f;
                        tile_tm.anchor = TextAnchor.MiddleCenter;
                        tile_tm.font = font;
                        tile_tm.color = Color.black;
                    }

                    OnTileTypeChanged(tile_data, tile_go);

                    tile_data.RegisterTileDistanceChangedCallback((tile) => { UpdateTileText(tile, tile_go); });
                    tile_data.RegisterTileManDistanceChangedCallback((tile) => { UpdateTileText(tile, tile_go); });
                    tile_data.RegisterTileRootDistanceChangedCallback((tile) => { UpdateTileText(tile, tile_go); });
                    tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
                    tile_data.RegisterTileVisitedChangedCallback((tile) => { OnTileVisitedChanged(tile, tile_go); });
                }
            }
        }
        GameObject.FindObjectOfType<WorldController>().CenterCamera();
    }

    void TilesContainer()
    {
        tilesContainer = new GameObject();
        tilesContainer.transform.SetParent(this.transform, true);
        tilesContainer.transform.position = tilesContainer.transform.parent.position;
        tilesContainer.name = "Maze 1";
        MazeSolver solver1 = tilesContainer.AddComponent<MazeSolver>();
        solver1.MazeIndex = 0;
        solver1.Init(this, worldController, timeController, dropA, dropB);
        maze1 = tilesContainer;

        if (compare)
        {
            tilesContainer = new GameObject();
            tilesContainer.transform.SetParent(this.transform, true);
            tilesContainer.transform.position = tilesContainer.transform.parent.position;
            tilesContainer.transform.position = new Vector3(tilesContainer.transform.position.x + mazeWidth + 5, tilesContainer.transform.position.y);
            tilesContainer.name = "Maze 2";
            MazeSolver solver2 = tilesContainer.AddComponent<MazeSolver>();
            solver2.MazeIndex = 1;
            solver2.Init(this, worldController, timeController, dropA, dropB);
            maze2 = tilesContainer;
        }
    }

    public void ResetVisited(Maze mazeData)
    {
        foreach (Tile t in mazeData.tiles)
        {
            t.Visited = false;
            if (t.Type == Tile.TileType.Path || t.Type == Tile.TileType.Current)
                t.Type = Tile.TileType.Floor;
            t.Distance = int.MaxValue;
        }
    }

    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        switch (tile_data.Type)
        {
            case Tile.TileType.Floor:
                tile_go.GetComponent<SpriteRenderer>().sprite = mazeFloors[tile_data.Cost];
                tile_go.GetComponent<SpriteRenderer>().color = colorFloor;
                break;
            case Tile.TileType.Wall:
                tile_go.GetComponent<SpriteRenderer>().sprite = mazeWall;
                tile_go.GetComponent<SpriteRenderer>().color = colorWall;
                break;
            case Tile.TileType.Start:
                tile_go.GetComponent<SpriteRenderer>().sprite = mazeFloors[0];
                tile_go.GetComponent<SpriteRenderer>().color = colorStart;
                break;
            case Tile.TileType.End:
                tile_go.GetComponent<SpriteRenderer>().sprite = mazeFloors[0];
                tile_go.GetComponent<SpriteRenderer>().color = colorEnd;
                break;
            case Tile.TileType.Path:
                tile_go.GetComponent<SpriteRenderer>().sprite = mazeFloors[0];
                tile_go.GetComponent<SpriteRenderer>().color = colorPath;
                break;
            case Tile.TileType.Current:
                tile_go.GetComponent<SpriteRenderer>().sprite = mazeFloors[0];
                tile_go.GetComponent<SpriteRenderer>().color = colorCurrent;
                break;
            default:
                break;
        }
    }

    void OnTileVisitedChanged(Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Visited && tile_data.Type != Tile.TileType.End)
        {
            //tile_go.GetComponent<SpriteRenderer>().sprite = mazeVisited;
            tile_go.GetComponent<SpriteRenderer>().color = colorVisited;
        }
        else if (tile_data.Visited == false && tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = mazeFloors[tile_data.Cost];
            tile_go.GetComponent<SpriteRenderer>().color = colorFloor;
        }

        UpdateTileText(tile_data, tile_go);
    }
    void OnTileDistanceChanged(Tile tile_data, GameObject tile_go)
    {
        UpdateTileText(tile_data, tile_go);
    }

    void UpdateTileText(Tile tile_data, GameObject tile_go)
    {
        if (tile_go.transform.childCount > 0)
        {

            string distance;
            if (tile_data.Distance == int.MaxValue)
                distance = "inf";
            else
                distance = tile_data.Distance.ToString();

            tile_go.transform.GetChild(0).GetComponent<TextMesh>().text = "C: " + tile_data.Cost + "\nD: " + distance + "\nR: " + tile_data.RootDistance + "\nM: " + tile_data.ManDistance + "\nV:" + tile_data.Visited;
        }
    }

    public void GenerateMap(bool type)
    {
        //Destroy maze if already there
        Destroy(maze1);
        Destroy(maze2);

        //Make a container GameObject for tiles
        TilesContainer();

        if (mazeWidth != GameObject.Find("Slider Width").GetComponent<Slider>().value || mazeHeight != GameObject.Find("Slider Height").GetComponent<Slider>().value)
        {
            mazeWidth = (int)GameObject.Find("Slider Width").GetComponent<Slider>().value;
            mazeHeight = (int)GameObject.Find("Slider Height").GetComponent<Slider>().value;

            print("@MazeController/GenerateMap - new size: " + mazeWidth + ", " + mazeHeight);

            mazeData[0] = new Maze(mazeWidth, mazeHeight);
            mazeData[1] = new Maze(mazeWidth, mazeHeight);
        }

        //Generate Tile Data for the Maze
        mazeData[0].GenerateTileData();

        //Check what type of maze is requested and generate it
        if (type)
        {
            mazeData[0].GenerateNoWalls();
            ResetVisited(mazeData[0]);
        }
        else
        {
            mazeData[0].GenerateMaze();
            ResetVisited(mazeData[0]);
        }

        if (compare)
        {
            mazeData[1] = mazeData[0].DeepClone();
            ResetVisited(mazeData[1]);
        }

        RenderTiles();
    }

}
