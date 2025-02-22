using UnityEngine;
using UnityEngine.Video;

public class TVController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Renderer screenRenderer;
    public Material screenOffMaterial; // Matériau éteint (ex: noir)
    public Material screenOnMaterial; // Matériau actif

    private bool isOn = false;

    void Start()
    {
        TurnOnTV(); // Commence éteint
    }

    public void ToggleTV()
    {
        if (isOn)
            TurnOffTV();
        else
            TurnOnTV();
    }

    void TurnOnTV()
    {
        isOn = true;
        videoPlayer.Play();
        screenRenderer.material = screenOnMaterial; // Change le matériau pour l'affichage vidéo
    }

    void TurnOffTV()
    {
        isOn = false;
        videoPlayer.Stop();
        screenRenderer.material = screenOffMaterial; // Affiche un écran noir ou statique
    }
}
