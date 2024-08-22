using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class GameManagerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIManagerMenu uiManagerMenu;
    [SerializeField] private UIManager uiManagerGame;
    [SerializeField] private AudioManager audioManager;
    public UnityEvent onStart;
    [Header("Properties")]
    [SerializeField] private float time;
    private void Start()
    {
        onStart?.Invoke();
        LoadAudioSettings();
    }
    public void LoadScene(string sceneName)
    {
        DOTween.KillAll();
        SaveAudioSettings();
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SaveAudioSettings()
    {
        if(uiManagerGame != null)
        {
            audioManager.AudioSettings.MusicVolume = uiManagerGame.MusicSlider.value;
            audioManager.AudioSettings.SfxVolume = uiManagerGame.SfxSlider.value;
            audioManager.AudioSettings.MasterVolume = uiManagerGame.MasterSlider.value;
        }
        else if(uiManagerMenu != null)
        {
            audioManager.AudioSettings.MusicVolume = uiManagerMenu.MusicSlider.value;
            audioManager.AudioSettings.SfxVolume = uiManagerMenu.SfxSlider.value;
            audioManager.AudioSettings.MasterVolume = uiManagerMenu.MasterSlider.value;
        }
        else
        {
            Debug.LogError("No existe un UI Manager, ambos son nulos");
        }
    }
    public void LoadAudioSettings()
    {
        if (uiManagerGame != null)
        {
            uiManagerGame.MusicSlider.value = audioManager.AudioSettings.MusicVolume;
            uiManagerGame.SfxSlider.value = audioManager.AudioSettings.SfxVolume;
            uiManagerGame.MasterSlider.value = audioManager.AudioSettings.MasterVolume;
            audioManager.SetVolumeOfMusic(uiManagerGame.MusicSlider);
            audioManager.SetVolumeOfSfx(uiManagerGame.SfxSlider);
            audioManager.SetVolumeOfMaster(uiManagerGame.MasterSlider);
        }
        else if (uiManagerMenu != null)
        {
            uiManagerMenu.MusicSlider.value = audioManager.AudioSettings.MusicVolume;
            uiManagerMenu.SfxSlider.value = audioManager.AudioSettings.SfxVolume;
            uiManagerMenu.MasterSlider.value = audioManager.AudioSettings.MasterVolume;
            audioManager.SetVolumeOfMusic(uiManagerMenu.MusicSlider);
            audioManager.SetVolumeOfSfx(uiManagerMenu.SfxSlider);
            audioManager.SetVolumeOfMaster(uiManagerMenu.MasterSlider);
        }
        else
        {
            Debug.LogError("No existe un UI Manager, ambos son nulos");
        }
    }
}
