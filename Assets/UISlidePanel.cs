using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISlidePanel : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform panelRect;
    public Button botonToggle;

    [Header("Ajustes")]
    public float posicionCerradoX = 600f;
    public float duracion = 0.5f;

    private float posicionAbiertoX;
    private bool estaAbierto = true;

    void Start()
    {
        if (panelRect != null)
        {
            posicionAbiertoX = panelRect.anchoredPosition.x;
        }

        if (botonToggle != null)
        {
            botonToggle.onClick.AddListener(AlternarPanel);
        }
    }

    public void AlternarPanel()
    {
        float destinoX = estaAbierto ? posicionCerradoX : posicionAbiertoX;

        panelRect.DOAnchorPosX(destinoX, duracion).SetEase(Ease.InOutQuad);

        estaAbierto = !estaAbierto;
    }
}