using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ObjectEventHandler : MonoBehaviour
{
    public string targetTag = "";
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTriggerStay;
    public UnityEvent onCollisionEnter;
    public UnityEvent onCollisionExit;
    public UnityEvent onCollisionStay;
    // Start is called before the first frame update
    void Start()
    {
        StartEvent(onTriggerEnter);
        StartEvent(onTriggerExit);
        StartEvent(onTriggerStay);
        StartEvent(onCollisionEnter);
        StartEvent(onCollisionExit);
        StartEvent(onCollisionStay);
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (string.IsNullOrEmpty(targetTag))
        {
            onTriggerEnter.Invoke();
        }
        else if (other.CompareTag(targetTag))
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onTriggerEnter.Invoke();
        }
        else if (other.CompareTag(targetTag))
        {
            onTriggerEnter.Invoke();
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onTriggerExit.Invoke();
        }
        else if (other.CompareTag(targetTag))
        {
            onTriggerExit.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    { 
        if(string.IsNullOrEmpty(targetTag))
        {
            onTriggerExit.Invoke();
        }
        else if (other.CompareTag(targetTag))
        {
            onTriggerExit.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onTriggerStay.Invoke();
        }
        else if (other.CompareTag(targetTag))
        {
            onTriggerStay.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onTriggerStay.Invoke();
        }
        else if (other.CompareTag(targetTag))
        {
            onTriggerStay.Invoke();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onCollisionEnter.Invoke();
        }
        else if (other.gameObject.CompareTag(targetTag))
        {
            onCollisionEnter.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onCollisionEnter.Invoke();
        }
        else if (other.gameObject.CompareTag(targetTag))
        {
            onCollisionEnter.Invoke();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onCollisionExit.Invoke();
        }
        else if (other.gameObject.CompareTag(targetTag))
        {
            onCollisionExit.Invoke();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onCollisionExit.Invoke();
        }
        else if (other.gameObject.CompareTag(targetTag))
        {
            onCollisionExit.Invoke();
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onCollisionStay.Invoke();
        }
        else if (other.gameObject.CompareTag(targetTag))
        {
            onCollisionStay.Invoke();
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            onCollisionStay.Invoke();
        }
        else if (other.gameObject.CompareTag(targetTag))
        {
            onCollisionStay.Invoke();
        }
    }

    private void StartEvent(UnityEvent eve)
    {
        if (eve == null)
        {
            eve = new UnityEvent();
        }
    }
}
