using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;
public class UIManagerMenu : MonoBehaviour
{

    [Header("Music Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
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
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    public void Exit()
    {
        Application.Quit();
    }
 
}
