using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换：玩家进入触发器后播放淡入动画，然后传送到新位置并加载新场景
/// </summary>
public class SceneChanger : MonoBehaviour
{
    [Header("目标场景")]
    public string sceneToLoad;             // 要加载的目标场景名称

    [Header("过渡动画")]
    public Animator fadeAnim;              // 淡入淡出动画控制器
    public float fadeTime = .5f;           // 淡入动画时长（秒）

    [Header("新位置")]
    public Vector2 newPlayerPosition;      // 玩家在新场景中的出生位置

    private Transform player;              // 玩家 Transform 引用

    /// <summary>
    /// 玩家进入触发器时：记录玩家位置、播放淡入动画、启动延迟加载协程
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.transform;
            fadeAnim.Play("FadeToWhite");
            StartCoroutine(DelayFade());
        }
    }

    /// <summary>
    /// 等待淡入动画完成后传送玩家并加载新场景
    /// </summary>
    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(fadeTime);
        player.position = newPlayerPosition;
        SceneManager.LoadScene(sceneToLoad);
    }
}
