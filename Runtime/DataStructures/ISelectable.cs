using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public enum SelectionType2D
    {
        Remove = -1,
        Replace = 0,
        Add = 1
    }

    public class ISelectable : IFillable
    {
        public bool IsSelected { get; private set; } = false;
        public List<Vector2Int> Selections
        {
            get
            {
                return _selections;
            }
        }

        private List<Vector2Int> _selections = new List<Vector2Int>();
        private List<Vector2Int> _tempSelection = new List<Vector2Int>();

        public ISelectable(int width, int height) : base(width, height)
        {
        }

        public void RectangleSelect(Vector2Int start, Vector2Int end, SelectionType2D selectionType = 0)
        {
            int x = Mathf.Max(Mathf.Min(start.x, end.x), 0), y = Mathf.Max(Mathf.Min(start.y, end.y), 0);
            int targetX = Mathf.Min(Mathf.Max(start.x, end.x), Width), targetY = Mathf.Min(Mathf.Max(start.y, end.y), Height);

            int startY = y;

            if (selectionType == 0)
            {
                _selections.Clear();
                _tempSelection.Clear();
            }

            while (x <= targetX)
            {
                while (y <= targetY)
                {
                    SelectPosition(x, y, selectionType);
                    y += 1;
                }

                x += 1;
                y = startY;
            }

            IsSelected = _selections.Count > 0;
        }

        public void MagicSelect(Vector2Int startPosition, SelectionType2D selectionType = 0)
        {
            var startColor = GetPixelColor(startPosition);

            if (selectionType == 0)
            {
                _selections.Clear();
                _tempSelection.Clear();
            }

            InternalMagicSelect(startPosition.x, startPosition.y, startColor, selectionType);

            foreach (var selection in _tempSelection)
            {
                SelectPosition(selection.x, selection.y, selectionType);
            }

            _tempSelection.Clear();

            IsSelected = _selections.Count > 0;
        }

        private void InternalMagicSelect(int x, int y, Color startColor, SelectionType2D selectionType = 0)
        {
            var isSelected = _tempSelection.Contains(new Vector2Int(x, y));
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;
            if (isSelected)
                return;
            if (GetPixelColor(x, y) != startColor)
                return;

            SelectPositionTo(x, y, ref _tempSelection, selectionType);

            InternalMagicSelect(x - 1, y, startColor, selectionType);
            InternalMagicSelect(x + 1, y, startColor, selectionType);
            InternalMagicSelect(x, y - 1, startColor, selectionType);
            InternalMagicSelect(x, y + 1, startColor, selectionType);
        }

        private void SelectPosition(Vector2Int position, SelectionType2D selectionType = 0)
        {
            SelectPosition(position.x, position.y, selectionType);
        }

        private void SelectPosition(int x, int y, SelectionType2D selectionType = 0)
        {
            var position = new Vector2Int(x, y);

            if (selectionType >= 0)
            {
                if (!_selections.Contains(position))
                {
                    _selections.Add(position);
                }
            }
            else if (selectionType < 0)
            {
                if (_selections.Contains(position))
                {
                    _selections.Remove(position);
                }
            }
        }

        private void SelectPositionTo(int x, int y, ref List<Vector2Int> selectionList, SelectionType2D selectionType = 0)
        {
            var position = new Vector2Int(x, y);

            if (!selectionList.Contains(position))
            {
                selectionList.Add(position);
            }
        }

        public void Deselect()
        {
            IsSelected = false;
            _selections.Clear();
            _tempSelection.Clear();
        }

        public void FillSelection(Color color)
        {
            foreach (var selection in _selections)
            {
                SetPixelColor(selection, color);
            }
        }
    }
}
