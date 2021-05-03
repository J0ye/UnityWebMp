using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleUI : MonoBehaviour
{
    public List<GameObject> elements = new List<GameObject>();
    
    public void SwitchActiveState(int i)
    {
        if (i >= elements.Count)
        {
            Debug.LogError("Index of target element is no in the list of elements.");
            return;
        }

        elements[i].SetActive(!elements[i].activeSelf);
    }
}
