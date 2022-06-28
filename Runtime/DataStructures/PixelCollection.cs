using ArashGh.Pixelator.Runtime.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class PixelCollection : Dictionary<Vector2Int, Color32>
    {
        private int _width, _height;
        private int _minX, _minY, _maxX, _maxY;

        public PixelCollection(int width, int height) : this(width, height, new Color32(0, 0, 0, 0))
        {
        }

        public PixelCollection(int width, int height, Color32 color) : base(width * height)
        {
            _width = width;
            _height = height;

            _minX = 0;
            _minY = 0;
            _maxX = width;
            _maxY = height;

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    this[x, y] = color;
                }
            }
        }

        public Color32 this[int x, int y]
        {
            get
            {
                if (x < _minX || x > _maxX || y < _minY || y > _maxY)
                    return new Color32(0, 0, 0, 0);

                var compKey = new Vector2Int(x, y);

                if (!ContainsKey(compKey))
                {
                    Add(compKey, new Color32(0, 0, 0, 0));
                }

                return this[compKey];
            }

            set
            {
                SetMinMax(x, y);

                var compKey = new Vector2Int(x, y);
                if (!ContainsKey(compKey))
                {
                    Add(compKey, value);
                }
                else if (!this[compKey].IsEqualTo(value))
                {
                    this[compKey] = value;
                }
            }
        }

        public void SetMinMax(int x, int y)
        {
            if (x < _minX)
                _minX = x;
            if (y < _minY)
                _minY = y;
            if (x > _maxX)
                _maxX = x;
            if (y > _maxY)
                _maxY = y;
        }
    }
}
