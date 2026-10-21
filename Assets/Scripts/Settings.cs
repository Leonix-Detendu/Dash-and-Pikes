using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    [SerializeField] private AudioMixer audioMixer;

    // Rendu public en lecture pour que l'UIManager de chaque scène puisse lire les valeurs
    public SettingsData CurrentSettings { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Charger les données depuis le disque
        CurrentSettings = SaveSystem.LoadSettings();

        // Appliquer les paramètres immédiatement au démarrage
        ApplyAllSettings();
    }

    public void ApplyAllSettings()
    {
        if (CurrentSettings == null) return;

        SetVolume(CurrentSettings.volume);
        SetQuality(CurrentSettings.qualityIndex);
        SetFullScreen(CurrentSettings.isFullScreen);
    }

    public void SetVolume(float sliderValue)
    {
        CurrentSettings.volume = sliderValue;
        float safeValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        audioMixer.SetFloat("Volume", Mathf.Log10(safeValue) * 20f);
    }

    public void SetQuality(int qualityIndex)
    {
        CurrentSettings.qualityIndex = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        CurrentSettings.isFullScreen = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    public void SaveSettings()
    {
        SaveSystem.SaveSettings(CurrentSettings);
    }
}