using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public abstract class IDrawable
    {
        private Vector2Int _position;
        private readonly int _width, _height;
        private readonly Texture2D _texture;
        protected IDrawable _parent;
        protected PixelCollection _pixels;
        private bool _needRender = true;
        protected bool NeedRender {
            get
            {
                return _needRender;
            }
            set
            {
                if (_parent != null)
                {
                    _parent.NeedRender = true;
                }

                _needRender = value;
            }
        }

        public Vector2Int Position
        {
            get
            {
                return _position;
            }
        }

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

        public IDrawable(int width, int height, IDrawable parent)
        {
            _width = width;
            _height = height;
            _parent = parent;

            _pixels = new PixelCollection(width, height);
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
        }

        public virtual Texture2D Render()
        {
            if (_needRender)
            {
                NeedRender = false;
                _texture.SetPixels32(_pixels.ViewableAreaArray(_position), 0);
                _texture.Apply();
            }

            return _texture;
        }

        public Texture2D GetRenderedTexture()
        {
            return _texture;
        }

        public Color32 GetPixelColor(Vector2Int position)
        {
            return GetPixelColor(position.x, position.y);
        }

        public Color32 GetPixelColor(int x, int y)
        {
            return _pixels[x, y];
        }

        public Color32 GetPositionedPixelColor(int x, int y)
        {
            return _pixels[x + _position.x, y + _position.y];
        }

        public void SetPixelColor(Vector2Int position, Color32 color)
        {
            SetPixelColor(position.x, position.y, color);
        }

        public void SetPixelColor(int x, int y, Color32 color)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return;

            _pixels[x, y] = color;
            NeedRender = true;
        }

        public void MergeLayerOnTop(Layer layer)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    var currentColor = GetPixelColor(j, i);
                    var layerColor = layer.GetPixelColor(j, i);

                    SetPixelColor(j, i, Color32.Lerp(currentColor, layerColor, layerColor.a));
                }
            }
        }

        public void ExportRenderedImage(string path)
        {
            var tex2D = Render();
            var pngBytes = tex2D.EncodeToPNG();

            File.WriteAllBytes(path, pngBytes);
        }

        protected void ChangePosition(Vector2Int dPos)
        {
            ChangePosition(dPos.x, dPos.y);
        }

        protected void ChangePosition(int x, int y)
        {
            _position.x += x;
            _position.y += y;

            if (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0)
            {
                NeedRender = true;
            }
        }
    }
}
