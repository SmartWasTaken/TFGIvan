using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameData;

public class GameController : MonoBehaviour
{
    [Header("Referencias Externas")]
    public MenuController menuController;

    [Header("Paneles Principales")]
    public GameObject panelInicio;
    public GameObject panelAmbientacion;
    public GameObject panelJuego;
    public GameObject panelFeedback;

    [Header("Elementos Inicio")]
    public Button botonIniciarJuego;

    [Header("Elementos Ambientación")]
    public TMP_Text textoAmbientacion;
    public Button botonContinuar;

    [Header("Elementos Juego")]
    public TMP_Text textoCapitulo;
    public TMP_Text textoTitulo;
    public TMP_Text textoContenido;
    public TMP_InputField inputComentario;
    public Button botonEnviar;

    [Header("Opciones de Voto")]
    public ToggleGroup grupoOpciones;
    public Toggle toggleHumano;
    public Toggle toggleIAFull;
    public Toggle toggleIAPolish;
    public Toggle toggleMixto;

    [Header("Elementos Feedback")]
    public TMP_Text textoResultado;
    public TMP_Text textoExplicacion;
    public Button botonSiguiente;

    [Header("Panel Fin")]
    public GameObject panelFin;
    public TMP_Text textoFin;
    public Button botonReiniciar;
    public Button botonSalirFin;

    private int currentTextoId = -1;

    void Start()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnTextoRecibido += AlRecibirTexto;
            NetworkManager.Instance.OnFeedbackRecibido += AlRecibirFeedback;
            NetworkManager.Instance.OnFinDelJuego += AlTerminarJuego;
        }

        botonIniciarJuego.onClick.AddListener(ComenzarJuego);
        botonContinuar.onClick.AddListener(IrAlPanelJuego);
        botonEnviar.onClick.AddListener(EnviarRespuesta);
        botonSiguiente.onClick.AddListener(SiguienteCaso);
        botonReiniciar.onClick.AddListener(ReiniciarTodo);
        botonSalirFin.onClick.AddListener(VolverAlMenuInicio);

        panelFin.SetActive(false);
        ActivarPanel(panelInicio);
    }

    public void ComenzarJuego()
    {
        NetworkManager.Instance.PedirSiguienteTexto();
    }

    void AlRecibirTexto(TextoRecibidoData data)
    {
        currentTextoId = data.id;

        textoAmbientacion.text = string.IsNullOrEmpty(data.ambientacion)
            ? "Contexto clasificado. Analice el texto directamente."
            : data.ambientacion;

        textoCapitulo.text = $"CAPÍTULO {data.orden_capitulo}: {data.nombre_capitulo}";
        textoTitulo.text = data.titulo;
        textoContenido.text = data.contenido;

        inputComentario.text = "";
        grupoOpciones.SetAllTogglesOff();

        ActivarPanel(panelAmbientacion);
        botonContinuar.interactable = true;
    }

    void AlRecibirFeedback(FeedbackRecibidoData data)
    {
        ActivarPanel(panelFeedback);

        if (data.es_acierto)
            textoResultado.text = "<color=green>¡CORRECTO!</color>";
        else
            textoResultado.text = "<color=red>INCORRECTO</color>";

        textoExplicacion.text = data.explicacion_experto;
        botonEnviar.interactable = true;
    }

    void AlTerminarJuego()
    {
        panelAmbientacion.SetActive(false);
        panelJuego.SetActive(false);
        ActivarPanel(panelFin);

        if (textoFin != null)
        {
            textoFin.text = "¡Atención! No quedan textos pendientes. Gracias por jugar.";
        }
    }

    void IrAlPanelJuego()
    {
        ActivarPanel(panelJuego);
    }

    void EnviarRespuesta()
    {
        if (!grupoOpciones.AnyTogglesOn()) return;

        string voto = "HUM";
        if (toggleHumano.isOn) voto = "HUM";
        else if (toggleIAFull.isOn) voto = "IA_FULL";
        else if (toggleIAPolish.isOn) voto = "IA_POLISH";
        else if (toggleMixto.isOn) voto = "MIX";

        string comentario = inputComentario.text;

        botonEnviar.interactable = false;
        NetworkManager.Instance.EnviarRespuesta(currentTextoId, voto, comentario);
    }

    void SiguienteCaso()
    {
        textoResultado.text = "Cargando...";
        textoExplicacion.text = "";
        NetworkManager.Instance.PedirSiguienteTexto();
    }

    void ActivarPanel(GameObject panelActivo)
    {
        panelInicio.SetActive(false);
        panelAmbientacion.SetActive(false);
        panelJuego.SetActive(false);
        panelFeedback.SetActive(false);

        panelActivo.SetActive(true);
    }

    void VolverAlMenuInicio()
    {
        ActivarPanel(panelInicio);
    }

    void ReiniciarTodo()
    {
        NetworkManager.Instance.ResetearTokenYReiniciar();
    }

    public void BTN_ReiniciarBucleJuego()
    {
        panelFin.SetActive(false);
        panelInicio.SetActive(true);
    }
}