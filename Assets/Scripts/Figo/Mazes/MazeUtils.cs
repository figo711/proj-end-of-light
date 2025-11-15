using System.Collections.Generic;
using UnityEngine;

namespace Figo.Mazes
{
    public static class MazeUtils
    {

        public static List<Cell> GetAvailableNeighbor(Cell cell, int width, int height, Cell[,] grid)
        {
            List<Cell> neighbors = new();

            // Haut
            if (cell.Y + 1 < height && !grid[cell.X, cell.Y + 1].WallBottom && !grid[cell.X, cell.Y + 1].Visited)
                neighbors.Add(grid[cell.X, cell.Y + 1]);

            // Bas
            if (cell.Y - 1 >= 0 && !grid[cell.X, cell.Y].WallBottom && !grid[cell.X, cell.Y - 1].Visited)
                neighbors.Add(grid[cell.X, cell.Y - 1]);

            // Droite
            if (cell.X + 1 < width && !grid[cell.X + 1, cell.Y].WallLeft && !grid[cell.X + 1, cell.Y].Visited)
                neighbors.Add(grid[cell.X + 1, cell.Y]);

            // Gauche
            if (cell.X - 1 >= 0 && !grid[cell.X, cell.Y].WallLeft && !grid[cell.X - 1, cell.Y].Visited)
                neighbors.Add(grid[cell.X - 1, cell.Y]);

            return neighbors;
        }

        public static Cell GetUnvisitedNeighbor(Cell cell, int width, int height, Cell[,] grid)
        {
            List<Cell> neighbors = new();

            // Haut
            if (cell.Y + 1 < height && !grid[cell.X, cell.Y + 1].Visited)
                neighbors.Add(grid[cell.X, cell.Y + 1]);

            // Bas
            if (cell.Y - 1 >= 0 && !grid[cell.X, cell.Y - 1].Visited)
                neighbors.Add(grid[cell.X, cell.Y - 1]);

            // Droite
            if (cell.X + 1 < width && !grid[cell.X + 1, cell.Y].Visited)
                neighbors.Add(grid[cell.X + 1, cell.Y]);

            // Gauche
            if (cell.X - 1 >= 0 && !grid[cell.X - 1, cell.Y].Visited)
                neighbors.Add(grid[cell.X - 1, cell.Y]);

            if (neighbors.Count > 0)
                return neighbors[Random.Range(0, neighbors.Count)]; // choix aléatoire

            return null; // aucun voisin
        }

        public static List<Cell> GetBreakableNeighbors(Cell cell, int width, int height, Cell[,] grid)
        {
            List<Cell> neighbors = new();
            int x = cell.X;
            int y = cell.Y;

            // Gauche
            if (x > 0 && !grid[x - 1, y].Visited) neighbors.Add(grid[x - 1, y]);
            // Droite
            if (x < width - 1 && !grid[x + 1, y].Visited) neighbors.Add(grid[x + 1, y]);
            // Bas
            if (y > 0 && !grid[x, y - 1].Visited) neighbors.Add(grid[x, y - 1]);
            // Haut
            if (y < height - 1 && !grid[x, y + 1].Visited) neighbors.Add(grid[x, y + 1]);

            return neighbors;
        }


        public static void RemoveWalls(Cell a, Cell b)
        {
            int dx = b.X - a.X;
            int dy = b.Y - a.Y;

            // b est à droite de a
            if (dx == 1 && dy == 0)
            {
                b.WallLeft = false;
            }
            // b est à gauche de a
            else if (dx == -1 && dy == 0)
            {
                a.WallLeft = false;
            }
            // b est en haut de a
            else if (dx == 0 && dy == 1)
            {
                b.WallBottom = false;
            }
            // b est en bas de a
            else if (dx == 0 && dy == -1)
            {
                a.WallBottom = false;
            }
        }

        public static List<Cell> GetDeadEnds(Cell[,] grid, int width, int height)
        {
            List<Cell> deadEnds = new();

            foreach (var cell in grid)
            {
                if (cell.Floor == null) // safe check
                    continue;

                if (cell.X == 0 && cell.Y == 0)
                    continue; // éviter clé au départ

                int openSides = 0;

                // Gauche
                if (cell.X > 0 && !cell.WallLeft)
                    openSides++;

                // Droite (dépend du voisin)
                if (cell.X < width - 1 && !grid[cell.X + 1, cell.Y].WallLeft)
                    openSides++;

                // Bas
                if (cell.Y > 0 && !cell.WallBottom)
                    openSides++;

                // Haut (dépend du voisin)
                if (cell.Y < height - 1 && !grid[cell.X, cell.Y + 1].WallBottom)
                    openSides++;

                if (openSides == 1)
                    deadEnds.Add(cell);
            }

            return deadEnds;
        }
    }
}