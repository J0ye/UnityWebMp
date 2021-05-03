using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPadInput : MonoBehaviour
{
    public Vector2 vector = Vector2.zero;
    public float x = 0;
    public float y = 0;

    public void PressUp()
    {
        y = 1;
        CalculateVector();
    }

    public void PressDown()
    {
        y = -1;
        CalculateVector();
    }

    public void PressLeft()
    {
        x = -1;
        CalculateVector();
    }

    public void PressRight()
    {
        x = 1;
        CalculateVector();
    }

    public void ReleaseButtons()
    {
        x = 0;
        y = 0;
        CalculateVector();
    }

    private void CalculateVector()
    {
        vector = new Vector2(x, y);
    }
}
