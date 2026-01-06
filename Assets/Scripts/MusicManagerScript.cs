using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    public AudioSource backgroundMusic;
    private bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            backgroundMusic.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            backgroundMusic.UnPause();
        }
    }
}
