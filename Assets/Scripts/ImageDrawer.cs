using UnityEngine;
using UnityEngine.UI;

public class ImageDrawer : MonoBehaviour
{
    public RawImage rawImage;
    private Texture2D texture;

    private int width = 256;
    private int height = 256;

    void Start()
    {
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Fill white
        Color[] fillColor = new Color[width * height];
        for (int i = 0; i < fillColor.Length; i++) fillColor[i] = Color.white;
        texture.SetPixels(fillColor);
        texture.Apply();

        rawImage.texture = texture;
    }

    void Update()
    {
        // Draw if mouse button is held
        if (Input.GetMouseButton(0)) // Left mouse
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rawImage.rectTransform,
                Input.mousePosition,
                null,
                out localPoint))
            {
                // Convert from local point to texture coordinates
                float x = localPoint.x + rawImage.rectTransform.rect.width / 2;
                float y = localPoint.y + rawImage.rectTransform.rect.height / 2;

                // Scale to texture size
                int px = Mathf.RoundToInt((x / rawImage.rectTransform.rect.width) * width);
                int py = Mathf.RoundToInt((y / rawImage.rectTransform.rect.height) * height);

                // Draw a small circle at mouse position
                DrawCircle(px, py, 5, Color.red);
                texture.Apply();
            }
        }
    }

    void DrawCircle(int cx, int cy, int radius, Color color)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 && px < width && py >= 0 && py < height)
                        texture.SetPixel(px, py, color);
                }
            }
        }
    }
}
