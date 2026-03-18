using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using static GameData;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private string baseUrl = "http://13.48.27.234:8000/api/";
    private string userToken;

    public event Action<TextoRecibidoData> OnTextoRecibido;
    public event Action<FeedbackRecibidoData> OnFeedbackRecibido;
    public event Action OnErrorRecibido;
    public event Action OnFinDelJuego;

    public event Action<TutorialInfoData> OnTutorialRecibido;
    public event Action OnTutorialCompletadoConfirmado;

    public UserData userConfig;

    [Header("Debug")]
    public string tokenVisible;
    [DllImport("__Internal")]
    private static extern string JS_GetToken();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        string token = "1c42206a51d7a3574202c4b0b47310b42bb4106f";

#if UNITY_WEBGL && !UNITY_EDITOR
            try {
                token = JS_GetToken();
            } catch (Exception e) {
                Debug.LogError("No se pudo obtener el token de JS: " + e.Message);
            }
#endif

        Inicializar(token);
    }

    public void Inicializar(string token)
    {
        this.userToken = token;
        this.tokenVisible = token;
        Debug.Log("NetworkManager inicializado con token: " + token);
    }

    public void PedirSiguienteTexto()
    {
        StartCoroutine(GetRequest("siguiente-texto/"));
    }

    public void EnviarRespuesta(int textoId, string voto, string comentario)
    {
        RespuestaEnviadaData data = new RespuestaEnviadaData();
        data.texto = textoId;
        data.voto_usuario = voto;
        data.comentario = comentario;
        data.nivel_confianza = 100;
        data.tiempo_lectura_segundos = Time.timeSinceLevelLoad;

        string json = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest("enviar-respuesta/", json));
    }

    IEnumerator GetRequest(string endpoint)
    {
        string url = baseUrl + endpoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "Token " + userToken);

            if (userConfig != null && !string.IsNullOrEmpty(userConfig.nombreUsuario))
            {
                webRequest.SetRequestHeader("X-User-Name", userConfig.nombreUsuario);
            }

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;

                if (jsonResponse.Contains("FIN_DEL_JUEGO"))
                {
                    OnFinDelJuego?.Invoke();
                }
                else
                {
                    TextoRecibidoData texto = JsonUtility.FromJson<TextoRecibidoData>(jsonResponse);
                    OnTextoRecibido?.Invoke(texto);
                }
            }
            else
            {
                Debug.LogError("Error GET (" + url + "): " + webRequest.error);
                OnErrorRecibido?.Invoke();
            }
        }
    }

    IEnumerator PostRequest(string endpoint, string jsonData)
    {
        string url = baseUrl + endpoint;
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Token " + userToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                FeedbackRecibidoData feedback = JsonUtility.FromJson<FeedbackRecibidoData>(jsonResponse);
                OnFeedbackRecibido?.Invoke(feedback);
            }
            else
            {
                Debug.LogError("Error POST (" + url + "): " + webRequest.error);
                OnErrorRecibido?.Invoke();
            }
        }
    }

    public void PedirTutorial()
    {
        StartCoroutine(GetTutorialRequest("tutorial/"));
    }

    public void MarcarTutorialCompletado()
    {
        StartCoroutine(PostTutorialCompletado("tutorial/completar/"));
    }

    IEnumerator GetTutorialRequest(string endpoint)
    {
        string url = baseUrl + endpoint;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", "Token " + userToken);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("RESPUESTA TUTORIAL DJANGO: " + jsonResponse);

                TutorialInfoData tutorialData = JsonUtility.FromJson<TutorialInfoData>(jsonResponse);
                OnTutorialRecibido?.Invoke(tutorialData);
            }
            else
            {
                Debug.LogError("Error GET Tutorial: " + webRequest.error);
                OnErrorRecibido?.Invoke();
            }
        }
    }

    IEnumerator PostTutorialCompletado(string endpoint)
    {
        string url = baseUrl + endpoint;
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(new byte[0]);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Token " + userToken);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                OnTutorialCompletadoConfirmado?.Invoke();
            }
        }
    }

    [ContextMenu("Borrar Roken")]
    public void ResetearTokenYReiniciar()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    Application.ExternalEval("location.reload();"); 
#else
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
#endif
    }
}