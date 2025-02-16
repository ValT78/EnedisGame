using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropWith3D : DragAndDropObject
{
    private DragAndDropWith3D otherObject; // Objet avec lequel on va échanger
    private Vector2 otherObjectPosition;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        // Vérifie si on est à la même hauteur qu'un autre objet
        foreach (var obj in Cuisine.Instance.GetUITo3DMap().Keys)
        {
            if (obj != this && Mathf.Abs(obj.rectTransform.anchoredPosition.x - rectTransform.anchoredPosition.x) < 50f)
            {
                otherObject = obj;
                otherObjectPosition = otherObject.rectTransform.anchoredPosition;
                otherObject.rectTransform.anchoredPosition = originalPosition;
                originalPosition = otherObjectPosition;
                Cuisine.Instance.SwapObjects(this, otherObject);
                otherObject = null; // Reset de la référence
                break;
            }
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        ResetPosition();

        
        
    }
}
