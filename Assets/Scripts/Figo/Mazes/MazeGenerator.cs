using System.Collections.Generic;
using Shared;
using Unity.AI.Navigation;
using UnityEngine;
using Game;

namespace Figo.Mazes
{
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] private OverlapWFC overlapWfc;
        [SerializeField] private SecondPassWFC secondPassWfc;
        [SerializeField] private PlayerInteract playerInteract;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private EnemyAgent enemyAgent;

        [SerializeField] private Transform tileParent;

        [SerializeField] private int _width = 20;
        [SerializeField] private int _height = 20;
        [SerializeField] private float _tileSize = 6f;

        private Cell[,] grid;

        private void Start()
        {
            Regenerate();
        }

        public void Regenerate()
        {
            EventEmitter.Instance.Emit(GameEvent.OnRegenerateStart);

            foreach (Transform child in tileParent)
            {
                Destroy(child.gameObject);
            }

            // GenerateMaze();
            GenerateMazeFromWFC();

            BuildMazeWithTiles();

            navMeshSurface.BuildNavMesh();

            PlaceObjects();

            // enemyAgent.Setup();
            playerInteract.Setup();

            EventEmitter.Instance.Emit(GameEvent.OnRegenerateEnd);
        }

        private void BuildMazeWithTiles()
        {
            for (int y = 0; y <= _height; y++)
            {
                for (int x = 0; x <= _width; x++)
                {
                    Vector3 pos = new(x * _tileSize, 0, y * _tileSize);

                    GameObject tileObj = AssetManager.SpawnTile(pos, tileParent);

                    if (x == _width || y == _height)
                    {
                        if (y == _height)
                        {
                            tileObj.transform.Find("WallLeft")
                                .gameObject.SetActive(false);
                        }

                        if (x == _width)
                        {
                            tileObj.transform.Find("WallBottom")
                                .gameObject.SetActive(false);
                        }

                        if (x == _width && y == _height - 1)
                        {
                            tileObj.transform.Find("WallLeft")
                                .gameObject.SetActive(false);
                            tileObj.transform.Find("ExitLeft")
                                .gameObject.SetActive(true);
                        }

                        continue;
                    }

                    Cell cell = grid[x, y];
                    cell.Floor = tileObj; // pour placer les objets dessus
                    var tile = cell.Floor.GetComponentInChildren<LightController>();
                    if (tile)
                    {
                        if (x == 0 || y == 0 || x == _width - 1 || y == _height - 1)
                        {
                            tile.LightOnDuration = Random.Range(80f, 100f);
                            tile.LightOffDuration = Random.Range(10f, 15f);
                        }
                        else
                        {
                            tile.LightOnDuration = Random.Range(5f, 10f);
                            tile.LightOffDuration = Random.Range(2f, 4f);
                        }
                    }

                    // Références aux murs dans le prefab
                    Transform wallLeft = tileObj.transform.Find("WallLeft");
                    Transform wallBottom = tileObj.transform.Find("WallBottom");

                    // Désactivation selon walls[]
                    // mur gauche
                    if (!cell.WallLeft && wallLeft != null)
                        wallLeft.gameObject.SetActive(false);

                    // mur bas
                    if (!cell.WallBottom && wallBottom != null)
                        wallBottom.gameObject.SetActive(false);
                }
            }
        }

        private void GenerateMazeFromWFC()
        {
            overlapWfc.Generate();
            overlapWfc.Run();
            grid = secondPassWfc.FixMaze();
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
                Cell next = MazeUtils.GetUnvisitedNeighbor(current, _width, _height, grid);

                if (next != null)
                {
                    MazeUtils.RemoveWalls(current, next);
                    next.Visited = true;
                    stack.Push(next);
                }
                else
                {
                    stack.Pop();
                }
            }
        }

        private void PlaceObjects()
        {
            List<Cell> deadEnds = MazeUtils.GetDeadEnds(grid, _width, _height);

            print("DeadEnds: " + deadEnds.Count.ToString());
            if (deadEnds.Count > 0)
            {
                if (deadEnds.Count >= 3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int keyIndex = Random.Range(0, deadEnds.Count);
                        Cell keyCell = deadEnds[keyIndex];
                        AssetManager.SpawnKey(
                            keyCell.Floor.transform.position + Vector3.up, tileParent);
                        deadEnds.RemoveAt(keyIndex);

                        var tile = keyCell.Floor.GetComponentInChildren<LightController>();
                        if (tile)
                        {
                            tile.LightOnDuration = Random.Range(80f, 100f);
                            tile.LightOffDuration = Random.Range(10f, 15f);
                        }
                    }
                }

                foreach (var cell in deadEnds)
                {
                    var tile = cell.Floor.GetComponentInChildren<LightController>();
                    if (tile)
                    {
                        tile.LightOnDuration = Random.Range(40f, 50f);
                        tile.LightOffDuration = Random.Range(5f, 10f);
                    }
                }
            }

            foreach (var cell in grid)
            {
                if (deadEnds.Contains(cell) && Random.value < 0.75f)
                {
                    AssetManager.SpawnBonus(
                        cell.Floor.transform.position + Vector3.up, tileParent);
                }
            }
        }
    }
}