using ArashGh.Pixelator.Runtime.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class PixelCollection : Dictionary<Tuple<int, int>, Color32>
    {
        private int _width, _height;

        public PixelCollection(int width, int height) : this(width, height, new Color32(0, 0, 0, 0))
        {
        }

        public PixelCollection(int width, int height, Color32 color) : base(width * height)
        {
            _width = width;
            _height = height;

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
                var compKey = Tuple.Create(x, y);

                if (!ContainsKey(compKey))
                {
                    Add(compKey, new Color32(0, 0, 0, 0));
                }

                return this[compKey];
            }

            set
            {
                var compKey = Tuple.Create(x, y);
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
    }
}
