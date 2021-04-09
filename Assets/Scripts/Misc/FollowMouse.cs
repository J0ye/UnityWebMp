using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;

public class FollowMouse : MonoBehaviour
{
    private Image crosshair;

    private void Awake()
    {
        crosshair = GetComponent<Image>();
    }

    private void Update()
    {
        if (crosshair != null)
        {
            crosshair.transform.position = Input.mousePosition;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
}
