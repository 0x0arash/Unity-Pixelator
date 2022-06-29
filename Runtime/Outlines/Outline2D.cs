using ArashGh.Pixelator.Runtime.DataStructures;
using ArashGh.Pixelator.Runtime.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.Oulines
{
    public enum SelectionOutlineType2D
    {
        Rectangle,
        Shape
    }

    public static class Outline2D
    {
        private static List<Vector2Int> pixels = new List<Vector2Int>();

        public static void OutlineLayer(BaseLayer layer, Color32 color, int thickness)
        {
            DrawRectangleOutline(layer, color, thickness, Vector2Int.zero, new Vector2Int(layer.Width - 1, layer.Height - 1));
        }

        public static void OutlineLayerContent(BaseLayer layer, Color32 color, bool outlineOutside, int thickness)
        {
            (Vector2Int min, Vector2Int max) = layer.GetPixelCollection().GetMinMax();

            if (outlineOutside)
            {
                min.x -= thickness;
                min.y -= thickness;
                max.x += thickness;
                max.y += thickness;
            }

            DrawRectangleOutline(layer, color, thickness, min, max);
        }

        private static void DrawRectangleOutline(BaseLayer layer, Color32 color, int thickness, Vector2Int min, Vector2Int max)
        {
            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    if (x - min.x < thickness || x > max.x - thickness ||
                        y - min.y < thickness || y > max.y - thickness)
                    {
                        pixels.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        private static void DrawOutlinePixels(BaseLayer layer, Color32 color)
        {
            var newLayer = new BaseLayer("TemporaryPrimitive", layer.Width, layer.Height);

            for (int i = 0; i < pixels.Count; i++)
            {
                newLayer.SetPixelColor(pixels[i], color);
            }

            pixels.Clear();

            layer.WriteLayerOnTop(newLayer);
        }

        public static void OutlineSelection(LayerSelection selection, Color32 color, bool outlineOutside, SelectionOutlineType2D outlineType, int thickness)
        {
            if (selection == null)
                return;

            switch (outlineType)
            {
                case SelectionOutlineType2D.Rectangle:
                    {
                        selection.SelectionList.Sort((a, b) => a.x.CompareTo(b.x) + a.y.CompareTo(b.y));

                        var min = selection.SelectionList.First();
                        var max = selection.SelectionList.Last();

                        if (outlineOutside)
                        {
                            min.x -= thickness;
                            min.y -= thickness;
                            max.x += thickness;
                            max.y += thickness;
                        }

                        DrawRectangleOutline(selection._selectionLayer, color, thickness, min, max);
                        break;
                    }

                case SelectionOutlineType2D.Shape:
                    {
                        foreach (var pos in selection.SelectionList)
                        {
                            int[] xDir = { -1, 1, 0, 0 };
                            int[] yDir = { 0, 0, 1, -1 };

                            for (int k = 0; k < 4; ++k)
                            {
                                var newPos = pos + new Vector2Int(xDir[k], yDir[k]) * thickness;
                                if (!selection.SelectionList.Contains(newPos))
                                {
                                    if (!outlineOutside)
                                        pixels.Add(pos);
                                    else
                                        pixels.Add(newPos);
                                }
                            }
                        }
                        break;
                    }
            }

            DrawOutlinePixels(selection._selectionLayer, color);
        }
    }
}
