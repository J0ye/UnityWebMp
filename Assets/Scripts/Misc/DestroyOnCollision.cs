using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyOnCollision : MonoBehaviour
{
    public UnityEvent OnDestroy;
    public void OnCollisionEnter(Collision other)
    {
        OnDestroy.Invoke();
        Destroy(gameObject);
    }
}
