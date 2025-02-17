using UnityEngine;
using UnityEngine.EventSystems;

public class RotatingDial : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform dialImage; // L'image du bouton rotatif
    [SerializeField] private float minAngle = 0f; // Angle minimum
    [SerializeField] private float maxAngle = 270f; // Angle maximum
    [SerializeField] private float rotationSpeed = 1.5f; // Facteur de vitesse (plus grand = plus rapide)
    [SerializeField] private int currentValue; // Valeur actuelle du bouton (0 à 100 %)
    [SerializeField] private int minValue; // Valeur minimale
    [SerializeField] private int maxValue; // Valeur maximale

    private float currentAngle = 0f; // Angle actuel du bouton
    private Vector2 centerPosition; // Centre du bouton
    private Vector2 previousMousePosition; // Position de la souris lors du dernier frame

    private void Start()
    {
        // Calculer la position centrale du bouton
        centerPosition = dialImage.position;

        // Calculer l'angle initial
        currentAngle = Mathf.Lerp(minAngle, maxAngle, (float)(currentValue - minValue) / (float)(maxValue - minValue));
        dialImage.rotation = Quaternion.Euler(0, 0, currentAngle);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Stocker la position actuelle de la souris
        previousMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculer la position actuelle de la souris
        Vector2 currentMousePosition = eventData.position;

        // Vecteurs de direction entre la souris et le centre
        Vector2 previousDirection = previousMousePosition - centerPosition;
        Vector2 currentDirection = currentMousePosition - centerPosition;

        // Calculer le produit vectoriel pour déterminer le sens de rotation
        float crossProduct = previousDirection.x * currentDirection.y - previousDirection.y * currentDirection.x;

        // Calculer la différence d'angle
        float angleDelta = Vector2.Angle(previousDirection, currentDirection);

        // Appliquer le signe basé sur le produit vectoriel
        angleDelta *= Mathf.Sign(crossProduct);

        // Appliquer la vitesse de rotation
        angleDelta *= rotationSpeed;

        // Mettre à jour l'angle actuel avec des limites
        currentAngle = Mathf.Clamp(currentAngle + angleDelta, minAngle, maxAngle);

        // Appliquer la rotation au bouton
        dialImage.rotation = Quaternion.Euler(0, 0, currentAngle);

        currentValue = (int)(Mathf.Lerp(minValue, maxValue, (currentAngle - minAngle) / (maxAngle - minAngle)));

        // Mettre à jour la position précédente de la souris
        previousMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Rien à faire pour l'instant, mais tu peux ajouter des effets si nécessaire
    }

    public int GetValue()
    {
        return currentValue;
    }
}
