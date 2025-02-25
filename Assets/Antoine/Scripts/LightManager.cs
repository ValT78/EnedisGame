using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private GameObject light;
    [SerializeField] private GameObject clock;
    [SerializeField] private float rotDebutJour = 0;
    [SerializeField] private float rotMilieuJour = 90f;
    [SerializeField] private float rotFinJour = 180;
    [SerializeField] private float rotFinNuit = 360;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float time = clock.GetComponent<ClockManager>().getTime();
        RotateLight(time);
    }

    private void RotateLight(float time)
    {
        float tFinNuit = 6 * 60f;
        float tDebutNuit = 20 * 60f;
        float tPicLuminosite = 13 * 60f;
        float tMinuit = 24 * 60f;
        float rotLight = 0;
        if ((time<=tFinNuit)||(time>tDebutNuit))
        //Durant la nuit, luminosité minimal
        {
            float t = (time - tDebutNuit + indicatrice(time)*tMinuit)/(tMinuit - tDebutNuit + tFinNuit);
            rotLight = Mathf.Lerp(rotFinJour, rotFinNuit, t);
        } else if (time<=tPicLuminosite)
        //Entre la fin de la nuit et le pic de luminosité, croissance de la luminosité minimale vers la luminosité maximale
        {
            float t = (time - tFinNuit) / (tPicLuminosite - tFinNuit);
            rotLight = Mathf.Lerp(rotDebutJour, rotMilieuJour, t);
        } else if (time>tPicLuminosite)
        //L'inverse entre le pic de luminosité et le début de la nuit
        {
            float t = (time - tPicLuminosite) / (tDebutNuit - tPicLuminosite);
            rotLight = Mathf.Lerp(rotMilieuJour, rotFinJour, t);
        }
        light.transform.rotation = Quaternion.Euler(rotLight, 0, 0);
    }

    private float indicatrice(float time)
    {
        float tDebutNuit = 20 * 60f;
        if (time<tDebutNuit)
        {
            return 1;
        } else
        {
            return 0;
        }
    }
}
