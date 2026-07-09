using AutoBattler.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : GenericMonoSingleton<AudioManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private string _masterVolumeParameter = "MasterVolume";
    [SerializeField] private string _musicVolumeParameter = "MusicVolume";
    [SerializeField] private string _combatVolumeParameter = "CombatVolume";
    [SerializeField] private string _soundEffectsVolumeParameter = "SoundEffectsVolume";

    [Header("Music AudioSource")]
    [SerializeField] private AudioSource _musicAudioSource;

    [Header("Sound Effects AudioSource")]
    [SerializeField] private AudioSource _soundEffectsAudioSource;

    [Header("Combat AudioSource Pool")]
    [SerializeField] private AudioSource _combatAudioSourcePrefab;
    [SerializeField] private int _poolSize = 20;

    private readonly Queue<AudioSource> _audioSourcePool = new();

    private readonly Dictionary<AudioChannelEnum, float> _audioVolumeDictionary = new();

    private Dictionary<AudioTypeEnum, AudioData> _audioDataDictionary;

    private Coroutine _musicTransitionCoroutine;

    protected override void Awake()
    {
        base.Awake();

        CreatePool();
        LoadVolumes();
    }

    private void CreatePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            AudioSource source = Instantiate(_combatAudioSourcePrefab, transform);
            source.gameObject.SetActive(false);
            _audioSourcePool.Enqueue(source);
        }
    }

    private void LoadVolumes()
    {
        foreach (AudioChannelEnum channel in Enum.GetValues(typeof(AudioChannelEnum)))
        {
            float value = PlayerPrefs.GetFloat(GetParameter(channel), 1f);
            _audioVolumeDictionary[channel] = value;
            ApplyVolume(channel, value);
        }
    }

    public void Initialize(AudioScriptableObjectScript audio_SO)
    {
        _audioDataDictionary = new Dictionary<AudioTypeEnum, AudioData>();

        foreach (AudioData data in audio_SO.audioDataList)
        {
            if (!_audioDataDictionary.ContainsKey(data.audioType))
            {
                _audioDataDictionary.Add(data.audioType, data);
            }
        }
    }

    public AudioData GetAudioData(AudioTypeEnum audioType)
    {
        _audioDataDictionary.TryGetValue(audioType, out AudioData audioData);
        return audioData;
    }

    public void PlayMusic(AudioTypeEnum audioType, bool loop = true)
    {
        AudioData audioData = GetAudioData(audioType);

        if (_musicAudioSource.isPlaying && _musicAudioSource.clip == audioData.audioClip)
        {
            return;
        }

        if (_musicTransitionCoroutine != null)
        {
            StopCoroutine(_musicTransitionCoroutine);
        }

        if (!_musicAudioSource.isPlaying)
        {
            _musicAudioSource.clip = audioData.audioClip;
            _musicAudioSource.loop = loop;
            _musicAudioSource.Play();
            return;
        }

        _musicTransitionCoroutine = StartCoroutine(ChangeMusicRoutine(audioData.audioClip, loop));
    }

    private IEnumerator ChangeMusicRoutine(AudioClip clip, bool loop)
    {
        yield return FadeMusicRoutine(0f, 1f);

        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.loop = loop;
        _musicAudioSource.Play();

        yield return FadeMusicRoutine(1f, 1f);

        _musicTransitionCoroutine = null;
    }

    private IEnumerator FadeMusicRoutine(float target, float duration)
    {
        float start = _musicAudioSource.volume;

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _musicAudioSource.volume = Mathf.Lerp(start, target, timer / duration);
            yield return null;
        }

        _musicAudioSource.volume = target;
    }

    public void PlaySoundEffectsAudio(AudioTypeEnum audioType)
    {
        Debug.Log("X");
        AudioData audioData = GetAudioData(audioType);
        _soundEffectsAudioSource.Stop();
        _soundEffectsAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayCombatAudio(AudioTypeEnum audioType)
    {
        AudioData audioData = GetAudioData(audioType);
        AudioSource combatAudioSource = GetAudioSource();

        combatAudioSource.gameObject.SetActive(true);
        combatAudioSource.PlayOneShot(audioData.audioClip);

        StartCoroutine(ReturnAudioSource(combatAudioSource, audioData.audioClip.length));
    }

    private AudioSource GetAudioSource()
    {
        if (_audioSourcePool.Count > 0)
            return _audioSourcePool.Dequeue();

        return Instantiate(_combatAudioSourcePrefab, transform);
    }

    private IEnumerator ReturnAudioSource(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);

        source.Stop();
        source.gameObject.SetActive(false);

        _audioSourcePool.Enqueue(source);
    }

    public void SetVolume(AudioChannelEnum channel, float value)
    {
        value = Mathf.Clamp01(value);

        _audioVolumeDictionary[channel] = value;

        ApplyVolume(channel, value);

        PlayerPrefs.SetFloat(GetParameter(channel), value);
        PlayerPrefs.Save();
    }

    public float GetVolume(AudioChannelEnum channel)
    {
        return _audioVolumeDictionary.TryGetValue(channel, out float value) ? value : 1f;
    }

    private void ApplyVolume(AudioChannelEnum channel, float value)
    {
        mixer.SetFloat(GetParameter(channel), LinearToDB(value));
    }

    private string GetParameter(AudioChannelEnum channel)
    {
        return channel switch
        {
            AudioChannelEnum.Master => _masterVolumeParameter,
            AudioChannelEnum.Music => _musicVolumeParameter,
            AudioChannelEnum.Combat => _combatVolumeParameter,
            AudioChannelEnum.SoundEffects => _soundEffectsVolumeParameter,
            _ => ""
        };
    }

    private float LinearToDB(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }

}
