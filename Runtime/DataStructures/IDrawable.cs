using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public abstract class IDrawable
    {
        private readonly int _width, _height;
        private readonly Texture2D _texture;
        private Color[] _pixels;

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public IDrawable(int width, int height)
        {
            _width = width;
            _height = height;

            _pixels = new Color[width * height];
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            _texture.filterMode = FilterMode.Point;
        }

        public virtual Texture2D Render()
        {
            _texture.SetPixels(_pixels);
            _texture.Apply();

            return _texture;
        }

        public Texture2D GetRenderedTexture()
        {
            return _texture;
        }

        public Color GetPixelColor(Vector2Int position)
        {
            return GetPixelColor(position.x, position.y);
        }

        public Color GetPixelColor(int x, int y)
        {
            return _pixels[y * _width + x];
        }

        public void SetPixelColor(Vector2Int position, Color color)
        {
            SetPixelColor(position.x, position.y, color);
        }

        public void SetPixelColor(int x, int y, Color color)
        {
            _pixels[y * _width + x] = color;
        }

        public void Fill(Color color)
        {
            _pixels = Enumerable.Repeat(color, _pixels.Length).ToArray();
        }
    }
}
