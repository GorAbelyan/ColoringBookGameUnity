using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class SceneParams
{
    public static string categoryName;
}

public class CategoryItemClick : MonoBehaviour
{

    public AudioSource clickSound;
    public float delayBeforeLoad = 1f;

    public string sceneToLoad;
    public string CategoryName;


    public void LoadScene()
    {
        clickSound.Play();
        StartCoroutine(LoadSceneAfterDelay());
    }
    public void LoadSceneFromBackButton()
    {
        if (!string.IsNullOrEmpty(CategoryName))
            SceneParams.categoryName = CategoryName;

        SceneManager.LoadScene(sceneToLoad);
    }



    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        if (!string.IsNullOrEmpty(CategoryName))
            SceneParams.categoryName = CategoryName;

        SceneManager.LoadScene(sceneToLoad);
    }
}