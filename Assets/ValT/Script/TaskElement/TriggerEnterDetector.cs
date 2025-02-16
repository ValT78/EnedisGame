using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool triggerOnExit = false; // Déclenche l'événement lors de la sortie si activé

    [Header("Events")]
    [SerializeField] private UnityEvent onTrigger; // Événement déclenché

    protected RectTransform rectTransform; // RectTransform de cet objet
    protected static List<DragAndDropObject> draggableObjects = new List<DragAndDropObject>(); // Tous les objets traçables

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Update()
    {
        // Vérifie tous les objets suivis
        foreach (DragAndDropObject obj in new List<DragAndDropObject>(draggableObjects))
        {
            RectTransform objRect = obj.GetComponent<RectTransform>();

            bool isNowFullyInside = IsRectTransformFullyInside(rectTransform, objRect);
            bool isNowFullyOutside = IsRectTransformFullyOutside(rectTransform, objRect);

            if (isNowFullyInside)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    CenterObjectInside(objRect);
                }
                if (!triggerOnExit)
                {
                    onTrigger?.Invoke();
                    
                }
            }
            else if (isNowFullyOutside)
            {
                if (triggerOnExit) onTrigger?.Invoke();
            }
        }

        // Si la souris est relâchée, centrer l'objet lâché s'il est dans la zone
        
    }

    // Centre le dernier objet lâché
    private void CenterObjectInside(RectTransform rect)
    {
            rect.anchoredPosition = rectTransform.anchoredPosition;
    }

    // Vérifie si rectA contient entièrement rectB
    protected bool IsRectTransformFullyInside(RectTransform rectA, RectTransform rectB)
    {
        Vector3[] worldCornersA = new Vector3[4];
        Vector3[] worldCornersB = new Vector3[4];

        rectA.GetWorldCorners(worldCornersA);
        rectB.GetWorldCorners(worldCornersB);

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
