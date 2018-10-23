using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputHandler
{
    public static bool GetButton(Input input) => UnityEngine.Input.GetKey(GetKeyCode(input)[0]);
    public static bool GetButtonDown(Input input) => UnityEngine.Input.GetKeyDown(GetKeyCode(input)[0]);
    public static bool GetButtonUp(Input input) => UnityEngine.Input.GetKeyUp(GetKeyCode(input)[0]);

    public static float GetAxis(Input input)
    {
        KeyCode[] axisKeys = GetKeyCode(input);
        return (UnityEngine.Input.GetKey(axisKeys[0]) ? 1 : 0) - (UnityEngine.Input.GetKey(axisKeys[1]) ? 1 : 0);
    }

    public static KeyCode[] GetKeyCode(Input input)
    {
        Config.Keybinds keybinds = Config.Main.keybinds;

        switch(input)
        {
            case Input.MovementVertical:
                return new KeyCode[] { keybinds.movementForward, keybinds.movementBackward };
            case Input.MovementHorizontal:
                return new KeyCode[] { keybinds.movementRight, keybinds.movementLeft };
            case Input.Action:
                return new KeyCode[] { keybinds.action };
            case Input.Flashlight:
                return new KeyCode[] { keybinds.flashlight };
            case Input.Pause:
                return new KeyCode[] { keybinds.pause };
            case Input.CallHome:
                return new KeyCode[] { keybinds.callHome };
            default:
                return null;
        }
    }

    public enum Input
    {
        MovementVertical,
        MovementHorizontal,
        Action,
        Flashlight,
        Pause,
        CallHome,
    }
}