using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHandler : MonoBehaviour
{
    private const KeyCode DEBUG_KEY = KeyCode.F3;
    private const KeyCode TOGGLE_VSYNC = KeyCode.V;
    private const KeyCode TARGET_FPS_INC = KeyCode.Period, TARGET_FPS_DEC = KeyCode.Comma;

    private void Update()
    {
        if(Input.GetKey(DEBUG_KEY))
        {
            if (Input.GetKeyDown(TOGGLE_VSYNC))
            {
                QualitySettings.vSyncCount = QualitySettings.vSyncCount == 0 ? 1 : 0;
                Debug.Log($"VSync is now {(QualitySettings.vSyncCount == 0 ? "Off" : "On")}");
            }
            if(Input.GetKeyDown(TARGET_FPS_INC))
            {
                Application.targetFrameRate = Mathf.Clamp(Application.targetFrameRate + 15, 0, 120);
                Debug.Log($"Target FPS is now {Application.targetFrameRate}");
            }
            if (Input.GetKeyDown(TARGET_FPS_DEC))
            {
                Application.targetFrameRate = Mathf.Clamp(Application.targetFrameRate - 15, 0, 120);
                Debug.Log($"Target FPS is now {Application.targetFrameRate}");
            }
        }
    }
}