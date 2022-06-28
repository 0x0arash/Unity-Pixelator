@ArashGh

# Pixelator

## What is it?

Pixelator is a Unity package which allows you to generate and design pixel art programmatically. (this is for now, automatic generation is on the way)

## Who is it for?

I mainly started developing this package because I'm not an artist and sometimes I feel much easier expressing what I want in my game art with code.

# Install

- Open Unity Package Manager
- Click the "+" icon and choose "add package from git url"
- Enter "https://github.com/ArashGh/Unity-Pixelator.git"
- Apply

-- Tested with **Unity 2021**

# Example

```c#
    public class BasicImageManipulation : MonoBehaviour
    {
        Image image;

        void Start()
        {
            // Creating a new Image
            var image = new Image(64, 64, false);

            // Each Image starts with a layer called "Base"
            // You can access different layers of an Image using the [] (bracket) operator and passing the layer name
            // SetPixelColor can be used on any layer to set individual pixel color
            image["Base"].SetPixelColor(63, 0, Color.cyan);
            image["Base"].SetPixelColor(0, 63, Color.cyan);

            // You can add new layers to the image
            // There are several methods to help you do that
            // => InsertLayerOnTop, InsertLayerOnBottom, InsertLayerUnder, InsertLayerAbove
            image.InsertLayerAbove("Pattern", "Base");

            // You can set and manipulate pixels on a layer in any way you want
            for (int i = 0; i < 64 * 64; i++)
            {
                if (i % 4 == 0)
                    image["Pattern"].SetPixelColor(i % 64, i / 64, Color.blue);
            }

            // The Render method on Image or Layer object will render the pixel color buffer to a Texture2D inside the object
            image.Render();
        }

        private void OnGUI()
        {
            // This is just for demonstration purposes
            // You can Access the final Image or Layer rendered texture using the GetRenderedTexture method
            GUI.DrawTexture(new Rect((Screen.width - image.Width * 4) / 2, (Screen.height - image.Height * 4) / 2, image.Width * 4, image.Height * 4), image.GetRenderedTexture());
        }
    }
```

For a more comprehensive sample please see [This Example](Samples/BasicImageManipulation.cs)

## Sample Output
<img src="Documentation/sample.png" width="256" height="256" />

# Community
If you have any suggestions or you found a problem in the package don't hesitate to tell me in [The Discussion](https://github.com/ArashGh/Unity-Pixelator/discussions) section.

# License
For license information please refer to (see [LICENSE](LICENSE.md))