using ArashGh.Pixelator.Runtime.Utils;
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
        public bool IsSelected
        {
            get
            {
                return _selections.Count > 0;
            }
        }

        public List<Vector2Int> Selections
        {
            get
            {
                return _selections;
            }
        }

        private Layer _selectionLayer;
        private List<Vector2Int> _selections = new List<Vector2Int>();
        private List<Vector2Int> _tempSelection = new List<Vector2Int>();

        public ISelectable(int width, int height, IDrawable parent) : base(width, height, parent)
        {
            if (parent != null)
                _selectionLayer = new Layer("Selection", width, height);
        }

        public Layer GetSelection()
        {
            return _selectionLayer;
        }

        public void ApplySelection()
        {
            if (_selectionLayer == null)
                return;
            if (_selections.Count == 0)
                return;

            foreach (var selection in _selections)
            {
                _selectionLayer.SetPixelColor(selection, GetPixelColor(selection));
                SetPixelColor(selection, new Color32(0, 0, 0, 0));
            }

            _overlay = _selectionLayer;
        }

        public void RectangleSelect(Vector2Int start, Vector2Int end, SelectionType2D selectionType = 0)
        {
            int x = Mathf.Max(Mathf.Min(start.x, end.x), 0), y = Mathf.Max(Mathf.Min(start.y, end.y), 0);
            int targetX = Mathf.Min(Mathf.Max(start.x, end.x), Width), targetY = Mathf.Min(Mathf.Max(start.y, end.y), Height);

            int startY = y;

            _tempSelection.Clear();
            if (selectionType == 0)
            {
                _selections.Clear();
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
        }

        public void MagicSelect(Vector2Int startPosition, SelectionType2D selectionType = 0)
        {
            var startColor = GetPixelColor(startPosition);

            _tempSelection.Clear();
            if (selectionType == 0)
            {
                _selections.Clear();
            }

            InternalMagicSelect(startPosition.x, startPosition.y, startColor, selectionType);

            foreach (var selection in _tempSelection)
            {
                SelectPosition(selection.x, selection.y, selectionType);
            }

            _tempSelection.Clear();
        }

        private void InternalMagicSelect(int x, int y, Color32 startColor, SelectionType2D selectionType = 0)
        {
            var position = new Vector2Int(x, y);
            var isSelected = _tempSelection.Contains(position);

            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;
            if (isSelected)
                return;
            if (!GetPixelColor(x, y).IsEqualTo(startColor))
                return;

            if (!_tempSelection.Contains(position))
            {
                _tempSelection.Add(position);
            }

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

        public void Deselect()
        {
            WriteLayerOnTop(_selectionLayer);

            _selectionLayer.MoveTo(0, 0);
            _selectionLayer.Clear();
            _selections.Clear();
            _tempSelection.Clear();
            _overlay = null;
        }

        public void FillSelection(Color32 color)
        {
            if (!IsSelected)
                return;

            foreach (var selection in _selections)
            {
                _selectionLayer.SetPixelColor(selection, color);
            }
        }

        public void MoveSelection(Vector2Int dPos)
        {
            MoveSelection(dPos.x, dPos.y);
        }

        public void MoveSelection(int x, int y)
        {
            //var tLayer = LayerFromSelection();

            //for (int i = 0; i < _selections.Count; i++)
            //{
            //    var s = _selections[i];
            //    SetPixelColor(new Vector2Int(s.x, s.y), new Color32(0, 0, 0, 0));
            //    _selections[i] = new Vector2Int(s.x + x, s.y + y);
            //}

            //tLayer.Move(x, y);
            if (_selectionLayer == null)
                return;

            if (!IsSelected)
                return;


            _selectionLayer.Move(x, y);

            //MergeLayerOnTop(tLayer);
        }
    }
}
