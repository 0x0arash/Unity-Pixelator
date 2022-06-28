using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class Layer : ISelectable
    {
        private readonly Image _image;
        private readonly string _name;

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

        public Layer(string name, Image image) : base(image.Width, image.Height, image._renderLayer)
        {
            _name = name;
            _image = image;
        }

        internal Layer(string name, int width, int height) : base(width, height, null)
        {
            _name = name;
        }

        public void Remove()
        {
            Image.RemoveLayer(this);
        }
    }
}
