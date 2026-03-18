using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using TMPro;

public abstract class GameState
{
    protected MenuController _ctx;
    public GameState(MenuController gc) { _ctx = gc; }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class MenuController : MonoBehaviour
{
    [Header("Referencias Generales")]
    public CameraManager camManager;
    public CanvasGroup blackScreenFade;
    public Animator splashTextAnimator;
    public AudioSource audioSource;
    public GameObject splashTextObject;

    [Header("Monitor Centro")]
    public GameObject panelMenuPrincipal;
    public GameObject panelPausa;
    public GameObject panelGameHUD;
    public GameObject panelFinal;

    [Header("Monitor Izquierda (Opciones)")]
    public GameObject panelBtnOpciones;
    public GameObject panelContenidoOpciones;

    [Header("Monitor Derecha (Créditos)")]
    public GameObject panelBtnCreditos;
    public GameObject panelContenidoCreditos;

    [Header("Monitor (Agradecimientos)")]
    public GameObject panelBtnAgradecimientos;
    public GameObject panelContenidoAgradecimientos;

    [Header("Ajustes: Volumen y Brillo")]
    public AudioMixer mainMixer;
    public Slider sliderVolumen;
    public Slider sliderBrillo;
    public Image imagenBrillo;

    [Header("Ajustes: Toggles Altavoces")]
    public Toggle toggleAltavoz1;
    public Toggle toggleAltavoz2;
    public AudioSource altavozPrincipal;

    private GameState currentState;

    [Header("Panel Nombre (Overlay)")]
    public GameObject panelNamingOverlay;
    public TMP_InputField inputNombre;
    public Button btnAceptarNombre;
    public UserData userConfig;

    [Header("UI Bienvenida")]
    public TMP_Text textoBienvenida;

    void Start()
    {
        if (sliderVolumen != null) sliderVolumen.onValueChanged.AddListener(SetVolumen);
        if (sliderBrillo != null) sliderBrillo.onValueChanged.AddListener(SetBrillo);

        if (toggleAltavoz1 != null) toggleAltavoz1.onValueChanged.AddListener(delegate { ActualizarPaneo(); });
        if (toggleAltavoz2 != null) toggleAltavoz2.onValueChanged.AddListener(delegate { ActualizarPaneo(); });

        if (sliderBrillo != null) SetBrillo(sliderBrillo.value);

        ChangeState(new SplashState(this));
    }

    void Update()
    {
        if (currentState != null) currentState.Update();
    }

    public void ChangeState(GameState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void BTN_Jugar()
    {
        ChangeState(new GameplayState(this));
    }

    public void BTN_IrAOpciones() => ChangeState(new OptionsState(this, currentState));
    public void BTN_IrACreditos() => ChangeState(new CreditsState(this, currentState));
    public void BTN_IrAAgradecimientos() => ChangeState(new AgradecimientosState(this, currentState));

    public void BTN_VolverAtras()
    {
        if (currentState is OptionsState ops) ChangeState(ops.previousState);
        else if (currentState is CreditsState creds) ChangeState(creds.previousState);
        else if (currentState is AgradecimientosState agrs) ChangeState(agrs.previousState);
        else ChangeState(new MainMenuState(this));
    }

    public void BTN_SalirJuego() => Application.Quit();
    public void BTN_Reanudar() => ChangeState(new GameplayState(this));
    public void BTN_MenuDesdeJuego() => ChangeState(new MainMenuState(this));
    public void BTN_PausarJuego() => ChangeState(new PauseState(this));

    public void BTN_IrARepasoTutorial()
    {
        ChangeState(new GameplayState(this));
    }

    public void SetVolumen(float sliderValue)
    {
        float db = Mathf.Log10(sliderValue) * 20f;
        if (mainMixer != null) mainMixer.SetFloat("MusicVol", db);
    }

    public void SetBrillo(float sliderValue)
    {
        if (imagenBrillo != null)
        {
            Color c = imagenBrillo.color;
            c.a = 1f - sliderValue;
            imagenBrillo.color = c;
        }
    }

    public void ActualizarPaneo()
    {
        if (altavozPrincipal == null) return;

        bool izq = toggleAltavoz1.isOn;
        bool der = toggleAltavoz2.isOn;

        if (izq && der)
        {
            altavozPrincipal.mute = false;
            altavozPrincipal.panStereo = 0f;
        }
        else if (izq && !der)
        {
            altavozPrincipal.mute = false;
            altavozPrincipal.panStereo = -1f;
        }
        else if (!izq && der)
        {
            altavozPrincipal.mute = false;
            altavozPrincipal.panStereo = 1f;
        }
        else
        {
            altavozPrincipal.mute = true;
        }
    }
}

public class SplashState : GameState
{
    bool haPulsado = false;
    public SplashState(MenuController gc) : base(gc) { }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camSplash);
        if (_ctx.splashTextObject) _ctx.splashTextObject.SetActive(true);
        _ctx.StartCoroutine(FadeIn());
    }

    public override void Update()
    {
        if (!haPulsado && Input.anyKeyDown) _ctx.StartCoroutine(SecuenciaInicio());
    }

    public override void Exit()
    {
        if (_ctx.splashTextObject) _ctx.splashTextObject.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        if (_ctx.blackScreenFade != null)
        {
            _ctx.blackScreenFade.alpha = 1;
            while (_ctx.blackScreenFade.alpha > 0)
            {
                _ctx.blackScreenFade.alpha -= Time.deltaTime * 0.5f;
                yield return null;
            }
        }
    }

