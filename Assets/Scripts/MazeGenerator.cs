using System.Collections.Generic;
using Shared;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private int _width = 20;
    [SerializeField] private int _height = 20;
    [SerializeField] private float _tileSize = 6f;

    private Cell[,] grid;

    class Cell
    {
        private int _x, _y;
        private bool _visited = false;
        // up, right, down, left
        private bool[] _walls = { true, true, true, true };
        private GameObject _floorObj;

        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public bool Visited
        {
            get => _visited;
            set => _visited = value;
        }

        public bool[] Walls
        {
            get => _walls;
        }

        public GameObject Floor
        {
            get => _floorObj;
            set => _floorObj = value;
        }

        public Cell(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }

    private void Start()
    {
        GenerateMaze();
        BuildMazeWithTiles();
        PlaceObjects();
    }

    private void BuildMazeWithTiles()
    {
        for (int x = 0; x <= _width; x++)
        {
            for (int y = 0; y <= _height; y++)
            {
                Vector3 pos = new(x * _tileSize, 0, y * _tileSize);

                GameObject tileObj = AssetManager.SpawnTile(pos);

                if (x == _width || y == _height)
                {
                    continue;
                }

                Cell cell = grid[x, y];
                cell.Floor = tileObj; // pour placer les objets dessus

                // Références aux murs dans le prefab
                Transform wallLeft = tileObj.transform.Find("WallLeft");
                Transform wallBottom = tileObj.transform.Find("WallBottom");

                // Désactivation selon walls[]
                // mur gauche
                if (!cell.Walls[3] && wallLeft != null)
                    wallLeft.gameObject.SetActive(false);

                // mur bas
                if (!cell.Walls[2] && wallBottom != null)
                    wallBottom.gameObject.SetActive(false);
            }
        }
    }

    private void GenerateMaze()
    {
        grid = new Cell[_width, _height];

        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                grid[x, y] = new Cell(x, y);

        Stack<Cell> stack = new();
        Cell current = grid[0, 0];
        current.Visited = true;

        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Peek();
            Cell next = GetUnvisitedNeighbor(current);

            if (next != null)
            {
                RemoveWalls(current, next);
                next.Visited = true;
                stack.Push(next);
            }
            else
            {
                stack.Pop();
            }
        }
    }

    private List<Cell> GetDeadEnds()
    {
        List<Cell> deadEnds = new();

        foreach (var cell in grid)
        {
            int openSides = 0;
            foreach (bool wall in cell.Walls)
                if (!wall) openSides++;

            if (openSides == 1)
                deadEnds.Add(cell);
        }

        return deadEnds;
    }

    private void PlaceObjects()
    {
        List<Cell> deadEnds = GetDeadEnds();

        if (deadEnds.Count > 0)
        {
            Cell keyCell = deadEnds[Random.Range(0, deadEnds.Count)];
            AssetManager.SpawnKey(keyCell.Floor.transform.position + Vector3.up);
        }

        foreach (var cell in grid)
        {
            if (!deadEnds.Contains(cell) && Random.value < 0.05f)
            {
                AssetManager.SpawnBonus(cell.Floor.transform.position + Vector3.up);
            }
        }
    }

    private Cell GetUnvisitedNeighbor(Cell cell)
    {
        List<Cell> neighbors = new();

        // Haut
        if (cell.Y + 1 < _height && !grid[cell.X, cell.Y + 1].Visited)
            neighbors.Add(grid[cell.X, cell.Y + 1]);

        // Bas
        if (cell.Y - 1 >= 0 && !grid[cell.X, cell.Y - 1].Visited)
            neighbors.Add(grid[cell.X, cell.Y - 1]);

        // Droite
        if (cell.X + 1 < _width && !grid[cell.X + 1, cell.Y].Visited)
            neighbors.Add(grid[cell.X + 1, cell.Y]);

        // Gauche
        if (cell.X - 1 >= 0 && !grid[cell.X - 1, cell.Y].Visited)
            neighbors.Add(grid[cell.X - 1, cell.Y]);

        if (neighbors.Count > 0)
            return neighbors[Random.Range(0, neighbors.Count)]; // choix aléatoire

        return null; // aucun voisin
    }

    private void RemoveWalls(Cell a, Cell b)
    {
        int dx = b.X - a.X;
        int dy = b.Y - a.Y;

        // b est à droite de a
        if (dx == 1 && dy == 0)
        {
            a.Walls[1] = false; // droite de a
            b.Walls[3] = false; // gauche de b
        }
        // b est à gauche de a
        else if (dx == -1 && dy == 0)
        {
            a.Walls[3] = false; // gauche de a
            b.Walls[1] = false; // droite de b
        }
        // b est en haut de a
        else if (dx == 0 && dy == 1)
        {
            a.Walls[0] = false; // haut de a
            b.Walls[2] = false; // bas de b
        }
        // b est en bas de a
        else if (dx == 0 && dy == -1)
        {
            a.Walls[2] = false; // bas de a
            b.Walls[0] = false; // haut de b
        }
    }
}