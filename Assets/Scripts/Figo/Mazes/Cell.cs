using UnityEngine;

namespace Figo.Mazes
{
    public class Cell
    {
        private int _x, _y;
        private bool _visited = false;
        private bool _wallLeft = true, _wallBottom = true;
        private bool _isExit = false;
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

        public bool WallLeft
        {
            get => _wallLeft;
            set => _wallLeft = value;
        }

        public bool WallBottom
        {
            get => _wallBottom;
            set => _wallBottom = value;
        }

        public bool IsExit
        {
            get => _isExit;
            set => _isExit = value;
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
}