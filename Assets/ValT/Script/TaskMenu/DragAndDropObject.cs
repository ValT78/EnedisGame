using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Components")]
    [SerializeField] private RectTransform rectTransform; // Référence au RectTransform
    [SerializeField] private Canvas canvas; // Canvas parent
    [SerializeField] private CanvasGroup canvasGroup; // Pour gérer l'opacité

    [Header("Settings")]
    [SerializeField] private float minX = -100f; // Limite minimale en X
    [SerializeField] private float maxX = 100f; // Limite maximale en X
    [SerializeField] private float minY = -100f; // Limite minimale en Y
    [SerializeField] private float maxY = 100f; // Limite maximale en Y
    [SerializeField] private float dragSpeed = 10f; // Vitesse de déplacement

    private Vector2 originalPosition; // Position d'origine pour reset si nécessaire
    private Vector2 dragOffset; // Décalage entre la souris et la position de l'objet

    private void Awake()
    {
        // Vérifie si les composants sont assignés dans l'inspecteur
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
        {
            Debug.LogError("L'objet doit être un enfant d'un Canvas !");
        }

        // Sauvegarde la position d'origine de l'objet
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Rendre l'objet semi-transparent pendant le drag
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }

        // Calcul du décalage entre la position de la souris et l'objet
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localMousePosition
        );

        dragOffset = rectTransform.anchoredPosition - localMousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Déplacer l'objet selon la position de la souris
        if (canvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localMousePosition
            );

            // Ajoute le décalage et applique une interpolation pour la vitesse
            Vector2 targetPosition = localMousePosition + dragOffset;
            targetPosition = new Vector2(
                Mathf.Clamp(targetPosition.x, minX, maxX),
                Mathf.Clamp(targetPosition.y, minY, maxY)
            );

            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPosition,
                Time.deltaTime * dragSpeed
            );
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Rétablir l'apparence de l'objet
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    // Méthode pour réinitialiser l'objet à sa position d'origine
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}
