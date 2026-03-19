using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class ControladorCinematica : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string escenaVolver = "SampleScene";
    public GameObject pantallaCarga;

    void Start()
    {
        if (videoPlayer != null)
        {
            if (pantallaCarga != null) pantallaCarga.SetActive(true);

            videoPlayer.loopPointReached += AlTerminarVideo;
            videoPlayer.prepareCompleted += AlEstarListo;

            videoPlayer.Prepare();
        }
    }

    void AlEstarListo(VideoPlayer vp)
    {
        if (pantallaCarga != null) pantallaCarga.SetActive(false);
        vp.Play();
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