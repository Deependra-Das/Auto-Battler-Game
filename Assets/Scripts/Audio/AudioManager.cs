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

    [Header("Combat AudioSources")]
    [SerializeField] private AudioSource _footstepAudioSource;
    [SerializeField] private AudioSource _healAudioSource;
    [SerializeField] private AudioSource _meleeAttackAudioSource;
    [SerializeField] private AudioSource _arrowShotAudioSource;
    [SerializeField] private AudioSource _elementalBurstAudioSource;
    [SerializeField] private AudioSource _fireAttackAudioSource;
    [SerializeField] private AudioSource _natureAttackAudioSource;
    [SerializeField] private AudioSource _thunderAttackAudioSource;
    [SerializeField] private AudioSource _damageShieldAudioSource;
    [SerializeField] private AudioSource _damageUnitAudioSource;
    [SerializeField] private AudioSource _unitDeathAudioSource;

    private readonly Dictionary<AudioChannelEnum, float> _audioVolumeDictionary = new();

    private Dictionary<AudioTypeEnum, AudioData> _audioDataDictionary;

    private Coroutine _musicTransitionCoroutine;

    protected override void Awake()
    {
        base.Awake();
        LoadVolumes();
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

        if (audioData == null || audioData.audioClip == null)
            return;

        if (_musicAudioSource.isPlaying && _musicAudioSource.clip == audioData.audioClip)
            return;

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

    public void PauseMusic()
    {
        if (_musicTransitionCoroutine != null)
        {
            StopCoroutine(_musicTransitionCoroutine);
            _musicTransitionCoroutine = null;
        }

        if (_musicAudioSource.isPlaying)
        {
            _musicAudioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (_musicAudioSource.clip != null && !_musicAudioSource.isPlaying)
        {
            _musicAudioSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (_musicTransitionCoroutine != null)
        {
            StopCoroutine(_musicTransitionCoroutine);
            _musicTransitionCoroutine = null;
        }

        _musicAudioSource.Stop();
    }

    private IEnumerator ChangeMusicRoutine(AudioClip clip, bool loop)
    {
        yield return FadeMusicRoutine(0f, 0.5f);

        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.loop = loop;
        _musicAudioSource.Play();

        yield return FadeMusicRoutine(0.25f, 0.5f);

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
        AudioData audioData = GetAudioData(audioType);
        _soundEffectsAudioSource.Stop();
        _soundEffectsAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayFootstepAudio()
    {
        int randomFootstep = UnityEngine.Random.Range((int)AudioTypeEnum.Footstep1, (int)AudioTypeEnum.Footstep10 + 1);
        AudioData audioData = GetAudioData((AudioTypeEnum)randomFootstep);

        if (audioData == null || audioData.audioClip == null)
            return;

        _footstepAudioSource.Stop();
        _footstepAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayHealAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.Heal);
        _healAudioSource.Stop();
        _healAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayMeleeAttackAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.MeleeAttack);
        _meleeAttackAudioSource.Stop();
        _meleeAttackAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayArrowShotAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.ArrowShot);
        _arrowShotAudioSource.Stop();
        _arrowShotAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayElementalBurstAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.ElementalBurst);
        _elementalBurstAudioSource.Stop();
        _elementalBurstAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayFireAttackAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.FireAttack);
        _fireAttackAudioSource.Stop();
        _fireAttackAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayNatureAttackAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.NatureAttack);
        _natureAttackAudioSource.Stop();
        _natureAttackAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayThunderAttackAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.ThunderAttack);
        _thunderAttackAudioSource.Stop();
        _thunderAttackAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayDamageShieldAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.DamageShield);
        _damageShieldAudioSource.Stop();
        _damageShieldAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayDamageUnitAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.DamageUnit);
        _damageUnitAudioSource.Stop();
        _damageUnitAudioSource.PlayOneShot(audioData.audioClip);
    }

    public void PlayUnitDeathAudio()
    {
        AudioData audioData = GetAudioData(AudioTypeEnum.UnitDeath);
        _unitDeathAudioSource.Stop();
        _unitDeathAudioSource.PlayOneShot(audioData.audioClip);
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
