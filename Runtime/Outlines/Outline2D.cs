using ArashGh.Pixelator.Runtime.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.Oulines
{
    public static class Outline2D
    {
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
            var newLayer = new BaseLayer("TemporaryPrimitive", layer.Width, layer.Height);

            for (int x = min.x; x <= max.x; x++)
            {
                for (int y = min.y; y <= max.y; y++)
                {
                    if (x - min.x < thickness || x > max.x - thickness ||
                        y - min.y < thickness || y > max.y - thickness)
                    {
                        newLayer.SetPixelColor(x, y, color);
                    }
                }
            }

            layer.WriteLayerOnTop(newLayer);
        }

        public static void OutlineSelection(LayerSelection selection, Color32 color, bool outlineOutside, bool followShape, int thickness)
        {
            if (selection == null)
                return;

            int minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = 0;

            for (int i = 0; i < selection.SelectionList.Count; i++)
            {
                var s = selection.SelectionList[i];
                if (i == 0)
                {
                    minX = s.x;
                    minY = s.y;
                    maxX = s.x;
                    maxY = s.y;
                }
                else
                {
                    if (s.x < minX)
                        minX = s.x;
                    if (s.y < minY)
                        minY = s.y;
                    if (s.x > maxX)
                        maxX = s.x;
                    if (s.y > maxY)
                        maxY = s.y;
                }
            }

            if (!followShape)
            {
                DrawRectangleOutline(selection._selectionLayer, color, thickness, new Vector2Int(minX, minY), new Vector2Int(maxX, maxY));
            }
            else
            {

            }
        }
    }
}
