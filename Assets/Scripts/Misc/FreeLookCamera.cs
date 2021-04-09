using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [Range(0.1f, 50f)]
    public float speed = 10f;
    [Range(2f, 50f)]
    public float sprintMultiplyer = 10f;
    [Range(0.01f, 10f)]
    public float sensitivity = 3f;
    [Range(0.001f, 10f)]
    public float smooth = 2f;

    protected Vector2 mouseOrientation;
    protected Vector2 smoothedVector;
    protected Vector2 mouseLook;
    protected float vertical = 0f;
    protected float horizontal = 0f;
    protected bool active = true;

    protected void Start() 
    { 
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    protected void Update()
    {
        if(active)
        {
            Sprint();

            // Horizontal movement
            Vector3 direction = CalculateFlyingVector();

            direction.y = AddVerticalMovement(direction.y);

            Move(direction);

            CalculateOrientation();          
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            UnlockMouse();
        }
    }

    protected virtual void CalculateOrientation()
    {
        // Calculation of camera orientation
        mouseOrientation = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseOrientation = Vector2.Scale(mouseOrientation, new Vector2(sensitivity * smooth, sensitivity * smooth));
        smoothedVector = new Vector2(Mathf.Lerp(smoothedVector.x, mouseOrientation.x, 1.0f / smooth), 
                                        Mathf.Lerp(smoothedVector.y, mouseOrientation.y, 1.0f / smooth));
        mouseLook += smoothedVector;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90, 90);

        // Translation of camera orientation
        transform.rotation = Quaternion.Euler(-mouseLook.y, mouseLook.x, transform.eulerAngles.z);   
    }

    protected virtual Vector3 CalculateFlyingVector()
    {
        Vector3 value = new Vector3(((Input.GetAxis("Horizontal") * speed) * Time.deltaTime), 0,(Input.GetAxis("Vertical") * speed) * Time.deltaTime);
        value = transform.TransformDirection(value);
        return value;
    }
    protected float AddVerticalMovement(float baseValue)
    {
        // Vertical movement
        if(Input.GetKey(KeyCode.Space))
        {
            return (1 *speed) * Time.deltaTime;
        }else if(Input.GetKey(KeyCode.LeftControl))
        {
            return ((1 *speed) * Time.deltaTime) * -1;
        }else 
        {
            return baseValue;
        }
    }

    protected virtual void Move(Vector3 direction)
    {
        transform.Translate(direction);
    }

    protected void Sprint()
    {
        // Activate sprint by pressing left shift
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= sprintMultiplyer;
        }
        
        // Deactivate sprint by releasing left shift
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= sprintMultiplyer;
        }
    }

    protected void UnlockMouse()
    {
		Cursor.lockState = CursorLockMode.None;
    }
}
