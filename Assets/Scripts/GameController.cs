using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public Button botonVerCinematica;

    [Header("Panel Tutorial")]
    public GameObject panelTutorial;
    public TMP_Text textoTituloTutorial;
    public TMP_Text textoContenidoTutorial;
    public Button botonContinuarTutorial;
    public Button botonSaltarTutorial;
    public TMP_Text textoContadorPaginas;
    public TMP_Text textoContadorPaginas2;
    public Button botonRepasarTutorial;

    private bool repasandoTutorial = false;
    private PaginaTutorialData[] paginasTutorialActual;
    private int paginaTutorialIndex = 0;
    private int currentTextoId = -1;

    void Start()
    {
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnTextoRecibido += AlRecibirTexto;
            NetworkManager.Instance.OnFeedbackRecibido += AlRecibirFeedback;
            NetworkManager.Instance.OnFinDelJuego += AlTerminarJuego;
            NetworkManager.Instance.OnTutorialRecibido += AlRecibirTutorial;
        }

        if (botonRepasarTutorial != null) botonRepasarTutorial.onClick.AddListener(AlPulsarBotonRepaso);
        if (botonContinuarTutorial != null) botonContinuarTutorial.onClick.AddListener(AvanzarPaginaTutorial);
        if (botonSaltarTutorial != null) botonSaltarTutorial.onClick.AddListener(TerminarTutorial);

        if (botonIniciarJuego != null) botonIniciarJuego.onClick.AddListener(ComenzarJuego);
        if (botonContinuar != null) botonContinuar.onClick.AddListener(IrAlPanelJuego);
        if (botonEnviar != null) botonEnviar.onClick.AddListener(EnviarRespuesta);
        if (botonSiguiente != null) botonSiguiente.onClick.AddListener(SiguienteCaso);
        if (botonReiniciar != null) botonReiniciar.onClick.AddListener(ReiniciarTodo);
        if (botonSalirFin != null) botonSalirFin.onClick.AddListener(VolverAlMenuInicio);
        if (botonVerCinematica != null) botonVerCinematica.onClick.AddListener(CargarCinematica);

        panelFin.SetActive(false);
        ActivarPanel(panelInicio);
    }

    public void AlPulsarBotonRepaso()
    {
        if (menuController != null) menuController.BTN_IrARepasoTutorial();
        RepasarTutorialMenu();
    }

    public void ComenzarJuego()
    {
        repasandoTutorial = false;
        NetworkManager.Instance.PedirTutorial();
    }

    public void RepasarTutorialMenu()
    {
        repasandoTutorial = true;

        if (paginasTutorialActual != null && paginasTutorialActual.Length > 0)
        {
            paginaTutorialIndex = 0;
            MostrarPaginaTutorial();
            ActivarPanel(panelTutorial);
        }
        else
        {
            NetworkManager.Instance.PedirTutorial();
        }
    }

    void AlRecibirTutorial(TutorialInfoData data)
    {
        if (data.paginas != null && data.paginas.Length > 0)
        {
            paginasTutorialActual = data.paginas;
        }

        if (!repasandoTutorial && data.completado)
        {
            NetworkManager.Instance.PedirSiguienteTexto();
            return;
        }

        if (paginasTutorialActual == null || paginasTutorialActual.Length == 0)
        {
            NetworkManager.Instance.PedirSiguienteTexto();
            return;
        }

        paginaTutorialIndex = 0;
        MostrarPaginaTutorial();
        ActivarPanel(panelTutorial);
    }

    public void AvanzarPaginaTutorial()
    {
        paginaTutorialIndex++;
        if (paginaTutorialIndex >= paginasTutorialActual.Length)
        {
            TerminarTutorial();
        }
        else
        {
            MostrarPaginaTutorial();
        }
    }

    public void MostrarPaginaTutorial()
    {
        PaginaTutorialData pag = paginasTutorialActual[paginaTutorialIndex];

        textoTituloTutorial.text = pag.titulo;
        textoContenidoTutorial.text = pag.contenido;

        if (textoContadorPaginas != null)
            textoContadorPaginas.text = $"{paginaTutorialIndex + 1} / {paginasTutorialActual.Length}";

        if (textoContadorPaginas2 != null)
            textoContadorPaginas2.text = $"{paginaTutorialIndex + 1} / {paginasTutorialActual.Length}";

        if (paginaTutorialIndex == paginasTutorialActual.Length - 1)
            botonContinuarTutorial.GetComponentInChildren<TMP_Text>().text = "Comenzar";
        else
            botonContinuarTutorial.GetComponentInChildren<TMP_Text>().text = "Siguiente";
    }

    public void TerminarTutorial()
    {
        if (panelTutorial != null) panelTutorial.SetActive(false);

        if (!repasandoTutorial)
        {
            NetworkManager.Instance.MarcarTutorialCompletado();
        }

        repasandoTutorial = false;
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
        if (panelTutorial != null) panelTutorial.SetActive(false);
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

    public void CargarCinematica()
    {
        SceneManager.LoadScene("CinematicaFinal");
    }
}