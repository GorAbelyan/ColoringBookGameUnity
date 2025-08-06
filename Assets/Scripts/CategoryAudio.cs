using UnityEngine;

public class CategoryAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip categoryClip;

    public void PlayCategorySound()
    {
            if (categoryClip == null) return;

        // Create a temporary GameObject to play the sound
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = categoryClip;
        tempSource.Play();

        // Destroy the GameObject after the clip finishes
        Object.Destroy(tempGO, categoryClip.length);
    }
}