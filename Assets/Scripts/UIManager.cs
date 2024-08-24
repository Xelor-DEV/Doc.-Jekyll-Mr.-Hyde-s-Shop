using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject robotCardPrefab;
    [Header("Music Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] public TMP_Text personsLive;
    [SerializeField] public GameObject pause;
    public Slider MasterSlider
    {
        get
        {
            return masterSlider;
        }
    }
    public Slider MusicSlider
    {
        get
        {
            return musicSlider;
        }
    }
    public Slider SfxSlider
    {
        get
        {
            return sfxSlider;
        }
    }
    private void Start()
    {
        Time.timeScale = 1;
    }
    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Resume()
    {
        pause.SetActive(false);
        Image pause1 = pause.GetComponent<Image>();
        pause1.raycastTarget = false;
        Time.timeScale = 1;
    }
    public void ResetGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void Pausar()
    {
        pause.SetActive(true);
        Image pause1 = pause.GetComponent<Image>();
        pause1.raycastTarget = true;
        Pause();
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}