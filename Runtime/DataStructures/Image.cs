using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class Image : IDrawable
    {
        private List<Layer> _layers;

        private bool _renderLayersOnRender;

        public List<Layer> Layers
        {
            get
            {
                return _layers;
            }
        }

        public Layer this[string layerName]
        {
            get
            {
                return _layers.Find(l => l.Name == layerName);
            }
        }

        public Image(int width, int height, bool renderLayersOnRender) : base(width, height)
        {
            _renderLayersOnRender = renderLayersOnRender;

            _layers = new List<Layer>();

            _layers.Add(new Layer("Base", this));
        }

        public void SetRenderLayersOnRender(bool renderLayerOnRender)
        {
            _renderLayersOnRender = renderLayerOnRender;
        }

        public override Texture2D Render()
        {
            foreach (Layer layer in _layers)
            {
                if (_renderLayersOnRender)
                    layer.Render();

                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        var currentColor = GetPixelColor(j, i);
                        var layerColor = layer.GetPixelColor(i, j);

                        SetPixelColor(j, i, Color.Lerp(currentColor, layerColor, layerColor.a));
                    }
                }
            }

            return base.Render();
        }

        public Layer InsertLayerOnTop(string name)
        {
            var newLayer = new Layer(name, this);
            _layers.Add(newLayer);

            return newLayer;
        }

        public Layer InsertLayerOnBottom(string name)
        {
            var newLayer = new Layer(name, this);
            _layers.Insert(0, newLayer);

            return newLayer;
        }

        public Layer InsertLayerUnder(string name, string aboveName)
        {
            var newLayer = new Layer(name, this);
            _layers.Insert(_layers.FindIndex(l => l.Name == aboveName), newLayer);

            return newLayer;
        }

        public Layer InsertLayerAbove(string name, string underName)
        {
            var newLayer = new Layer(name, this);
            _layers.Insert(_layers.FindIndex(l => l.Name == underName) + 1, newLayer);

            return newLayer;
        }
    }
}
