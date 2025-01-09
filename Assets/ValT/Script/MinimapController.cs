using UnityEngine;

public class MinimapPlayerIcon : MonoBehaviour
{
    private Transform player; // Référence vers le joueur dans la scène
    public RectTransform minimapIcon; // L'icône du joueur sur la minimap
    public RectTransform minimapRect; // Le RectTransform de la RawImage minimap
    public Camera minimapCamera; // La caméra utilisée pour la minimap

    private void Start()
    {
        player = GameManager.Instance.playerController.transform;
    }

    private void Update()
    {
        // 1. Obtenir la position du joueur dans l'espace du monde
        Vector3 playerWorldPos = player.position;

        // 2. Convertir la position du joueur en coordonnées de la caméra minimap
        Vector3 cameraRelativePos = minimapCamera.WorldToViewportPoint(playerWorldPos);

        // 3. Vérifier si le joueur est visible par la minimapCamera
        if (cameraRelativePos.z < 0)
        {
            // Si le joueur est hors champ, cacher l'icône (optionnel)
            minimapIcon.gameObject.SetActive(false);
            return;
        }
        minimapIcon.gameObject.SetActive(true);

        // 4. Mapper les coordonnées de la caméra (Viewport) au RectTransform de la minimap
        Vector2 minimapSize = minimapRect.sizeDelta;
        Vector2 playerMinimapPos = new Vector2(
            (cameraRelativePos.x - 0.5f) * minimapSize.x,
            (cameraRelativePos.y - 0.5f) * minimapSize.y
        );

        // 5. Appliquer la position calculée à l'icône du joueur
        minimapIcon.anchoredPosition = playerMinimapPos;
    }
}
