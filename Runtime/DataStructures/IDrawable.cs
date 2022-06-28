using ArashGh.Pixelator.Runtime.Utils;
using System;
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
        protected IDrawable _overlay;
        protected PixelCollection _pixels;
        private bool _needRender = true;
        private Color32[] _temporaryPixelArray;

        protected internal bool NeedRender
        {
            get
            {
                return _needRender;
            }
            set
            {
                if (_parent != null && value)
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

            _temporaryPixelArray = new Color32[_width * _height];

            _pixels = new PixelCollection(width, height);
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
        }

        public virtual void Render(bool forceRender = false)
        {
            if (_needRender || forceRender)
            {
                if (_parent != null)
                    _parent.NeedRender = _needRender || forceRender;

                _needRender = false;

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        _temporaryPixelArray[y * _width + x] = GetPixelColor(x, y);
                    }
                }

                _texture.SetPixels32(_temporaryPixelArray, 0);
                _texture.Apply();
            }
        }

        public Texture2D GetRenderedTexture()
        {

            return _texture;
        }

        protected Color32 GetPixelColor(int x, int y)
        {
            var pos = _position;
            if (_parent != null)
            {
                pos += _parent._position;
            }

            return _overlay != null ?
                Color32.Lerp(_pixels[x - pos.x, y - pos.y], _overlay.GetPixelColor(x, y), _overlay.GetPixelColor(x, y).a / 255.0f) :
                _pixels[x - pos.x, y - pos.y];
        }

        protected Color32 GetRawPixelColor(int x, int y)
        {
            return _overlay != null ?
                Color32.Lerp(_pixels[x, y],
                _overlay.GetRawPixelColor(new Vector2Int(x, y) - _overlay._position), _overlay.GetRawPixelColor(new Vector2Int(x, y) - _overlay._position).a / 255.0f) :
                _pixels[x, y];
        }

        protected Color32 GetRawPixelColor(Vector2Int pos)
        {
            return GetRawPixelColor(pos.x, pos.y);
        }

        public Color32 GetPixelColor(Vector2Int pos)
        {
            return GetPixelColor(pos.x, pos.y);
        }

        public void SetPixelColor(Vector2Int position, Color32 color)
        {
            SetPixelColor(position.x, position.y, color);
        }

        protected void SetRawPixelColor(Vector2Int pos, Color32 color)
        {
            SetRawPixelColor(pos.x, pos.y, color);
        }

        protected void SetRawPixelColor(int x, int y, Color32 color)
        {
            if (_pixels[x, y].IsEqualTo(color))
                return;

            _pixels[x, y] = color;
            NeedRender = true;
        }

        public void SetPixelColor(int x, int y, Color32 color)
        {
            var pos = _position;
            if (_parent != null)
            {
                pos += _parent._position;
            }

            if (_pixels[x + pos.x, y + pos.y].IsEqualTo(color))
                return;

            _pixels[x + pos.x, y + pos.y] = color;
            NeedRender = true;
        }

        public void WriteLayerOnTop(IDrawable layer)
        {
            var keys = _pixels.Keys.ToList();
            keys.AddRange(layer._pixels.Keys);

            foreach (var key in keys)
            {
                if (!layer._pixels.Keys.Contains(key))
                    continue;

                if (!_pixels.ContainsKey(key + layer.Position))
                {
                    SetPixelColor(key + layer.Position, layer.GetRawPixelColor(key));
                }
                else
                {
                    SetPixelColor(key + layer.Position, Color32.Lerp(GetPixelColor(key + layer.Position),
                        layer.GetRawPixelColor(key),
                        layer.GetRawPixelColor(key).a / 255.0f));
                }
            }
        }

        public void ExportRenderedImage(string path)
        {
            Render(true);
            var pngBytes = _texture.EncodeToPNG();

            File.WriteAllBytes(path, pngBytes);
        }

        public void Move(Vector2Int dPos)
        {
            Move(dPos.x, dPos.y);
        }

        public void Move(int x, int y)
        {
            if (Mathf.Abs(x) > 0 || Mathf.Abs(y) > 0)
            {
                NeedRender = true;

                _position.x += x;
                _position.y += y;
            }
        }

        protected void MoveTo(Vector2Int pos)
        {
            MoveTo(pos.x, pos.y);
        }

        protected void MoveTo(int x, int y)
        {
            if (_position.x != x || _position.y != y)
            {
                NeedRender = true;

                _position.x = x;
                _position.y = y;
            }
        }

        protected PixelCollection GetContents()
        {
            return _pixels;
        }
    }
}
