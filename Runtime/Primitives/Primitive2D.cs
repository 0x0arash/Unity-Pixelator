using ArashGh.Pixelator.Runtime.DataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArashGh.Pixelator.Runtime.Primitives
{
    public static class Primitive2D
    {
        public static void DrawLine(Layer layer, Vector2Int start, Vector2Int end, Color color, bool pixelPerfect = true)
        {
            if (pixelPerfect)
            {
                var dx = Mathf.Abs(end.x - start.x);
                var sx = start.x < end.x ? 1 : -1;
                var dy = -Mathf.Abs(end.y - start.y);
                var sy = start.y < end.y ? 1 : -1;
                var error = dx + dy;

                while (true)
                {
                    layer.SetPixelColor(start, color);
                    if (start == end)
                        break;

                    var e2 = 2 * error;
                    if (e2 >= dy)
                    {
                        if (start.x == end.x)
                            break;
                        error += dy;
                        start.x += sx;
                    }
                    if (e2 <= dx)
                    {
                        if (start.y == end.y)
                            break;
                        error += dx;
                        start.y += sy;
                    }
                }
            }
            else
            {
                ///TODO: Maybe implement pixel imperfect (anti-ailiased) line drawing later
                DrawLine(layer, start, end, color, true);
            }
        }

        public static void DrawCircle(Layer layer, Vector2Int position, int radius, Color outlineColor, bool fill = false, Color fillColor = default)
        {
            var newLayer = layer.Image.InsertLayerOnBottom("Temp");

            int y = radius, x = 0, d = 1 - radius;

            void drawCirclePart()
            {
                newLayer.SetPixelColor(position.x + y, position.y + x, outlineColor);
                newLayer.SetPixelColor(position.x - y, position.y + x, outlineColor);
                newLayer.SetPixelColor(position.x + y, position.y - x, outlineColor);
                newLayer.SetPixelColor(position.x - y, position.y - x, outlineColor);
                newLayer.SetPixelColor(position.x + x, position.y + y, outlineColor);
                newLayer.SetPixelColor(position.x - x, position.y + y, outlineColor);
                newLayer.SetPixelColor(position.x + x, position.y - y, outlineColor);
                newLayer.SetPixelColor(position.x - x, position.y - y, outlineColor);
            }

            while (y >= x)
            {
                drawCirclePart();

                x++;

                if (d < 0)
                {
                    d = d + (2 * x) + 1;
                }
                else
                {
                    y--;
                    d = d + 2 * (x - y) + 1;
                }
            }

            if (fill && radius > 0)
            {
                newLayer.FloodFill(fillColor, position.x, position.y);
            }

            layer.MergeLayerOnTop(newLayer);
            newLayer.Remove();
        }
    }
}
