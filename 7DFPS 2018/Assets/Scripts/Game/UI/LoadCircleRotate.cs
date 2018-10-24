using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCircleRotate : MonoBehaviour
{
    public float speed = 100.0f;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, Time.unscaledTime * speed);
    }
}