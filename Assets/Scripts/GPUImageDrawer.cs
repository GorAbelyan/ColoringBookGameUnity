using UnityEngine;
using UnityEngine.UI;

public class GPUImageDrawerDynamic : MonoBehaviour
{
    public RawImage rawImage;
    public Texture2D brushTexture;
    public Color brushColor = Color.red;
    public Shader shader;
    [Range(0.001f, 0.5f)] public float brushSize = 0.05f;
    public Texture2D starTexture;

    private RenderTexture rt;
    private Material brushMat;
    private Vector2? lastUV = null;

    void Start()
    {
        rt = new RenderTexture(2000, 2000, 0, RenderTextureFormat.ARGB32);
        rt.Create();
        rawImage.texture = rt;


        brushMat = new Material(shader);
        brushMat.SetTexture("_BrushTex", brushTexture);
       // brushMat.SetTexture("_MaskTex", starTexture);

        // Clear white
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.white);

        Graphics.Blit(starTexture, rt);
        RenderTexture.active = null;
    }
    public void SetBrushColor(Color newColor)
    {
        brushColor = newColor;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            lastUV = null;

        if (Input.GetMouseButton(0))
        {
            if (TryGetMouseUV(out Vector2 uv))
            {
                if (lastUV.HasValue)
                {
                    // Interpolate to fill gaps
                    float dist = Vector2.Distance(lastUV.Value, uv);
                    int steps = Mathf.CeilToInt(dist * rt.width);
                    for (int i = 0; i <= steps; i++)
                    {
                        Vector2 stepUV = Vector2.Lerp(lastUV.Value, uv, i / (float)steps);
                        StampBrush(stepUV);
                    }
                }
                else
                {
                    StampBrush(uv);
                }

                lastUV = uv;
            }
        }
    }

    bool TryGetMouseUV(out Vector2 uv)
    {
        uv = Vector2.zero;
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rawImage.rectTransform,
            Input.mousePosition,
            null,
            out localPoint))
        {
            Rect rect = rawImage.rectTransform.rect;
            float u = (localPoint.x - rect.xMin) / rect.width;
            float v = 1f - (localPoint.y - rect.yMin) / rect.height;

            uv = new Vector2(u, v);
            return (u >= 0 && u <= 1 && v >= 0 && v <= 1);
        }
        return false;
    }

    void StampBrush(Vector2 uv)
    {
        if (rawImage == null || rt == null || brushTexture == null || brushMat == null)
            return;

        RenderTexture.active = rt;
        brushMat.SetColor("_Color", brushColor);
        brushMat.SetFloat("_BrushSize", brushSize);
        brushMat.SetVector("_BrushPos", new Vector4(uv.x, uv.y, 0, 0));

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);

        // RT-space center point for the UV
        float px = uv.x * rt.width;
        float py = uv.y * rt.height;

        // RawImage rect in local units (the same rect used to compute UVs)
        Rect imageRect = rawImage.rectTransform.rect;
        float rectW = imageRect.width;
        float rectH = imageRect.height;
        if (rectW <= 0 || rectH <= 0)
        {
            GL.PopMatrix();
            RenderTexture.active = null;
            return;
        }

        // --- CORRECT APPROACH ---
        // Choose a display-space diameter that is the same for X and Y so the brush is circular.
        // Here we use the smaller side so the circle fits regardless of orientation.
        float desiredDisplayDiameter = brushSize * Mathf.Min(rectW, rectH);

        // Convert that display size into RenderTexture pixels (X and Y separately).
        float sizePxX = desiredDisplayDiameter * (rt.width / rectW);
        float sizePxY = desiredDisplayDiameter * (rt.height / rectH);

        // Build the rectangle in RT pixel space
        Rect drawRect = new Rect(
            px - sizePxX * 0.5f,
            py - sizePxY * 0.5f,
            sizePxX,
            sizePxY
        );

        Graphics.DrawTexture(drawRect, brushTexture, brushMat);

        GL.PopMatrix();
        RenderTexture.active = null;
    }

}