    IEnumerator SecuenciaInicio()
    {
        haPulsado = true;
        if (_ctx.audioSource) _ctx.audioSource.Play();
        if (_ctx.splashTextAnimator) _ctx.splashTextAnimator.SetTrigger("Confirm");

        yield return new WaitForSeconds(1.5f);

        _ctx.ChangeState(new NamingState(_ctx));
    }
}

public class NamingState : GameState
{
    public NamingState(MenuController gc) : base(gc) { }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camMainMenu);
        _ctx.panelNamingOverlay.SetActive(true);
        _ctx.btnAceptarNombre.onClick.AddListener(OnConfirmarNombre);
    }

    void OnConfirmarNombre()
    {
        if (!string.IsNullOrEmpty(_ctx.inputNombre.text))
        {
            _ctx.userConfig.nombreUsuario = _ctx.inputNombre.text;
            _ctx.ChangeState(new MainMenuState(_ctx));
        }
    }

    public override void Exit()
    {
        _ctx.panelNamingOverlay.SetActive(false);
        _ctx.btnAceptarNombre.onClick.RemoveListener(OnConfirmarNombre);
    }
}

public class MainMenuState : GameState
{
    public MainMenuState(MenuController gc) : base(gc) { }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camMainMenu);

        _ctx.panelMenuPrincipal.SetActive(true);
        if (_ctx.panelGameHUD)
        {
            _ctx.panelGameHUD.SetActive(false);
            _ctx.panelFinal.SetActive(false);
        }

        if (_ctx.textoBienvenida != null && _ctx.userConfig != null)
        {
            _ctx.textoBienvenida.text = $"Hola, {_ctx.userConfig.nombreUsuario}";
        }

        if (_ctx.panelPausa) _ctx.panelPausa.SetActive(false);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(true);
        if (_ctx.panelContenidoOpciones) _ctx.panelContenidoOpciones.SetActive(false);

        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(true);
        if (_ctx.panelContenidoCreditos) _ctx.panelContenidoCreditos.SetActive(false);

        if (_ctx.panelBtnAgradecimientos) _ctx.panelBtnAgradecimientos.SetActive(true);
        if (_ctx.panelContenidoAgradecimientos) _ctx.panelContenidoAgradecimientos.SetActive(false);
    }
}

public class GameplayState : GameState
{
    public GameplayState(MenuController gc) : base(gc) { }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camPlayMenu);

        if (_ctx.panelMenuPrincipal) _ctx.panelMenuPrincipal.SetActive(false);
        if (_ctx.panelPausa) _ctx.panelPausa.SetActive(false);
        if (_ctx.panelGameHUD) _ctx.panelGameHUD.SetActive(true);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(false);
        if (_ctx.panelContenidoOpciones) _ctx.panelContenidoOpciones.SetActive(false);

        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(false);
        if (_ctx.panelContenidoCreditos) _ctx.panelContenidoCreditos.SetActive(false);

        if (_ctx.panelBtnAgradecimientos) _ctx.panelBtnAgradecimientos.SetActive(false);
        if (_ctx.panelContenidoAgradecimientos) _ctx.panelContenidoAgradecimientos.SetActive(false);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) _ctx.ChangeState(new PauseState(_ctx));
    }
}

public class PauseState : GameState
{
    public PauseState(MenuController gc) : base(gc) { }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camPlayMenu);

        if (_ctx.panelGameHUD) _ctx.panelGameHUD.SetActive(false);
        _ctx.panelMenuPrincipal.SetActive(false);
        _ctx.panelPausa.SetActive(true);

        Time.timeScale = 0;
    }

    public override void Exit() { Time.timeScale = 1; }
}

public class OptionsState : GameState
{
    public GameState previousState;
    public OptionsState(MenuController gc, GameState prev) : base(gc) { previousState = prev; }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camOptionsMenu);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(false);
        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(false);
        if (_ctx.panelBtnAgradecimientos) _ctx.panelBtnAgradecimientos.SetActive(false);

        if (_ctx.panelContenidoOpciones) _ctx.panelContenidoOpciones.SetActive(true);
    }

    public override void Exit()
    {
        if (_ctx.panelContenidoOpciones) _ctx.panelContenidoOpciones.SetActive(false);
    }
}

public class CreditsState : GameState
{
    public GameState previousState;
    public CreditsState(MenuController gc, GameState prev) : base(gc) { previousState = prev; }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camCreditsMenu);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(false);
        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(false);
        if (_ctx.panelBtnAgradecimientos) _ctx.panelBtnAgradecimientos.SetActive(false);

        if (_ctx.panelContenidoCreditos) _ctx.panelContenidoCreditos.SetActive(true);
    }

    public override void Exit()
    {
        if (_ctx.panelContenidoCreditos) _ctx.panelContenidoCreditos.SetActive(false);
    }
}

public class AgradecimientosState : GameState
{
    public GameState previousState;
    public AgradecimientosState(MenuController gc, GameState prev) : base(gc) { previousState = prev; }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camCreditsMenu);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(false);
        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(false);
        if (_ctx.panelBtnAgradecimientos) _ctx.panelBtnAgradecimientos.SetActive(false);

        if (_ctx.panelContenidoAgradecimientos) _ctx.panelContenidoAgradecimientos.SetActive(true);
    }

    public override void Exit()
    {
        if (_ctx.panelContenidoAgradecimientos) _ctx.panelContenidoAgradecimientos.SetActive(false);
    }
}