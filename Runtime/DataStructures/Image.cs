using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.DataStructures
{
    public class Image
    {
        private int _width, _height;

        private readonly List<Layer> _layers;
        protected internal Layer _renderLayer;

        public List<Layer> Layers
        {
            get
            {
                return _layers;
            }
        }

        public int Width { get => _width; }
        public int Height { get => _height; }

        public Layer this[string layerName]
        {
            get
            {
                return _layers.Find(l => l.Name == layerName);
            }
        }

        public Image(int width, int height)
        {
            _width = width;
            _height = height;

            _renderLayer = new Layer("RenderLayer", _width, _height);
            _layers = new List<Layer>
            {
                new Layer("Base", this)
            };
        }

        public Texture2D Render(bool forceRender = false)
        {
            if (_renderLayer.NeedRender || forceRender)
            {
                _renderLayer.Clear();

                for (int i = 0; i < _layers.Count; i++)
                {
                    Layer layer = _layers[i];
                    _renderLayer.WriteLayerOnTop(layer);
                }

                return _renderLayer.Render();
            }

            return _renderLayer.GetRenderedTexture();
        }

        public Texture2D GetRenderedTexture()
        {
            return _renderLayer.GetRenderedTexture();
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

        public void RemoveLayer(string name)
        {
            _layers.Remove(this[name]);
        }

        public void RemoveLayer(Layer layer)
        {
            _layers.Remove(layer);
        }
    }
}
