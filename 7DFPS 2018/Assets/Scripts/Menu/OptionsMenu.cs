using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle vsyncToggle;
    public Toggle buttonPromptsToggle;

    private void OnEnable()
    {
        volumeSlider.value = AudioListener.volume;
        vsyncToggle.isOn = QualitySettings.vSyncCount == 1;
        buttonPromptsToggle.isOn = Config.Main.buttonPrompts;
    }

    public void ChangeVSync()
    {
        Config.Main.vsync = vsyncToggle.isOn;
        Config.Apply();
        Config.Save();
    }

    public void ChangeButtonPrompts()
    {
        Config.Main.buttonPrompts = buttonPromptsToggle.isOn;
        Config.Save();
    }

    public void ChangeVolume()
    {
        Config.Main.volume = volumeSlider.value;
        Config.Apply();
        Config.Save();
    }
}