using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TriggerEnterStorage : TriggerEnterDetector, IPointerClickHandler
{
    [Header("Dishwasher Settings")]
    [SerializeField] private Transform spawnArea; // Zone où les assiettes réapparaissent
    [SerializeField] private Sprite[] dishwasherSprites; // Sprites selon le nombre d'assiettes
    [SerializeField] private UnityEngine.UI.Image dishwasherImage; // Image UI du lave-vaisselle

    private List<GameObject> storedObjects = new(); // Stocke les assiettes

    protected override void Awake()
    {
        base.Awake();
        if (dishwasherImage == null)
        {
            dishwasherImage = GetComponent<UnityEngine.UI.Image>();
        }
    }

    public static void RegisterDraggable(DragAndDropObject obj)
    {
        if (!draggableObjects.Contains(obj))
            draggableObjects.Add(obj);
    }

    public static void UnregisterDraggable(DragAndDropObject obj)
    {
        if (draggableObjects.Contains(obj))
            draggableObjects.Remove(obj);
    }

    public void TryStoreObject(DragAndDropObject dragObject)
    {
        if (storedObjects.Contains(dragObject.gameObject)) return;

        RectTransform plateRect = dragObject.GetComponent<RectTransform>();
        if (IsRectTransformFullyInside(rectTransform, plateRect))
        {
            storedObjects.Add(dragObject.gameObject);
            dragObject.gameObject.SetActive(false); // Désactive l'assiette
            UpdateDishwasherSprite();
        }
    }

    public void ReleaseLastPlate()
    {
        if (storedObjects.Count > 0)
        {
            GameObject lastPlate = storedObjects[storedObjects.Count - 1];
            storedObjects.RemoveAt(storedObjects.Count - 1);

            Vector3 randomOffset = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
            lastPlate.transform.position = spawnArea.position + randomOffset;
            lastPlate.SetActive(true);

            UpdateDishwasherSprite();
        }
    }

    private void UpdateDishwasherSprite()
    {
        if (dishwasherSprites.Length < 3) return;

        if (storedObjects.Count >= 6)
            dishwasherImage.sprite = dishwasherSprites[2];
        else if (storedObjects.Count >= 5)
            dishwasherImage.sprite = dishwasherSprites[1];
        else if (storedObjects.Count >= 1)
            dishwasherImage.sprite = dishwasherSprites[0];
    }

    // Gère le clic sur l'élément UI
    public void OnPointerClick(PointerEventData eventData)
    {
        ReleaseLastPlate();
    }
}
