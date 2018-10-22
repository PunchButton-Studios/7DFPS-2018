using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;

    private void OnEnable()
    {
        volumeSlider.value = AudioListener.volume;
    }

    public void ChangeVolume()
    {
        Config.Main.volume = volumeSlider.value;
        Config.Apply();
        Config.Save();
    }
}