using UnityEngine;
using System.Collections.Generic;

public class Cuisine : MonoBehaviour
{
    public static Cuisine Instance; // Singleton pour accès global

    [System.Serializable]
    public struct UITo3DLink
    {
        public DragAndDropWith3D uiElement;
        public Transform object3D;
    }

    [SerializeField] private Transform objectContainer; // Parent des objets 3D

    private Dictionary<DragAndDropWith3D, Transform> uiTo3DMap = new Dictionary<DragAndDropWith3D, Transform>();

    private void Awake()
    {
        Instance = this;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("CuisineObject");
        
        //Relier les enfants su objectContainer aux objets 3D. Les objets et UI ont le même numéro dans leur nom
        foreach (Transform child in objectContainer)
        {
            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains(child.name))
                {
                    uiTo3DMap.Add(child.GetComponent<DragAndDropWith3D>(), obj.transform);
                    // print les objets reliés
                    break;
                }
            }
        }
        

    }

    public Transform Get3DObject(DragAndDropWith3D uiElement)
    {
        return uiTo3DMap.ContainsKey(uiElement) ? uiTo3DMap[uiElement] : null;
    }

    public void SwapObjects(DragAndDropWith3D uiA, DragAndDropWith3D uiB)
    {
        Transform objA = Get3DObject(uiA);
        Transform objB = Get3DObject(uiB);

        if (objA == null || objB == null) return;

        Vector3 tempPos = objA.position;
        objA.position = objB.position;
        objB.position = tempPos;

        // Met à jour le dictionnaire
       /* uiTo3DMap[uiA] = objB;
        uiTo3DMap[uiB] = objA;*/
    }

    public Dictionary<DragAndDropWith3D, Transform> GetUITo3DMap()
    {
        return uiTo3DMap;
    }
}