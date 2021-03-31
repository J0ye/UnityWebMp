using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVCamera : MonoBehaviour
{
    [Range(1f, 10f)]
    public float sensitivity = 3f;
    [Range(1f, 10f)]
    public float smooth = 2f;

    private Vector2 mouseOrientation;
    private Vector2 smoothedVector;
    private Vector2 mouseLook;

    // Update is called once per frame
    void Update()
    {
        // Calculation of camera orientation
        mouseOrientation = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseOrientation = Vector2.Scale(mouseOrientation, new Vector2(sensitivity * smooth, sensitivity * smooth));
        smoothedVector = new Vector2(Mathf.Lerp(smoothedVector.x, mouseOrientation.x, 1.0f / smooth),
                                        Mathf.Lerp(smoothedVector.y, mouseOrientation.y, 1.0f / smooth));
        mouseLook += smoothedVector;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90, 90);

        // Translation of camera orientation
        transform.rotation = Quaternion.Euler(-mouseLook.y, mouseLook.x, 0);
    }
}
