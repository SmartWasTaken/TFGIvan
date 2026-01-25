using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public abstract class GameState
{
    protected GameController _ctx;
    public GameState(GameController gc) { _ctx = gc; }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

public class GameController : MonoBehaviour
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

    [Header("Monitor Izquierda (Opciones)")]
    public GameObject panelBtnOpciones;
    public GameObject panelContenidoOpciones;

    [Header("Monitor Derecha (Créditos)")]
    public GameObject panelBtnCreditos;
    public GameObject panelContenidoCreditos;

    private GameState currentState;

    void Start()
    {
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

    public void BTN_Jugar() => ChangeState(new GameplayState(this));

    public void BTN_IrAOpciones() => ChangeState(new OptionsState(this, currentState));
    public void BTN_IrACreditos() => ChangeState(new CreditsState(this, currentState));

    public void BTN_VolverAtras()
    {
        if (currentState is OptionsState ops) ChangeState(ops.previousState);
        else if (currentState is CreditsState creds) ChangeState(creds.previousState);
        else ChangeState(new MainMenuState(this));
    }

    public void BTN_SalirJuego() => Application.Quit();
    public void BTN_Reanudar() => ChangeState(new GameplayState(this));
    public void BTN_MenuDesdeJuego() => ChangeState(new MainMenuState(this));
    public void BTN_PausarJuego() => ChangeState(new PauseState(this));
}

public class SplashState : GameState
{
    bool haPulsado = false;
    public SplashState(GameController gc) : base(gc) { }

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

        _ctx.ChangeState(new MainMenuState(_ctx));
    }
}

public class MainMenuState : GameState
{
    public MainMenuState(GameController gc) : base(gc) { }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camMainMenu);

        _ctx.panelMenuPrincipal.SetActive(true);
        if (_ctx.panelPausa) _ctx.panelPausa.SetActive(false);
        if (_ctx.panelGameHUD) _ctx.panelGameHUD.SetActive(false);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(true);
        if (_ctx.panelContenidoOpciones) _ctx.panelContenidoOpciones.SetActive(false);

        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(true);
        if (_ctx.panelContenidoCreditos) _ctx.panelContenidoCreditos.SetActive(false);
    }
}

public class GameplayState : GameState
{
    public GameplayState(GameController gc) : base(gc) { }

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
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) _ctx.ChangeState(new PauseState(_ctx));
    }
}

public class PauseState : GameState
{
    public PauseState(GameController gc) : base(gc) { }

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
    public OptionsState(GameController gc, GameState prev) : base(gc) { previousState = prev; }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camOptionsMenu);

        if (_ctx.panelBtnOpciones) _ctx.panelBtnOpciones.SetActive(false);
        if (_ctx.panelContenidoOpciones) _ctx.panelContenidoOpciones.SetActive(true);
    }
}

public class CreditsState : GameState
{
    public GameState previousState;
    public CreditsState(GameController gc, GameState prev) : base(gc) { previousState = prev; }

    public override void Enter()
    {
        _ctx.camManager.CamActivate(_ctx.camManager.camCreditsMenu);

        if (_ctx.panelBtnCreditos) _ctx.panelBtnCreditos.SetActive(false);
        if (_ctx.panelContenidoCreditos) _ctx.panelContenidoCreditos.SetActive(true);
    }
}
