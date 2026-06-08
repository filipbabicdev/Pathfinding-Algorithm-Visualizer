using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Maze
{
    public Tile[,] tiles;
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Maze(int width = 31, int height = 31)
    {
        this.Width = width;
        this.Height = height;
            }

    public Tile GetTileAt(int x, int y)
    {
        if (x >= Width || x < 0 || y >= Height || y < 0)
        {
            Debug.Log("Tile is out of range.");

            return null;
        }
        if (tiles[x, y] == null)
            tiles[x, y] = new Tile(x, y);

        return tiles[x, y];
    }

    public Tile GenerateMazeStartEnd()
    {
        Tile tile_data;
        int index;

        index = UnityEngine.Random.Range(1, Height);
        if (index % 2 == 0)
            index--;

        tile_data = GetTileAt(Width - 1, index);

        tile_data.Type = Tile.TileType.End;
        tile_data.Cost = 0;
        tile_data.End = true;

        index = UnityEngine.Random.Range(1, Height);
        if (index % 2 == 0)
            index--;

        tile_data = GetTileAt(0, index);

        tile_data.Type = Tile.TileType.Start;
        tile_data.Cost = 0;
        tile_data.Start = true;

        return tile_data;
    }

    public void GenerateTileData()
    {
        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = new Tile(x, y);
            }
        }
    }

    public void GenerateNoWalls()
    {
        GenerateMazeStartEnd();

        foreach (Tile t in tiles)
        {
            //Check X coordinate
            if (t.X == 0 || t.X == Width - 1)
                continue;

            //check Y coordinate
            if (t.Y == 0 || t.Y == Height - 1)
                continue;

            t.Type = Tile.TileType.Floor;
            t.Cost = UnityEngine.Random.Range(0, 8);
        }
    }

    public void GenerateMaze(Tile startTile = null)
    {
        //Desegnate maze Start and End nodes if there are none 
        if (startTile == null)
            startTile = GetTileAt(1, GenerateMazeStartEnd().Y);

        //Generate a maze path using DFS algorithm
        Stack<Tile> pathStack = new Stack<Tile>();
        pathStack.Push(startTile);

        startTile.Visited = true;
        startTile.Type = Tile.TileType.Floor;

        while (pathStack.Count > 0)
        {
            Tile currentTile = pathStack.Pop();

            List<Tile> tileNeighbours = FindNeighbours(currentTile);

            currentTile.Type = Tile.TileType.Floor;

            if (GetTileAt(currentTile.X, currentTile.Y).Type == Tile.TileType.End)
                continue;

            if (tileNeighbours.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, tileNeighbours.Count);

                pathStack.Push(currentTile);

                Tile temp = GetTileAt((tileNeighbours[index].X + currentTile.X) / 2, (tileNeighbours[index].Y + currentTile.Y) / 2);

                temp.Cost = 0;
                temp.Type = Tile.TileType.Floor;

                tileNeighbours[index].Visited = true;
                pathStack.Push(tileNeighbours[index]);
            }
        }
    }

    public List<Tile> FindNeighbours(Tile currentTile, int jump = 2)
    {
        List<Tile> result = new List<Tile>();

        int x = currentTile.X;
        int y = currentTile.Y;

        Tile currentNeighbour;

        for (int i = -jump; i <= jump; i += jump * 2)
        {

            if (x + i < 0 || x + i > Width - 1)
                continue;

            currentNeighbour = GetTileAt(x + i, y);
            if (currentNeighbour.Visited)
                continue;

            result.Add(currentNeighbour);
        }

        for (int j = -jump; j <= jump; j += jump * 2)
        {

            if (y + j < 0 || y + j > Height - 1)
                continue;

            currentNeighbour = GetTileAt(x, y + j);
            if (currentNeighbour.Visited)
                continue;

            result.Add(currentNeighbour);
        }

        return result;
    }

    public Maze DeepClone()
    {
        Maze copy = new Maze(Width, Height);
        copy.tiles = new Tile[Width, Height];

        foreach (Tile t in this.tiles)
        {
            copy.tiles[t.X, t.Y] = t.DeepClone();
        }

        return copy;
    }
}