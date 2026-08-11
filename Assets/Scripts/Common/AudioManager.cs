using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音频管理器：单例，从 Resources/Audio/SFX 加载全部音效，通过音源池播放
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private int poolSize = 5;       // 音源池大小
    [SerializeField] private float sfxVolume = 0.8f; // 音效音量

    private AudioSource[] sources;                     // 音源池
    private int sourceIndex;                           // 轮转索引
    private Dictionary<string, AudioClip> clipDict;    // 名称→音频片段

    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        // 构建音源池
        sources = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
            sources[i].spatialBlend = 0f; // 2D 音效
        }

        // 从 Resources 加载全部音效并构建字典
        AudioClip[] loadedClips = Resources.LoadAll<AudioClip>("Audio/SFX");
        clipDict = new Dictionary<string, AudioClip>(loadedClips.Length);
        foreach (var clip in loadedClips)
        {
            clipDict[clip.name] = clip;
        }
        Debug.Log($"音频管理器初始化完成，加载了 {clipDict.Count} 个音效");
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
    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
