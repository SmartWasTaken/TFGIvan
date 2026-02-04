using UnityEngine;
using System.Runtime.InteropServices;

public class BotonSalir : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void JS_CerrarJuego();

    public void AlPulsarSalir()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Si estamos en la web, llamamos a JavaScript
            JS_CerrarJuego();
#else
        Application.Quit();
        Debug.Log("Saliendo del juego...");
#endif
    }
}