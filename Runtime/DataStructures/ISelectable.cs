using ArashGh.Pixelator.Runtime.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class LayerSelection
    {
        public List<Vector2Int> SelectionList = new List<Vector2Int>();
        internal Selection _selectionLayer;
    }

    internal class Selection : BaseLayer
    {
        internal Selection(string name, int width, int height) : base(name, width, height)
        {
        }
    }

    public class ISelectable : IFillable
    {
        private bool _isSelected = false;

        public bool IsSelected
        {
            get
            {
                return _selections.SelectionList.Count > 0 && _isSelected;
            }
        }

        public List<Vector2Int> Selections
        {
            get
            {
                return _selections.SelectionList;
            }
        }

        private LayerSelection _selections = new();
        private List<Vector2Int> _tempSelection = new List<Vector2Int>();

        public ISelectable(int width, int height, IDrawable parent) : base(width, height, parent)
        {
            if (parent != null)
                _selections._selectionLayer = new Selection("Selection", width, height);
        }

        public LayerSelection GetSelections()
        {
            return _selections;
        }

        public void ApplySelection()
        {
            if (_selections._selectionLayer == null)
                return;
            if (_selections.SelectionList.Count == 0)
                return;

            _isSelected = true;

            for (int i = 0; i < _selections.SelectionList.Count; i++)
            {
                _selections._selectionLayer.SetPixelColor(_selections.SelectionList[i], GetPixelColor(_selections.SelectionList[i]));
                SetRawPixelColor(_selections.SelectionList[i] - Position, PixelCollection.transparent);
            }

            _overlay = _selections._selectionLayer;
        }

        public void RectangleSelect(Vector2Int start, Vector2Int end)
        {
            int x = Mathf.Min(start.x, end.x), y = Mathf.Min(start.y, end.y);
            int targetX = Mathf.Max(start.x, end.x), targetY = Mathf.Max(start.y, end.y);

            int startY = y;

            _tempSelection.Clear();
            Deselect();

            while (x <= targetX)
            {
                while (y <= targetY)
                {
                    SelectPosition(x, y);
                    y += 1;
                }

                x += 1;
                y = startY;
            }

            ApplySelection();
        }

        public void MagicSelect(Vector2Int startPosition)
        {
            var startColor = GetPixelColor(startPosition);

            Deselect();

            InternalMagicSelect(startPosition.x, startPosition.y, startColor);

            foreach (var selection in _tempSelection)
            {
                SelectPosition(selection.x, selection.y);
            }

            _tempSelection.Clear();

            ApplySelection();
        }

        private void InternalMagicSelect(int x, int y, Color32 startColor)
        {
            var position = new Vector2Int(x, y);
            var isSelected = _tempSelection.Contains(position);

            if (isSelected)
                return;
            if (!GetPixelColor(x, y).IsEqualTo(startColor))
                return;

            if (!_tempSelection.Contains(position))
            {
                _tempSelection.Add(position);
            }

            InternalMagicSelect(x - 1, y, startColor);
            InternalMagicSelect(x + 1, y, startColor);
            InternalMagicSelect(x, y - 1, startColor);
            InternalMagicSelect(x, y + 1, startColor);
        }

        private void SelectPosition(Vector2Int position)
        {
            SelectPosition(position.x, position.y);
        }

        private void SelectPosition(int x, int y)
        {
            var position = new Vector2Int(x, y);

            if (!_selections.SelectionList.Contains(position))
            {
                _selections.SelectionList.Add(position);
            }
        }

        private void Deselect(bool write = true)
        {
            if (write)
                WriteLayerOnTop(_selections._selectionLayer);

            _selections._selectionLayer.MoveTo(0, 0);
            _selections._selectionLayer.Clear();
            _isSelected = false;
            _selections.SelectionList.Clear();
            _tempSelection.Clear();
            _overlay = null;
        }

        public void Deselect()
        {
            Deselect(true);
        }

        public void FillSelection(Color32 color)
        {
            if (!IsSelected)
                return;

            for (int i = 0; i < _selections.SelectionList.Count; i++)
            {
                Vector2Int selection = _selections.SelectionList[i];
                _selections._selectionLayer.SetRawPixelColor(selection.x, selection.y, color);
            }
        }

        public void MoveSelection(Vector2Int dPos)
        {
            MoveSelection(dPos.x, dPos.y);
        }

        public void MoveSelection(int x, int y)
        {
            if (_selections._selectionLayer == null)
                return;

            if (!IsSelected)
                return;

            _selections._selectionLayer.Move(x, y);
            NeedRender = true;
        }
    }
}
