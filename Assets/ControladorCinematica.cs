using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class ControladorCinematica : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string escenaVolver = "SampleScene";

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += AlTerminarVideo;
        }
    }

    void AlTerminarVideo(VideoPlayer vp)
    {
        SceneManager.LoadScene(escenaVolver);
    }

    public void SaltarCinematica()
    {
        SceneManager.LoadScene(escenaVolver);
    }
}