using ArashGh.Pixelator.Runtime.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class IFillable : IDrawable
    {
        public IFillable(int width, int height, IDrawable parent) : base(width, height, parent)
        {
        }

        public void Clear()
        {
            Fill(PixelCollection.transparent);
        }

        public void Fill(Color32 color)
        {
            _pixels = new PixelCollection(Width, Height, color);
        }

        public void FloodFill(Color32 color, Vector2Int startPosition)
        {
            FloodFill(color, startPosition.x, startPosition.y);
        }

        public void FloodFill(Color32 color, int x, int y)
        {
            InternalFloodFill(color, x, y, GetPixelColor(x, y));
        }

        private void InternalFloodFill(Color32 color, int x, int y, Color32 startColor)
        {
            if (!GetPixelColor(x, y).IsEqualTo(startColor))
                return;

            SetPixelColor(x, y, color);

            InternalFloodFill(color, x - 1, y, startColor);
            InternalFloodFill(color, x + 1, y, startColor);
            InternalFloodFill(color, x, y - 1, startColor);
            InternalFloodFill(color, x, y + 1, startColor);
        }
    }
}
