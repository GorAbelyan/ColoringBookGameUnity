using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public ImageDrawer drawer;
    public Button redButton;
    public Button greenButton;
    public Button eraseButton;

    void Start()
    {
        redButton.onClick.AddListener(() => drawer.SetBrushColor(Color.red));
        greenButton.onClick.AddListener(() => drawer.SetBrushColor(Color.green));
        eraseButton.onClick.AddListener(() => drawer.SetBrushColor(Color.white));
    }
}
