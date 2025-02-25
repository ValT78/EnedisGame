using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    [SerializeField] private int hour = 7;
    [SerializeField] private int minute = 0;
    [SerializeField] private float changeHourDuration = 2f;
    [SerializeField] private float time = 420f;
    [SerializeField] private Image imageHour;
    [SerializeField] private Image imageMinute;

    [SerializeField] private int newHourTest;
    [SerializeField] private int newMinuteTest;

    float maxTime = 24 * 60f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DisplayHour();
        UpdateTime(time);
    }

    IEnumerator ChangeHourCoroutine(int newHour, int newMinute)
    {
        float t = 0;
        float currentTime = time;
        float targetTime = newHour * 60f + newMinute;
        if (currentTime > targetTime)
        {
            targetTime += maxTime;
        }
        while (t<1)
        {
            t += Time.deltaTime/changeHourDuration;
            float newtime = Mathf.Lerp(currentTime, targetTime, t);
            UpdateTime(newtime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void UpdateTime(float newTime)
    {
        //Update time, hour and minute
        time = newTime % maxTime;
        hour = Mathf.FloorToInt(time)/60;
        minute = Mathf.FloorToInt(time) - hour * 60;
    }

    private void DisplayHour()
    {
        float t_hour = time / 60f;
        float t_minute = time - hour * 60;
        float angleMax = 720;
        float angleHour = angleMax - (angleMax * t_hour / 24f);
        imageHour.transform.rotation = Quaternion.Euler(0, 0, angleHour);
        float angleMinute = angleMax - (angleMax * t_minute / 60f);
        imageMinute.transform.rotation = Quaternion.Euler(0, 0, angleMinute);
    }

    public float getTime()
    {
        return time;
    }

    public void changeHour(int newHour, int newMinute, float transitionDuration)
    {
        changeHourDuration = transitionDuration;
        StartCoroutine(ChangeHourCoroutine(newHour, newMinute));
    }
}
