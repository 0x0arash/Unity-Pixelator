using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class IFillable : IDrawable
    {
        public IFillable(int width, int height) : base(width, height)
        {
        }

        public void Clear()
        {
            Fill(new Color(0, 0, 0, 0));
        }

        public void Fill(Color color)
        {
            _pixels = Enumerable.Repeat(color, _pixels.Length).ToArray();
        }

        public void FloodFill(Color color, Vector2Int startPosition)
        {
            FloodFill(color, startPosition.x, startPosition.y);
        }

        public void FloodFill(Color color, int x, int y)
        {
            InternalFloodFill(color, x, y, GetPixelColor(x, y));
        }

        private void InternalFloodFill(Color color, int x, int y, Color startColor)
        {
            if (GetPixelColor(x, y) != startColor)
                return;

            if (x > 0 && x < Width - 1 && y > 0 && y < Height - 1)
            {
                SetPixelColor(x, y, color);

                InternalFloodFill(color, x + 1, y, startColor);
                InternalFloodFill(color, x - 1, y, startColor);
                InternalFloodFill(color, x, y + 1, startColor);
                InternalFloodFill(color, x, y - 1, startColor);
            }
        }
    }
}
