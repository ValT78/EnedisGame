using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Components")]
    protected RectTransform rectTransform; // Référence au RectTransform
    private Canvas canvas; // Canvas parent
    private CanvasGroup canvasGroup; // Pour gérer l'opacité

    [Header("Settings")]
    private readonly float minX = -960; // Limite minimale en X
    private readonly float maxX = 960; // Limite maximale en X
    private readonly float minY = -540; // Limite minimale en Y
    private readonly float maxY = 540; // Limite maximale en Y
    private readonly float dragSpeed = 20f; // Vitesse de déplacement

    protected Vector2 originalPosition; // Position d'origine pour reset si nécessaire
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

        
        TriggerEnterStorage.RegisterDraggable(this); // Enregistre l'objet au lave-vaisselle

    }

    private void OnDestroy()
    {
        TriggerEnterStorage.UnregisterDraggable(this); // Le retire du suivi
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // Sauvegarde la position d'origine de l'objet
        originalPosition = rectTransform.anchoredPosition;

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

    public virtual void OnDrag(PointerEventData eventData)
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

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // Rétablir l'apparence de l'objet
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        TriggerEnterStorage dishwasher = FindAnyObjectByType<TriggerEnterStorage>();
        if (dishwasher != null)
        {
            dishwasher.TryStoreObject(this);
        }
    }

    // Méthode pour réinitialiser l'objet à sa position d'origine
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}
