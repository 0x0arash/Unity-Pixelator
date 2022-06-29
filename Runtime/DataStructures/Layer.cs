using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class BaseLayer : ISelectable
    {
        private readonly string _name;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        internal BaseLayer(string name, IDrawable parent) : base(parent.Width, parent.Height, parent)
        {
            _name = name;
        }

        internal BaseLayer(string name, int width, int height) : base(width, height, null)
        {
            _name = name;
        }
    }

    public class Layer : BaseLayer
    {
        private readonly Image _image;

        public Image Image
        {
            get
            {
                return _image;
            }
        }

        public Layer(string name, Image image) : base(name, image._renderLayer)
        {
            _image = image;
        }

        public void Remove()
        {
            Image.RemoveLayer(this);
        }
    }
}
