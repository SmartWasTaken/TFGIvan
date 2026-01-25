using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cámaras Virtuales")]
    public GameObject camSplash;
    public GameObject camMainMenu;
    public GameObject camPlayMenu;
    public GameObject camOptionsMenu;
    public GameObject camCreditsMenu;
    private List<GameObject> allCameras;

    void Awake()
    {
        allCameras = new List<GameObject> { camSplash, camMainMenu, camPlayMenu, camOptionsMenu, camCreditsMenu };
    }
    public void CamActivate(GameObject cameraToActivate)
    {
        foreach (var c in allCameras)
        {
            c.SetActive(false);
        }
        if(cameraToActivate != null) cameraToActivate.SetActive(true);
    }

}
