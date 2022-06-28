using ArashGh.Pixelator.Runtime.DataStructures;
using ArashGh.Pixelator.Runtime.Primitives;
using UnityEngine;

namespace ArashGh.Pixelator.Samples
{
    public class BasicImageManipulation : MonoBehaviour
    {
        Image image;

        Layer movableLayer;

        Vector3 lastMousePos;
        bool moving = false;

        private void Start()
        {
            Test1();
            // Test2();
        }

        void Test1()
        {
            image = new Image(64, 64);

            // image["Base"].Fill(Color.black);
            movableLayer = image["Base"];

            // You can set and manipulate pixels on a layer in any way you want
            for (int i = 0; i < 64 * 64; i++)
            {
                if (i % 4 == 0)
                    movableLayer.SetPixelColor(i % 64, i / 64, Color.blue);
            }

            movableLayer.RectangleSelect(new Vector2Int(10, 10), new Vector2Int(25, 15), SelectionType2D.Replace);
            movableLayer.ApplySelection();

            // MoveSelection will move the selected pixels by the specified amount
            movableLayer.MoveSelection(12, 20);
            movableLayer.FillSelection(Color.white);
            movableLayer.Deselect();

            Primitive2D.DrawCircle(movableLayer, new Vector2Int(20, 35), 10, Color.white, true, Color.grey);
            movableLayer.MagicSelect(new Vector2Int(20, 35));
            movableLayer.ApplySelection();
            movableLayer.FillSelection(Color.red);
            movableLayer.MoveSelection(-20, 0);
            movableLayer.Deselect();

            image.Render();
        }

        void Test2()
        {
            //============Uncomment to time the operations
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            // Creating a new Image
            image = new Image(64, 64);

            // Each Image starts with a layer called "Base"
            // You can access different layers of an Image using the [] (bracket) operator and passing the layer name
            // SetPixelColor can be used on any layer to set individual pixel color
            image["Base"].SetPixelColor(63, 0, Color.cyan);
            image["Base"].SetPixelColor(0, 63, Color.cyan);

            // You can add new layers to the image
            // There are several methods to help you do that
            // => InsertLayerOnTop, InsertLayerOnBottom, InsertLayerUnder, InsertLayerAbove
            image.InsertLayerOnTop("Top");
            image["Top"].SetPixelColor(63, 0, Color.white);
            image["Top"].SetPixelColor(0, 63, Color.white);

            image.InsertLayerOnBottom("Background");
            image["Background"].Fill(Color.black);

            image.InsertLayerUnder("HiddenUnder", "Background");
            image["HiddenUnder"].Fill(Color.red);

            movableLayer = image.InsertLayerAbove("Pattern", "Background");

            // You can set and manipulate pixels on a layer in any way you want
            for (int i = 0; i < 64 * 64; i++)
            {
                if (i % 4 == 0)
                    movableLayer.SetPixelColor(i % 64, i / 64, Color.blue);
            }

            // The Primitive2D class helps you draw simple shapes such as Lines and Circles
            Primitive2D.DrawLine(image["Top"], new Vector2Int(0, 0), new Vector2Int(63, 42), Color.white, false);
            Primitive2D.DrawCircle(image["Top"], new Vector2Int(20, 35), 10, Color.white, true, Color.grey);

            // There is a Selection system with basic tools for now. (WIP)
            // Remember to call ApplySelection to actually select
            // There are 3 different Selection modes. (Default is Replace any previous selection)
            // => Remove (Removes the new selection from the previous selection)
            // => Replace (Replaces the old selection with the new selected pixels) (This is the default behaviour if you don't specify the selection type)
            // => Add (Adds the new selection to the previous selection)

            // RectangleSelect lets you select a rectangular area
            movableLayer.RectangleSelect(new Vector2Int(35, 15), new Vector2Int(63, 63));
            movableLayer.ApplySelection();

            // You can then fill the selected pixels with the color you like
            movableLayer.FillSelection(Color.red);

            movableLayer.RectangleSelect(new Vector2Int(0, 63), new Vector2Int(15, 15));

            // MagicSelect lets you select the connected pixels with the same color starting from the position you specify (You know what magic wand does, right?)
            movableLayer.MagicSelect(new Vector2Int(5, 0), SelectionType2D.Remove);
            movableLayer.ApplySelection();
            movableLayer.FillSelection(Color.magenta);

            // Calling Deselect will apply the changes done to the selection and apply them to the corrosponding layer
            movableLayer.Deselect();

            // The Render method on Image or Layer object will render the pixel color buffer to a Texture2D inside the object
            image.Render();

            //============Uncomment to time the operations
            //sw.Stop();
            //UnityEngine.Debug.Log($"{sw.ElapsedMilliseconds}");
            //sw.Restart();


            // The ExportRenderedImage on the Image objects or Layer objects lets you export the corrosponding Image or Layer's content to a png file at the location you specify
            //image.ExportRenderedImage(Path.Combine(Application.dataPath, "test.png"));

            //============Uncomment to time the operations
            //sw.Stop();
            //UnityEngine.Debug.Log($"{sw.ElapsedMilliseconds}");
        }

        private void Update()
        {
            if (movableLayer == null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                moving = true;
                lastMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                moving = false;
            }

            if (moving)
            {
                Vector2Int dPos = new Vector2Int((int)(Input.mousePosition.x - lastMousePos.x), (int)(Input.mousePosition.y - lastMousePos.y));
                dPos /= 4;
                if (Mathf.Abs(dPos.x) > 0 || Mathf.Abs(dPos.y) > 0)
                {
                    lastMousePos = Input.mousePosition;

                    // The Move Layer on the Layer objects moves the whole layer by the amount you provide
                    movableLayer.Move(dPos.x, dPos.y);
                }
            }

            image.Render();
        }

        private void OnGUI()
        {
            // This is just for demonstration purposes
            // You can Access the final Image or Layer rendered texture using the GetRenderedTexture method
            GUI.DrawTexture(new Rect((Screen.width - image.Width * 4) / 2, (Screen.height - image.Height * 4) / 2, image.Width * 4, image.Height * 4), image.GetRenderedTexture());
        }
    }
}
