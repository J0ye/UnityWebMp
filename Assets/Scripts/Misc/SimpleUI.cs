using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUI : MonoBehaviour
{
    public List<GameObject> elements = new List<GameObject>();
    [Header("Options")]
    public Color targetColor = new Color();

    protected List<Color> oldColors = new List<Color>();

    public virtual void Start()
    {
        foreach(GameObject obj in elements)
        {
            oldColors.Add(new Color());
        }
    }

    public void SwitchTextColor(int i)
    {
        if (!CheckListLength(i)) return;

        try
        {
            Text t = elements[i].GetComponent<Text>();
            if (t.color != targetColor)
            {
                // Switch to target color
                oldColors[i] = t.color;
                t.color = targetColor;
            } else
            {
                t.color = oldColors[i];
            }
        }
        catch(Exception e)
        {
            Debug.Log("Could not switch text color of " + elements[i] + ", because of " + e);
        }
    }

    public void SwitchButtonColor(int i)
    {
        if (!CheckListLength(i)) return;

        try
        {
            Button b = elements[i].GetComponent<Button>();
            if (b.colors.selectedColor != targetColor)
            {
                // Switch to target color
                oldColors[i] = b.colors.selectedColor;

                ColorBlock temp = b.colors;
                temp.selectedColor = targetColor;
                temp.normalColor = targetColor;

                b.colors = temp;
            }
            else
            {
                ColorBlock temp = b.colors;
                temp.selectedColor = oldColors[i];
                temp.normalColor = oldColors[i];

                b.colors = temp;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Could not switch button color of " + elements[i] + ", because of " + e);
        }
    }

    public void SwitchActiveState(int i)
    {
        if (!CheckListLength(i)) return;

        elements[i].SetActive(!elements[i].activeSelf);
    }

    protected bool CheckListLength(int i)
    {
        if (i >= elements.Count)
        {
            Debug.LogError("Index of target element is no in the list of elements.");
            return false;
        }

        return true;
    }
}
