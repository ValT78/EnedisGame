using UnityEngine;

public class ScissorsObject : DragAndDropObject
{
    [Header("Scissors Settings")]
    [SerializeField] private RectTransform cutPoint; // Point rouge pour découper

    public RectTransform CutPoint => cutPoint; // Accès au point de découpe
}