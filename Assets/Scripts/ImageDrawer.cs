using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageDrawer : MonoBehaviour
{
    [Header("UI Elements")]
    public RawImage rawImage;

    [Header("Drawing Settings")]
    public int brushRadius = 300;
    public Color brushColor = Color.red;
    public Camera uiCamera; // Optional, assign if using World Space Canvas

    private Texture2D texture;
    private int width = 1000;
    private int height = 1000;
    private Color[] pixels;
    private List<Vector2Int> circleOffsets;

    private bool isDrawing = false;
    private Vector2Int? lastPixelPos = null;

    void Start()
    {
        // Create texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        pixels = new Color[width * height];

        // Fill background white
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        texture.SetPixels(pixels);
        texture.Apply();

        rawImage.texture = texture;

        // Precompute circle offsets for brush
      //  PrecomputeCircleOffsets();
    }

    public void SetBrushColor(Color newColor)
    {
        brushColor = newColor;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lastPixelPos = null; // Reset
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            lastPixelPos = null; // Reset
        }

        if (!isDrawing) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImage.rectTransform,
            Input.mousePosition,
            uiCamera,
            out localPoint))
        {
            float x = localPoint.x + rawImage.rectTransform.rect.width / 2f;
            float y = localPoint.y + rawImage.rectTransform.rect.height / 2f;

            int px = Mathf.RoundToInt((x / rawImage.rectTransform.rect.width) * width);
            int py = Mathf.RoundToInt((y / rawImage.rectTransform.rect.height) * height);

            Vector2Int currentPos = new Vector2Int(px, py);

            if (lastPixelPos.HasValue)
            {
                // Interpolate between last and current position
                DrawLine(lastPixelPos.Value, currentPos);
            }
            else
            {
                DrawCircle(currentPos.x, currentPos.y, brushColor);
            }

            lastPixelPos = currentPos;

            texture.SetPixels(pixels);
            texture.Apply();
        }
    }

    void DrawLine(Vector2Int start, Vector2Int end)
    {
        int steps = Mathf.CeilToInt(Vector2Int.Distance(start, end));
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(start.x, end.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(start.y, end.y, t));
            DrawCircle(x, y, brushColor);
        }
    }

    void DrawCircle(int cx, int cy, Color color)
    {
        for (int y = -brushRadius; y <= brushRadius; y++)
        {
            for (int x = -brushRadius; x <= brushRadius; x++)
            {
                float dist = Mathf.Sqrt(x * x + y * y);
                if (dist <= brushRadius)
                {
                    int px = cx + x;
                    int py = cy + y;

                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        float alpha = 1f - (dist / brushRadius); // fade edge
                        alpha = Mathf.Clamp01(alpha);

                        Color existing = pixels[py * width + px];
                        Color blended = Color.Lerp(existing, color, alpha);

                        pixels[py * width + px] = blended;
                    }
                }
            }
        }
    }

    void PrecomputeCircleOffsets()
    {
        circleOffsets = new List<Vector2Int>();
        for (int y = -brushRadius; y <= brushRadius; y++)
        {
            for (int x = -brushRadius; x <= brushRadius; x++)
            {
                if (x * x + y * y <= brushRadius * brushRadius)
                {
                    circleOffsets.Add(new Vector2Int(x, y));
                }
            }
        }
    }
}
