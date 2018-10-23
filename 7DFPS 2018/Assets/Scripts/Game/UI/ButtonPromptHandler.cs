using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPromptHandler : MonoBehaviour
{
    public Sprite button, leftMouse, middleMouse, rightMouse;
    public Prompt[] prompts;

    private void OnValidate()
    {
        foreach(Prompt prompt in prompts)
        {
            if (prompt.image != null && prompt.text == null)
                prompt.text = prompt.image.GetComponentInChildren<Text>();
        }
    }

    private void Awake()
    {
        if (!Config.Main.buttonPrompts)
            gameObject.SetActive(false);
        else
        {
            foreach (Prompt prompt in prompts)
            {
                KeyCode keyCode = InputHandler.GetKeyCode(prompt.input)[0];
                switch (keyCode)
                {
                    case KeyCode.Mouse0:
                        prompt.image.sprite = leftMouse;
                        prompt.text.text = string.Empty;
                        break;
                    case KeyCode.Mouse1:
                        prompt.image.sprite = rightMouse;
                        prompt.text.text = string.Empty;
                        break;
                    case KeyCode.Mouse2:
                        prompt.image.sprite = middleMouse;
                        prompt.text.text = string.Empty;
                        break;
                    default:
                        prompt.image.sprite = button;
                        prompt.text.text = keyCode.ToString();
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class Prompt
    {
        public Image image;
        public Text text;
        public InputHandler.Input input;
    }
}