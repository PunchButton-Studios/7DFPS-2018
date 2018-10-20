using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NewGameMenu : MonoBehaviour
{
    public Regex saveNameRegex = new Regex("[^a-zA-Z0-9 \\-_]");
    public Button createButton;
    public InputField inputField;
    public Text errorText;

    private void OnEnable()
    {
        inputField.text = string.Empty;
        ValidateSaveName();
    }

    public void ValidateSaveName()
    {
        if (!string.IsNullOrWhiteSpace(inputField.text))
        {
            if (saveNameRegex.IsMatch(inputField.text))
            {
                createButton.interactable = false;
                errorText.text = "Name contains invalid characters.";
            }
            else
            {
                if (Directory.Exists($@"{Application.persistentDataPath}\saves\{inputField.text}"))
                {
                    createButton.interactable = false;
                    errorText.text = "There's already a saved game with this name.";
                }
                else
                {
                    createButton.interactable = true;
                    errorText.text = string.Empty;
                }
            }
        }
        else
        {
            createButton.interactable = false;
            errorText.text = "Enter a name.";
        }
    }

    public void CreateNewGame() => FindObjectOfType<MainMenu>().NewGame(inputField.text);
}