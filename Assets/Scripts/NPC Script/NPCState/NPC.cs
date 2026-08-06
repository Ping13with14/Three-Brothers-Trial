using UnityEngine;

/// <summary>
/// NPC状态机：通过启用/禁用组件来切换巡逻、漫游、对话等行为
/// </summary>
public class NPC : MonoBehaviour
{
    public enum NPCState { Default, Idle, Patol, Wander, Talk }
    // 初始状态（可在Inspector中设置）
    public NPCState currentState = NPCState.Wander;
    // 玩家离开触发器后恢复的默认状态
    private NPCState defaultState;

    public NPC_Patol patol;
    public NPC_Wander wander;
    public NPC_Talk talk;

    void Start()
    {
        defaultState = currentState;
        SwitchState(currentState);
    }

    // 切换NPC状态：启用对应组件，禁用其余组件
    public void SwitchState(NPCState newState)
    {
        currentState = newState;

        // Default和Idle状态关闭所有移动组件（静止不动）
        bool isPatrol = newState == NPCState.Patol;
        bool isWander = newState == NPCState.Wander;
        bool isTalk   = newState == NPCState.Talk;

        if (patol != null) patol.enabled = isPatrol;
        if (wander != null) wander.enabled = isWander;
        if (talk != null)   talk.enabled = isTalk;
    }

    // 玩家靠近时切换到对话状态
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            SwitchState(NPCState.Talk);
    }

    // 玩家离开时恢复默认状态
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            SwitchState(defaultState);
    }
}
