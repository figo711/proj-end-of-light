using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Figo.Mazes
{
    [ExecuteInEditMode]
    public class SecondPassWFC : MonoBehaviour
    {
        [SerializeField] private OverlapWFC _overlapWfc;
        [SerializeField] private TilePlaceholder _emptyTile;

        public OverlapWFC OverlapWFC_Ref => _overlapWfc;

        private Cell[,] grid;

        public Cell[,] FixMaze()
        {
            var _width = _overlapWfc.width;
            var _height = _overlapWfc.depth;

            grid = new Cell[_width, _height];

            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                {
                    var cell = new Cell(x, y);
                    var tile = _overlapWfc.rendering[x, y];
                    print($"{x} {y} {(tile != null ? tile.name : "null")}");
                    if (tile == null)
                    {
                        var tileObj = Instantiate(_emptyTile, new Vector3(), Quaternion.identity);
                        Vector3 fscale = tileObj.transform.localScale;
                        tileObj.transform.parent = _overlapWfc.GetGroup();
                        tileObj.transform.localPosition = new Vector3(x * _overlapWfc.gridsize, y * _overlapWfc.gridsize, 0f);
                        tileObj.transform.localEulerAngles = new Vector3(0, 0, 360);
                        tileObj.transform.localScale = fscale;
                        _overlapWfc.rendering[x, y] = tileObj.gameObject;

                        cell.WallBottom = false;
                        cell.WallLeft = false;
                    }
                    else
                    {
                        var tp = tile.GetComponent<TilePlaceholder>();
                        cell.WallBottom = tp.HasBottom;
                        cell.WallLeft = tp.HasLeft;
                    }
                    grid[x, y] = cell;

                    if (y == 0)
                    {
                        grid[x, y].WallBottom = true;
                    }
                    if (x == 0)
                    {
                        grid[x, y].WallLeft = true;
                    }
                    /*                     if (x + 1 == _width)
                                        {
                                            grid[x + 1, y].WallLeft = true;
                                        }
                                        if (y + 1 == _height)
                                        {
                                            grid[x, y + 1].WallBottom = true;
                                        } */
                }

            Stack<Cell> stack = new();
            List<Cell> isolatedDeadEnds = new();
            Cell start = grid[0, 0];
            start.Visited = true;

            stack.Push(start);

            while (stack.Count > 0 || isolatedDeadEnds.Count > 0)
            {
                Cell current;

                if (stack.Count > 0)
                    current = stack.Peek();
                else
                {
                    current = isolatedDeadEnds[Random.Range(0, isolatedDeadEnds.Count)];
                    isolatedDeadEnds.Remove(current);

                    // Débloquer l'impasse
                    var breakableNeighbors = MazeUtils.GetBreakableNeighbors(current, _width - 1, _height - 1, grid);
                    if (breakableNeighbors.Count > 0)
                    {
                        Cell neighbor = breakableNeighbors[Random.Range(0, breakableNeighbors.Count)];
                        MazeUtils.RemoveWalls(current, neighbor);
                        neighbor.Visited = true;
                        stack.Push(current); // continuer depuis cette cellule
                        stack.Push(neighbor);
                        continue; // passe au prochain tour
                    }
                    else
                    {
                        // Si aucun voisin pour débloquer, on ignore cette impasse
                        continue;
                    }
                }

                var neighbors = MazeUtils.GetAvailableNeighbor(current, _width - 1, _height - 1, grid);

                if (neighbors.Count > 0)
                {
                    Cell next = neighbors[Random.Range(0, neighbors.Count)];
                    MazeUtils.RemoveWalls(current, next);
                    next.Visited = true;
                    stack.Push(next);
                }
                else
                {
                    // Aucun voisin accessible → cellule isolée
                    if (!isolatedDeadEnds.Contains(current))
                        isolatedDeadEnds.Add(current);
                    stack.Pop();
                }
            }

            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                {
                    var tile = _overlapWfc.rendering[x, y];
                    var tp = tile.GetComponent<TilePlaceholder>();
                    if (x < _width && y < _height)
                    {
                        tp.SetLeftVisibility(grid[x, y].WallLeft);
                        tp.SetBottomVisibility(grid[x, y].WallBottom);
                    }
                    if (x + 1 == _width)
                    {
                        tp.SetLeftVisibility(true);
                        tp.SetBottomVisibility(false);
                    }
                    if (y + 1 == _height)
                    {
                        tp.SetLeftVisibility(false);
                        tp.SetBottomVisibility(true);
                    }
                    if (y + 1 == _height && x + 1 == _width)
                    {
                        tp.SetLeftVisibility(false);
                        tp.SetBottomVisibility(false);
                    }
                }

            print("Done Fix !");

            return grid;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SecondPassWFC))]
    public class WFCSecondPassEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SecondPassWFC secondPassWFC = (SecondPassWFC)target;
            if (secondPassWFC.OverlapWFC_Ref != null && secondPassWFC.OverlapWFC_Ref.training != null)
            {
                if (GUILayout.Button("second pass fix"))
                {
                    secondPassWFC.FixMaze();
                }
            }
            DrawDefaultInspector();
        }
    }
#endif
}