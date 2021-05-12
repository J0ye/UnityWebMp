using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float seconds = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        yield return new WaitForSeconds(seconds);

        Destroy(gameObject);
    }

}
