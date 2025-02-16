using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class CurtainObject : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform curtainRect; // RectTransform du rideau
    private ScissorsObject scissors; // Ciseaux uniques
    private Canvas parentCanvas; // Canvas UI parent

    [Header("Cut Settings")]
    [SerializeField] private float cutStep = 10f; // Hauteur coupée par passage
    private float lastCutY; // Dernière hauteur coupée

    [Header("Cut Effect Settings")]
    [SerializeField] private GameObject cutMarkerPrefab; // Prefab du point rouge
    [SerializeField] private GameObject cutPiecePrefab; // Prefab des morceaux de rideau
    [SerializeField] private float minPieceSize = 40f;
    [SerializeField] private float maxPieceSize = 120f;
    [SerializeField] private float minEjectionForce = 1000f;
    [SerializeField] private float maxEjectionForce = 1000f;
    [SerializeField] private float lifetime = 3f; // Temps avant destruction des morceaux
    [SerializeField] private float gravity = 10f; // Gravité simulée

    private void Start()
    {
        // Trouver les ciseaux une seule fois
        scissors = FindAnyObjectByType<ScissorsObject>();
        lastCutY = float.MaxValue; // Initialisation haute

        // Trouver dynamiquement le Canvas parent
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("CurtainObject: Aucun Canvas parent trouvé !");
        }
    }

    private void Update()
    {
        DetectScissors();
    }

    private void DetectScissors()
    {
        Vector2 localCutPoint = ConvertToLocalPosition(scissors.CutPoint.position);

        if (IsInsideCurtain(localCutPoint))
        {
            CutCurtain(localCutPoint);
            lastCutY = localCutPoint.y; // Mise à jour de la dernière coupe
        }
    }

    private void CutCurtain(Vector2 cutPoint)
    {
        // Réduit la hauteur du rideau uniquement par le bas
        float newHeight = Mathf.Max(0, curtainRect.sizeDelta.y - cutStep);

        // Ajuste la position Y pour éviter que le haut du rideau bouge
        curtainRect.anchoredPosition += new Vector2(0, cutStep / 2);
        curtainRect.sizeDelta = new Vector2(curtainRect.sizeDelta.x, newHeight);

        // Génération de morceaux de rideau coupés
        GenerateCutPieces(cutPoint);
    }

    private void GenerateCutPieces(Vector2 cutPoint)
    {
        if (cutPiecePrefab == null || parentCanvas == null) return;

        int pieceCount = Random.Range(2, 5); // Nombre aléatoire de morceaux
        for (int i = 0; i < pieceCount; i++)
        {
            GameObject piece = Instantiate(cutPiecePrefab, parentCanvas.transform);
            RectTransform pieceRect = piece.GetComponent<RectTransform>();

            // Définir une taille aléatoire
            float width = Random.Range(minPieceSize, maxPieceSize);
            float height = Random.Range(minPieceSize, maxPieceSize);
            pieceRect.sizeDelta = new Vector2(width, height);

            // Positionner au point de découpe
            pieceRect.anchoredPosition = cutPoint;
            print(cutPoint);

            // Ajouter un mouvement aléatoire
            float generateForce = Random.Range(minEjectionForce, maxEjectionForce);
            float generateForce2 = Random.Range(minEjectionForce, maxEjectionForce);
            //Générer 1 ou -1 aléatoirement
            float ejectionForce = Random.Range(0, 2) == 0 ? generateForce : -generateForce;
            float ejectionForce2 = Random.Range(0, 2) == 0 ? generateForce2 : -generateForce2;

            Vector2 force = new Vector2(
                ejectionForce,
                ejectionForce2
            );
            StartCoroutine(AnimatePiece(pieceRect, force));
        }
    }

    private IEnumerator AnimatePiece(RectTransform piece, Vector2 force)
    {
        float elapsedTime = 0f;
        Vector2 velocity = force / 10f; // Vitesse initiale

        while (elapsedTime < lifetime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            // Appliquer la gravité
            velocity.y -= gravity * Time.deltaTime;

            // Déplacer le morceau
            piece.anchoredPosition += velocity * Time.deltaTime;

            // Vérifier la sortie de l'écran
            /*if (IsOutOfScreen(piece))
            {
                Destroy(piece.gameObject);
                yield break;
            }*/

        }

        Destroy(piece.gameObject);
    }

    private bool IsOutOfScreen(RectTransform piece)
    {
        Vector3 screenPos = piece.position;
        return screenPos.y < -Screen.height / 2 || screenPos.x < -Screen.width / 2 || screenPos.x > Screen.width / 2;
    }

    private Vector2 ConvertToLocalPosition(Vector3 worldPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            worldPosition,
            null,
            out Vector2 localPoint
        );
        return localPoint;
    }

    private bool IsInsideCurtain(Vector2 position)
    {
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();

        // Récupérer les coins du rideau en coordonnées mondiales
        Vector3[] worldCorners = new Vector3[4];
        curtainRect.GetWorldCorners(worldCorners);

        // Convertir ces coins en coordonnées locales du Canvas
        Vector2 bottomLeft, topRight;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, worldCorners[0], null, out bottomLeft);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, worldCorners[2], null, out topRight);

        // Debug pour vérifier les valeurs
        print($"Rideau BottomLeft: {bottomLeft}, TopRight: {topRight}");
        print($"CutPoint: {position}");

        // Vérifier si le point est à l'intérieur des limites
        return position.x >= bottomLeft.x &&
               position.x <= topRight.x &&
               position.y >= bottomLeft.y &&
               position.y <= topRight.y;
    }
}
