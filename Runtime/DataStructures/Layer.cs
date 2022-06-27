using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class Layer : ISelectable
    {
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
                return (Image)_parent;
            }
        }

        public Layer(string name, Image image) : base(image.Width, image.Height, image)
        {
            _name = name;
        }

        public void Remove()
        {
            Image.RemoveLayer(this);
        }

        public void Move(int x, int y)
        {
            ChangePosition(x, y);
        }
    }
}
