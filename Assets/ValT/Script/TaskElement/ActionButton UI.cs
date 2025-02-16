using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButtonUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private RectTransform uiElementPrefab; // Le prefab de l'élément UI à instancier
    [SerializeField] private RectTransform parentEmpty; // L'empty servant de conteneur pour l'élément

    private RectTransform currentUIElement = null; // Référence à l'élément actuellement instancié

    /// <summary>
    /// Méthode appelée lorsque le bouton est cliqué.
    /// </summary>
    public void SpawnUIElement()
    {
        if (uiElementPrefab == null || parentEmpty == null)
        {
            Debug.LogError("Le prefab ou le conteneur parent (Empty) n'est pas assigné !");
            return;
        }

        // Si un élément est déjà instancié, empêcher d'en créer un nouveau
        if (currentUIElement != null)
        {
            Debug.Log("Un élément est déjà instancié. Détruisez-le avant d'en créer un nouveau.");
            return;
        }

        // Instancie l'élément UI en tant qu'enfant de l'empty
        currentUIElement = Instantiate(uiElementPrefab, parentEmpty);

        // Place le nouvel élément au même endroit que le bouton
        if (!TryGetComponent<RectTransform>(out var buttonRectTransform))
        {
            Debug.LogError("Le script UIButtonSpawner doit être attaché à un bouton avec un RectTransform !");
            return;
        }

        // Copie la position et la taille du bouton dans le nouvel élément
        currentUIElement.anchorMin = buttonRectTransform.anchorMin;
        currentUIElement.anchorMax = buttonRectTransform.anchorMax;
        currentUIElement.anchoredPosition = buttonRectTransform.anchoredPosition;
        currentUIElement.sizeDelta = buttonRectTransform.sizeDelta;

        // Assure que le nouvel élément est rendu au-dessus dans le moteur de rendu
        currentUIElement.SetAsLastSibling();

        // Activer le suivi des clics pour détecter les clics en dehors de l'élément
        StartListeningForClicks();
    }

    /// <summary>
    /// Détruit l'élément actuellement instancié.
    /// </summary>
    private void DestroyCurrentUIElement()
    {
        if (currentUIElement != null)
        {
            Destroy(currentUIElement.gameObject);
            currentUIElement = null;
        }
    }

    /// <summary>
    /// Commence à écouter les clics pour détecter les clics en dehors de l'élément.
    /// </summary>
    private void StartListeningForClicks()
    {
        // Ajoute un listener pour détecter les clics globaux
        StartCoroutine(CheckForOutsideClicks());
    }

    /// <summary>
    /// Coroutine qui vérifie si un clic est en dehors de l'élément.
    /// </summary>
    private System.Collections.IEnumerator CheckForOutsideClicks()
    {
        while (currentUIElement != null)
        {
            // Si l'utilisateur clique
            if (Input.GetMouseButtonDown(0))
            {
                // Récupère l'élément UI cliqué
                GameObject clickedObject = GetClickedUIObject();

                // Si le clic est en dehors de l'élément instancié et de ses enfants
                if (clickedObject == null || !IsChildOf(currentUIElement.gameObject, clickedObject))
                {
                    DestroyCurrentUIElement(); // Détruire l'élément
                }
            }
            yield return null; // Attendre la prochaine frame
        }
    }

    /// <summary>
    /// Vérifie si un objet est un enfant (ou lui-même) d'un autre.
    /// </summary>
    private bool IsChildOf(GameObject parent, GameObject potentialChild)
    {
        Transform t = potentialChild.transform;
        while (t != null)
        {
            if (t.gameObject == parent)
                return true;
            t = t.parent;
        }
        return false;
    }

    /// <summary>
    /// Récupère l'objet UI cliqué.
    /// </summary>
    private GameObject GetClickedUIObject()
    {
        PointerEventData pointerData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            return raycastResults[0].gameObject;
        }
        return null;
    }
}
