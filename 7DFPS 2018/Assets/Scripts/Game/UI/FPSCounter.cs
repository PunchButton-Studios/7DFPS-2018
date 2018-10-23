using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;
    private float dt;

    private void Start() => StartCoroutine(UpdateCounter());

    private void Update()
    {
        dt += (Time.unscaledDeltaTime - dt) * 0.1f;
    }

    private IEnumerator UpdateCounter()
    {
        fpsText.text = $"FPS: {(1.0f / dt).ToString("F0")}";
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(UpdateCounter());
    }
}