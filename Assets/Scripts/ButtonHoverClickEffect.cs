using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonInteractive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    [Header("Composants")]
    public RectTransform rectTransform;      // Le RectTransform du bouton
    public Image overlayImage;               // Une image enfant semi-transparente pour la couleur

    [Header("Couleurs")]
    public Color normalColor = new Color(1, 1, 1, 0);   // transparent overlay par défaut
    public Color hoverColor = new Color(1f, 0.9f, 0.7f, 0.3f);
    public Color clickColor = new Color(1f, 0.8f, 0.5f, 0.5f);

    [Header("Scale")]
    public float scaleAmount = 1.05f;
    public float smoothSpeed = 10f;

    [Header("Tilt")]
    public float tiltAmount = 10f;
    public float tiltSmooth = 10f;

    private Vector3 initialScale;
    private Vector3 targetRotation;
    private bool isHovered = false;

    void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (overlayImage != null)
            overlayImage.color = normalColor;

        initialScale = transform.localScale;
    }

    void Update()
    {
        // Scale fluide (indépendant du timeScale)
        Vector3 targetScale = isHovered ? initialScale * scaleAmount : initialScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * smoothSpeed);

        // Rotation fluide (indépendant du timeScale)
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotation), Time.unscaledDeltaTime * tiltSmooth);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        SetOverlayColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        SetOverlayColor(normalColor);
        targetRotation = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetOverlayColor(clickColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetOverlayColor(hoverColor);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!isHovered) return;

        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localMousePos);

        Vector2 normalized = new Vector2(
            Mathf.Clamp(localMousePos.x / rectTransform.rect.width, -0.5f, 0.5f) * 2f,
            Mathf.Clamp(localMousePos.y / rectTransform.rect.height, -0.5f, 0.5f) * 2f
        );

        targetRotation = new Vector3(-normalized.y * tiltAmount, normalized.x * tiltAmount, 0);
    }

    private void SetOverlayColor(Color c)
    {
        if (overlayImage != null)
            overlayImage.color = c;
    }
}
