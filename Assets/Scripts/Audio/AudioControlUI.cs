using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AudioControlUI
{
    public AudioChannelEnum channel;

    public Slider slider;
    public TMP_Text valueText;
    public Toggle muteToggle;
    public Image muteIcon;

    [HideInInspector]
    public float previousVolume = 1f;
}