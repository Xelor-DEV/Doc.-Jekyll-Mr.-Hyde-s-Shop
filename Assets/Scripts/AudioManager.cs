using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioSettings audioSettings;
    public AudioSettings AudioSettings
    {
        get
        {
            return audioSettings;
        }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void SetVolumeOfMusic(Slider musicConfiguration)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicConfiguration.value) * 20f);
    }
    public void SetVolumeOfSfx(Slider sfxConfiguration)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxConfiguration.value) * 20f);
    }
    public void SetVolumeOfMaster(Slider masterConfiguration)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterConfiguration.value) * 20f);
    }
    public void PlayMusic(int index)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = musicClips[index];
        musicAudioSource.Play();
    }
    public void PlaySfx(int index)
    {
        sfxAudioSource.PlayOneShot(sfxClips[index]);
    }
}