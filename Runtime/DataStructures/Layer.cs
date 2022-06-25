using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class Layer : IDrawable
    {
        private string _name;
        private readonly Image _image;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Image Image
        {
            get
            {
                return _image;
            }
        }

        public Layer(string name, Image image) : base(image.Width, image.Height)
        {
            _name = name;
            _image = image;
        }
    }
}
