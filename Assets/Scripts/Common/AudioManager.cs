using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音频管理器：单例，管理音效播放（音源池）和背景音乐（独立音源）
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [Header("音效配置")]
    [SerializeField] private int poolSize = 5;       // 音源池大小
    [SerializeField] private float sfxVolume = 0.8f; // 音效音量

    [Header("背景音乐")]
    [SerializeField] private string bgmFileName = "A Brand New Wisdom"; // 背景音乐文件名（不含扩展名）
    [SerializeField] private float bgmVolume = 0.5f;                     // 背景音乐音量

    // 音效
    private AudioSource[] sources;                     // 音源池
    private int sourceIndex;                           // 轮转索引
    private Dictionary<string, AudioClip> clipDict;    // 名称→音频片段

    // 背景音乐
    private AudioSource bgmSource;                     // BGM 专用音源（独立于音效池）

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        // 构建音效音源池
        sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
            sources[i].spatialBlend = 0f; // 2D 音效
        }

        // 构建 BGM 专用音源
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.spatialBlend = 0f;
        bgmSource.loop = true;                        // BGM 循环播放
        bgmSource.volume = bgmVolume;

        // 从 Resources 加载全部音效并构建字典
        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("Audio/SFX");
        clipDict = new Dictionary<string, AudioClip>(loadedClips.Length);
        foreach (var clip in loadedClips)
        {
            clipDict[clip.name] = clip;
        }
        Debug.Log($"音频管理器初始化完成，加载了 {clipDict.Count} 个音效");

        // 自动播放背景音乐
        PlayBGM();
    }

    /// <summary>
    /// 按文件名（不含扩展名）播放音效
    /// </summary>
    public void PlaySFX(string clipName)
    {
        if (!clipDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"音效不存在: {clipName}");
            return;
        }

        AudioSource source = sources[sourceIndex];
        sourceIndex = (sourceIndex + 1) % poolSize;

        source.volume = sfxVolume;
        source.PlayOneShot(clip);
    }

    /// <summary>
    /// 修改音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 播放背景音乐（自动加载 Resources/Audio/ 下的指定文件）
    /// </summary>
    public void PlayBGM()
    {
        if (bgmSource.clip == null)
        {
            bgmSource.clip = Resources.Load<AudioClip>($"Audio/{bgmFileName}");
            if (bgmSource.clip == null)
            {
                Debug.LogWarning($"背景音乐加载失败: Audio/{bgmFileName}");
                return;
            }
        }
        bgmSource.Play();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    /// <summary>
    /// 修改背景音乐音量
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }
}
