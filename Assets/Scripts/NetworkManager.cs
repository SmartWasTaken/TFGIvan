using System.Runtime.InteropServices;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    [Header("Debug")]
    public string miToken; //guardamos el token aqui

    [DllImport("__Internal")]
    private static extern string JS_GetToken();

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ObtenerToken();
    }

    public void ObtenerToken()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        miToken = JS_GetToken();
        Debug.Log("Token recibido " + miToken);
#else
        miToken = "TOKEN_FALSO_EDITOR_123";
        Debug.Log("Usando token de preuba " + miToken);
#endif

    }

}
