using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class Pixel
    {
        private readonly Layer _layer;
        private Vector2Int _position;

        public Layer Layer
        {
            get
            {
                return _layer;
            }
        }

        public Vector2Int Position
        {
            get
            {
                return _position;
            }
        }

        public Color Color
        {
            get
            {
                return Layer.GetPixelColor(Position);
            }
        }

        public void SetColor(Color color)
        {
            Layer.SetPixelColor(Position, color);
        }
    }
}
