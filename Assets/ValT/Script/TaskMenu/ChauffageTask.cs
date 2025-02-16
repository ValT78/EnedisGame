using UnityEngine;

public class ChauffageTask : GeneralTaskMenu
{
    [SerializeField] private RotatingDial valueGetter; // L'événement déclenché

    private bool firstTime = true;

    private void Update()
    {
        if(firstTime && valueGetter.GetValue() >= 25)
        {
            firstTime = false;
            CompleteTask();
        }
    }
}
