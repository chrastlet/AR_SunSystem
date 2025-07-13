using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GetBiggestPossible : MonoBehaviour
{
    public CustomSpawnPositions FindSpawnPositions;
    public GameObject[] SunSystemPrefab = new GameObject[10];
    public float maxsize;
    public float minsize;
    private MRUKRoom room;

    // Start is called before the first frame update
void Start()
{
    if (MRUK.Instance != null)
    {
        MRUK.Instance.RegisterSceneLoadedCallback(() =>
        {
            room = MRUK.Instance.GetCurrentRoom();
            Debug.Log("✅ MRUK scene and room fully loaded!");
            placeAllSizes();
        });
    }
}

    public void placeAllSizes()
    {
        for (int i =9; i>= 0;i--)
        {
            GameObject sunsystem = SunSystemPrefab[i];
            Debug.Log("Trying to place " + sunsystem);
            Debug.Log("Size" + sunsystem.transform.localScale.x + " " + sunsystem.transform.localScale.y + " " + sunsystem.transform.localScale.z);
            FindSpawnPositions.SpawnObject = sunsystem;
            
            FindSpawnPositions.StartSpawn(room);
            if (GameObject.FindWithTag("SunSystem")) break;

        }
}
        public Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }


}
