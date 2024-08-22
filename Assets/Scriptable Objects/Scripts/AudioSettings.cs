using UnityEngine;
[CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/Data/AudioSettings", order = 1)]
public class AudioSettings : ScriptableObject
{
    [Header("Volume Levels")]
    [SerializeField] private float musicVolume;
    [SerializeField] private float sfxVolume;
    [SerializeField] private float masterVolume;
    public float MusicVolume
    {
        get 
        { 
            return musicVolume; 
        }
        set 
        { 
            musicVolume = value; 
        }
    }
    public float SfxVolume
    {
        get 
        { 
            return sfxVolume; 
        }
        set 
        { 
            sfxVolume = value; 
        }
    }
    public float MasterVolume
    {
        get 
        { 
            return masterVolume; 
        }
        set 
        { 
            masterVolume = value; 
        }
    }
}
