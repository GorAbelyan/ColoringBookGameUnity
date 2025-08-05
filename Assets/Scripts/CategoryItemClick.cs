using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneParams
{
    public static string categoryName;
}

public class CategoryItemClick : MonoBehaviour
{
    public string sceneToLoad;
    public string CategoryName;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(CategoryName))
            SceneParams.categoryName = CategoryName;
        SceneManager.LoadScene(sceneToLoad);
    }
}