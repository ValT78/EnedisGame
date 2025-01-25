using UnityEngine;
using UnityEngine.Events;

public class UIElementCollisionTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private RectTransform targetElement; // L'élément UI à surveiller
    [SerializeField] private bool triggerOnExit = false; // Déclenche l'événement lors de la sortie si activé

    [Header("Events")]
    [SerializeField] private UnityEvent onTrigger; // L'événement déclenché

    private RectTransform rectTransform; // Le RectTransform de cet objet
    private bool isTargetInside = false; // Indique si le targetElement est actuellement dans cet élément

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (targetElement == null)
        {
            Debug.LogError("L'élément cible (targetElement) n'est pas assigné !", this);
        }
    }

    private void Update()
    {
        if (targetElement == null || rectTransform == null)
            return;

        // Vérifie si le targetElement est entièrement à l'intérieur de cet élément
        bool isNowFullyInside = IsRectTransformFullyInside(rectTransform, targetElement);
        bool isNowFullyOutside = IsRectTransformFullyOutside(rectTransform, targetElement);

        if (isNowFullyInside && !isTargetInside)
        {
            isTargetInside = true;
            if (!triggerOnExit) // Déclenche l'événement si le mode est "entrée"
            {
                onTrigger?.Invoke();
            }
        }
        else if (isNowFullyOutside && isTargetInside)
        {
            isTargetInside = false;
            if (triggerOnExit) // Déclenche l'événement si le mode est "sortie"
            {
                onTrigger?.Invoke();
            }
        }

        // Si la souris est relâchée et que l'objet est à l'intérieur, recentre l'objet
        if (Input.GetMouseButtonUp(0) && isTargetInside)
        {
            CenterTargetInside();
        }
    }

    // Méthode pour centrer l'élément cible à l'intérieur de cet objet
    private void CenterTargetInside()
    {
        if (rectTransform != null && targetElement != null)
        {
            // Définit la position locale de l'objet cible comme étant au centre de cet objet
            targetElement.anchoredPosition = rectTransform.anchoredPosition;
        }
    }

    // Vérifie si rectA contient entièrement rectB
    private bool IsRectTransformFullyInside(RectTransform rectA, RectTransform rectB)
    {
        Vector3[] worldCornersA = new Vector3[4];
        Vector3[] worldCornersB = new Vector3[4];

        rectA.GetWorldCorners(worldCornersA);
        rectB.GetWorldCorners(worldCornersB);

        // Vérifie si tous les coins de rectB sont à l'intérieur de rectA
        for (int i = 0; i < 4; i++)
        {
            if (!IsPointInsideRectangle(worldCornersB[i], worldCornersA))
            {
                return false;
            }
        }

        return true;
    }

    // Vérifie si rectA ne contient aucun point de rectB
    private bool IsRectTransformFullyOutside(RectTransform rectA, RectTransform rectB)
    {
        Vector3[] worldCornersA = new Vector3[4];
        Vector3[] worldCornersB = new Vector3[4];

        rectA.GetWorldCorners(worldCornersA);
        rectB.GetWorldCorners(worldCornersB);

        // Vérifie si tous les coins de rectB sont à l'extérieur de rectA
        for (int i = 0; i < 4; i++)
        {
            if (IsPointInsideRectangle(worldCornersB[i], worldCornersA))
            {
                return false;
            }
        }

        return true;
    }

    // Vérifie si un point est à l'intérieur des coins d'un rectangle
    private bool IsPointInsideRectangle(Vector3 point, Vector3[] rectangleCorners)
    {
        return point.x >= rectangleCorners[0].x &&
               point.x <= rectangleCorners[2].x &&
               point.y >= rectangleCorners[0].y &&
               point.y <= rectangleCorners[2].y;
    }
}
