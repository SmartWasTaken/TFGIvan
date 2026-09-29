using UnityEngine;

/// <summary>
/// Configuración local del servidor. Crea el asset en
/// Assets/Resources/Local/ApiConfig.asset (menú Create > TFG > ApiConfig).
/// Esa carpeta está en .gitignore: no subas nunca direcciones ni tokens reales.
/// </summary>
[CreateAssetMenu(fileName = "ApiConfig", menuName = "TFG/ApiConfig")]
public class ApiConfig : ScriptableObject
{
    [Tooltip("URL base de la API, p. ej. https://mi-servidor.com/api/")]
    public string baseUrl = "http://localhost:8000/api/";

    [Tooltip("Token SOLO para probar en el editor. En la build WebGL el token llega desde la página web.")]
    public string editorDebugToken = "";
}
