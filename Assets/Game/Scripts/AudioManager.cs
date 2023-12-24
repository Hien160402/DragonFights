using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] musicSound, sfxSounds;
    public AudioSource musicSource, sfxSource, sfxAuraSource, sfxSkillSource, sfxHitSource, sfxVoiceSource, sfxMoveSource;
    public Canvas menu;
    public GameObject optionPanel;
    public Slider sliderMusic;
    public Slider sliderVFX;
    public Toggle toggleMusic;
    public Toggle toggleVFX;
    public static bool isDontDestroyCalled = false;
    private void Awake()
    {
        menu = GameObject.Find("Canvas").GetComponent<Canvas>();
        musicSource = GameObject.Find("AudioManager").transform.Find("MusicSource").GetComponent<AudioSource>();
        if (!isDontDestroyCalled)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(menu);
            isDontDestroyCalled = true;
        }
    }

    private void Start()
    {
        optionPanel = GameObject.Find("Canvas");
        optionPanel = optionPanel.transform.Find("OptionPanel").gameObject;
        sliderMusic = optionPanel.transform.Find("SliderMusic").GetComponent<Slider>();
        sliderVFX = optionPanel.transform.Find("SliderVFX").GetComponent<Slider>();
        toggleMusic = optionPanel.transform.Find("MuteMusic").GetComponent<Toggle>();
        toggleVFX = optionPanel.transform.Find("MuteVFX").GetComponent<Toggle>();
        float musicSliderValue = PlayerPrefs.GetFloat("MusicSlider");
        float vfxSliderValue = PlayerPrefs.GetFloat("VFXSlider");
        bool isMusicMuted = PlayerPrefs.GetInt("MusicMute") == 1;
        bool isVFXMuted = PlayerPrefs.GetInt("SFXMute") == 1;

        sliderMusic.value = musicSliderValue;
        sliderVFX.value = vfxSliderValue;
        toggleMusic.isOn = isMusicMuted;
        toggleVFX.isOn = isVFXMuted;

        musicSource.volume = sliderMusic.value;

        musicSource.mute = !isMusicMuted;
    }

    public void PlayMusic(string name)
    {
        if (PlayerPrefs.GetInt("MusicMute") == 0) musicSource.mute = true;
        else musicSource.mute = false;
        Sound s = Array.Find(musicSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Music not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        sfxSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxSource.mute = true;
        else sfxSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void PlayDelayedSFX(string name)
    {

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSource.PlayDelayed(2f);
        }
    }
    public void StopSFX(string name)
    {

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSource.Stop();
        }
    }

    public void PlayLoopSFX(string name)
    {
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxSource.mute = true;
        else sfxSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }
    public void PlayAuraSFX(string name)
    {
        sfxAuraSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxAuraSource.mute = true;
        else sfxAuraSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxAuraSource.PlayOneShot(s.clip);
        }
    }
    public void PlayAuraLoopSFX(string name)
    {
        sfxAuraSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxAuraSource.mute = true;
        else sfxAuraSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxAuraSource.loop = true;
            sfxAuraSource.clip = s.clip;
            sfxAuraSource.Play();
        }
    }
    public void StopAuraLoopSFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxAuraSource.loop = false;
            sfxAuraSource.Stop();
        }
    }
    public void PlayVoiceSFX(string name)
    {
        sfxVoiceSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxVoiceSource.mute = true;
        else sfxVoiceSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxVoiceSource.PlayOneShot(s.clip);
        }
    }
    public void PlayVoiceOneShotSFX(string name)
    {
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxVoiceSource.mute = true;
        else sfxVoiceSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else if (name == sfxVoiceSource.GetComponent<AudioSource>().clip.name)
        {
            sfxVoiceSource.PlayOneShot(s.clip);
        }
    }

    public void StopVoiceSFX(string name)
    {

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxVoiceSource.Stop();
        }
    }

    public void PlaySkillSFX(string name)
    {
        sfxSkillSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxSkillSource.mute = true;
        else sfxSkillSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSkillSource.PlayOneShot(s.clip);
        }
    }
    public void StopSkillSFX(string name)
    {

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSkillSource.Stop();
        }
    }

    public void PlayHitSFX(string name)
    {
        sfxHitSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxHitSource.mute = true;
        else sfxHitSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxHitSource.PlayOneShot(s.clip);
        }
    }

    public void PlayMoveLoopSFX(string name)
    {
        sfxMoveSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxMoveSource.mute = true;
        else sfxMoveSource.mute = false;
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxMoveSource.loop = true;
            sfxMoveSource.clip = s.clip;
            sfxMoveSource.Play();
        }
    }
    public void StopMoveLoopSFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxMoveSource.loop = false;
            sfxMoveSource.Stop();
        }
    }

    public void PlayMoveSFX(string name)
    {
        sfxVoiceSource.volume = PlayerPrefs.GetFloat("VFXSlider");
        if (PlayerPrefs.GetInt("SFXMute") == 0) sfxVoiceSource.mute = true;
        else sfxVoiceSource.mute = false;

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxVoiceSource.PlayOneShot(s.clip);
        }
    }
    public void StopMoveSFX(string name)
    {

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxVoiceSource.Stop();
        }
    }
    public void StopLoopSFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            sfxSource.loop = false;
            sfxSource.Stop();
        }
    }

    public void ToggleSFX()
    {
        bool isMuted = PlayerPrefs.GetInt("SFXMute") == 0;
        isMuted = !isMuted;
        PlayerPrefs.SetInt("SFXMute", isMuted ? 0 : 1);
        toggleVFX.isOn = !isMuted;
    }

    public void ToggleMusic()
    {
        bool isMuted = PlayerPrefs.GetInt("MusicMute") == 0;
        isMuted = !isMuted;
        PlayerPrefs.SetInt("MusicMute", isMuted ? 0 : 1);
        toggleMusic.isOn = !isMuted;
        if (PlayerPrefs.GetInt("MusicMute") == 0) musicSource.mute = true;
        else musicSource.mute = false;
    }

    private void Update()
    {
        if (optionPanel.activeInHierarchy)
        {
            PlayerPrefs.SetFloat("MusicSlider", sliderMusic.value);
            PlayerPrefs.SetFloat("VFXSlider", sliderVFX.value);
            musicSource.volume = sliderMusic.value;
        }
    }
}
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}