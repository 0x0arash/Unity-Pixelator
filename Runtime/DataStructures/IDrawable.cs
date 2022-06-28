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

            _pixels = new PixelCollection(width, height);
            _texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
        }

        public virtual Texture2D Render(bool forceRender = false)
        {
            if (_needRender || forceRender)
            {
                if (_parent != null)
                    _parent.NeedRender = _needRender || forceRender;

                _needRender = false;

                var pixels = ViewableAreaArray();

                _texture.SetPixels32(pixels, 0);
                _texture.Apply();
            }

            return _texture;
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

        public Color32 GetPixelColor(Vector2Int pos)
        {
            return GetPixelColor(pos.x, pos.y);
        }

        public void SetPixelColor(Vector2Int position, Color32 color)
        {
            SetPixelColor(position.x, position.y, color);
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

            _pixels[x + pos.x, y + pos.x] = color;
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

                if (!_pixels.ContainsKey(key))
                {
                    SetPixelColor(key.Item1, key.Item2, layer.GetPixelColor(key.Item1, key.Item2));
                }
                else
                {
                    SetPixelColor(key.Item1, key.Item2, Color32.Lerp(GetPixelColor(key.Item1, key.Item2),
                        layer.GetPixelColor(key.Item1, key.Item2),
                        layer.GetPixelColor(key.Item1, key.Item2).a / 255.0f));
                }
            }

            //for (int i = 0; i < layerPixelKeysList.Count; i++)
            //{
            //    var pos = layer._position;
            //    var layerParent = layer._parent;
            //    while (layerParent != null)
            //    {
            //        pos += layerParent._position;
            //        layerParent = layerParent._parent;
            //    }

            //    var adjustedPosition = new Vector2Int(layerPixelKeysList[i].Item1 + pos.x, layerPixelKeysList[i].Item2 + pos.y);
            //    var key = new Tuple<int, int>(layerPixelKeysList[i].Item1, layerPixelKeysList[i].Item2);
            //    if (!layerContents.ContainsKey(key))
            //        continue;

            //    var layerPixel = layerContents[key];
            //    if (layerPixel.IsEqualTo(new Color32(0, 0, 0, 0)))
            //        continue;

            //    var currentColor = GetPixelColor(adjustedPosition);
            //    SetPixelColor(adjustedPosition, Color32.Lerp(currentColor, layerPixel, layerPixel.a / 255.0f));
            //}
        }

        public void ExportRenderedImage(string path)
        {
            var tex2D = Render();
            var pngBytes = tex2D.EncodeToPNG();

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
                Debug.Log($"{x}, {y}");
                NeedRender = true;

                if (_overlay != null)
                    _overlay.Move(x, y);

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

                if (_overlay != null)
                    _overlay.MoveTo(x, y);

                _position.x = x;
                _position.y = y;
            }
        }

        protected PixelCollection GetContents()
        {
            return _pixels;
        }

        protected Color32[] ViewableAreaArray()
        {
            var result = new Color32[_width * _height];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    result[y * _width + x] = GetPixelColor(x, y);
                }
            }

            return result;
        }
    }
}
