using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 剧情资产生成器：一键创建主线任务所需的所有 QuestSO 和 DialogueSO 资产
/// 使用方法：在Unity编辑器菜单中选择 Tools → 生成主线剧情资产
/// </summary>
public class StorylineGenerator : EditorWindow
{
    // 现有资产路径
    private const string ITEM_PATH = "Assets/Scripts/Inventory & Shop/ItemSos/";
    private const string ACTOR_PATH = "Assets/Scripts/NPC Script/ActorSOs/";
    private const string LOCATION_PATH = "Assets/Scripts/NPC Script/LocationSOs/";
    private const string DIALOGUE_OUT_PATH = "Assets/Scripts/NPC Script/DialogueSOs/";
    private const string QUEST_OUT_PATH = "Assets/Scripts/QuestSystem/_Quest/";

    // 资产引用（运行时加载）
    private static ItemSo goldItem, expItem, meatItem, mushroomItem, arrowItem;
    private static ItemSo wolkItem, woodItem, luPaiItem, daoRenItem, pickaxeItem;
    private static ActorSO blueBob, yellowBob, purpleBob;
    private static LocationSO greenHills;

    [MenuItem("Tools/生成主线剧情资产")]
    public static void GenerateAllAssets()
    {
        LoadExistingAssets();
        if (!ValidateAssets()) return;

        // 创建兜底任务（QuestBoard未配置任务时显示"暂无可用任务"）
        CreateFallbackNoQuest();

        // 创建5个主线任务
        QuestSO q1 = CreateQuest01_BrothersReunited();
        QuestSO q2 = CreateQuest02_ForestPurge();
        QuestSO q3 = CreateQuest03_SupplyGathering();
        QuestSO q4 = CreateQuest04_GreenHillsSecrets();
        QuestSO q5 = CreateQuest05_HerosTrial();

        // 创建Purple Bob的剧情对话链
        List<DialogueSO> purpleConvos = CreatePurpleBobDialogues(q1, q2, q3, q4, q5);

        // 创建Blue Bob和Yellow Bob的剧情对话链
        List<DialogueSO> blueConvos = CreateBlueBobDialogues();
        List<DialogueSO> yellowConvos = CreateYellowBobDialogues();

        // 尝试自动挂接到场景NPC
        WireDialoguesToNPC("PurpleBob", purpleConvos);
        WireDialoguesToNPC("BlueBob", blueConvos);
        WireDialoguesToNPC("YellowBob", yellowConvos);
        // 如果场景中NPC叫"NPC"，也尝试挂接
        WireDialoguesToNPC("NPC", purpleConvos);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("===== 主线剧情资产生成完毕 =====");
        Debug.Log($"创建了 5 个主线任务 QuestSO（{QUEST_OUT_PATH}MainQuest_*.asset）");
        Debug.Log($"创建了 1 个兜底任务 QuestSO（{QUEST_OUT_PATH}Quest_NoAvailableQuest.asset）");
        Debug.Log($"创建了 {purpleConvos.Count} 个Purple Bob对话（{DIALOGUE_OUT_PATH}Purple_Story_*.asset）");
        Debug.Log($"创建了 {blueConvos.Count} 个Blue Bob对话（{DIALOGUE_OUT_PATH}Blue_Story_*.asset）");
        Debug.Log($"创建了 {yellowConvos.Count} 个Yellow Bob对话（{DIALOGUE_OUT_PATH}Yellow_Story_*.asset）");
        Debug.Log("主线任务顺序：兄弟重逢 → 森林清剿 → 物资征集 → 绿丘的秘密 → 英雄试炼");
        Debug.Log("=================================");
        Debug.Log("如果NPC对话未自动挂接，请手动将 Purple_Story_* 对话资产拖入场景中NPC的 Conversations 列表。");
        Debug.Log("对话列表中的顺序很重要（从上到下按优先级排列），请参考对话文件名中的编号。");
    }

    #region 加载现有资产

    private static void LoadExistingAssets()
    {
        goldItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Gold.asset");
        expItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "EXP.asset");
        meatItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Meat.asset");
        mushroomItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Mushroom.asset");
        arrowItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Arrow.asset");
        wolkItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Wolk.asset");
        woodItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Wood.asset");
        luPaiItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "LuPai.asset");
        daoRenItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "DaoCaoRen.asset");
        pickaxeItem = AssetDatabase.LoadAssetAtPath<ItemSo>(ITEM_PATH + "Pickaxe.asset");

        blueBob = AssetDatabase.LoadAssetAtPath<ActorSO>(ACTOR_PATH + "Blue Bob.asset");
        yellowBob = AssetDatabase.LoadAssetAtPath<ActorSO>(ACTOR_PATH + "Yellow Bob.asset");
        purpleBob = AssetDatabase.LoadAssetAtPath<ActorSO>(ACTOR_PATH + "Purple Bob.asset");

        greenHills = AssetDatabase.LoadAssetAtPath<LocationSO>(LOCATION_PATH + "The Green Hills.asset");
    }

    private static bool ValidateAssets()
    {
        bool ok = true;
        if (goldItem == null) { Debug.LogError("缺少 Gold.asset"); ok = false; }
        if (expItem == null) { Debug.LogError("缺少 EXP.asset"); ok = false; }
        if (meatItem == null) { Debug.LogError("缺少 Meat.asset"); ok = false; }
        if (mushroomItem == null) { Debug.LogError("缺少 Mushroom.asset"); ok = false; }
        if (arrowItem == null) { Debug.LogError("缺少 Arrow.asset"); ok = false; }
        if (wolkItem == null) { Debug.LogError("缺少 Wolk.asset"); ok = false; }
        if (luPaiItem == null) { Debug.LogError("缺少 LuPai.asset"); ok = false; }
        if (daoRenItem == null) { Debug.LogError("缺少 DaoCaoRen.asset"); ok = false; }
        if (pickaxeItem == null) { Debug.LogError("缺少 Pickaxe.asset"); ok = false; }
        if (blueBob == null) { Debug.LogError("缺少 Blue Bob.asset"); ok = false; }
        if (yellowBob == null) { Debug.LogError("缺少 Yellow Bob.asset"); ok = false; }
        if (purpleBob == null) { Debug.LogError("缺少 Purple Bob.asset"); ok = false; }
        if (greenHills == null) { Debug.LogError("缺少 The Green Hills.asset"); ok = false; }
        return ok;
    }

    #endregion

    #region 创建主线任务

    // 主线1：兄弟重逢 —— 找到Blue和Yellow，向Purple汇报
    private static QuestSO CreateQuest01_BrothersReunited()
    {
        QuestSO q = CreateInstance<QuestSO>();
        q.questName = "兄弟重逢";
        q.questDescription = "Purple Bob希望三兄弟重新团聚。找到Blue Bob和Yellow Bob，然后向Purple Bob汇报。";
        q.questLevel = 1;

        q.objectives = new List<QuestObjective>
        {
            new QuestObjective
            {
                description = "找到Blue Bob并与他对话",
                objectiveType = ObjectiveType.Talk,
                target = blueBob,
                requiredAmount = 1
            },
            new QuestObjective
            {
                description = "找到Yellow Bob并与他对话",
                objectiveType = ObjectiveType.Talk,
                target = yellowBob,
                requiredAmount = 1
            },
            new QuestObjective
            {
                description = "返回与Purple Bob对话",
                objectiveType = ObjectiveType.Talk,
                target = purpleBob,
                requiredAmount = 1
            }
        };

        q.rewards = new List<QuestReward>
        {
            new QuestReward { itemSo = goldItem, quantity = 5 },
            new QuestReward { itemSo = expItem, quantity = 2 }
        };

        SaveAsset(q, QUEST_OUT_PATH + "MainQuest_01_BrothersReunited.asset");
        return q;
    }

    // 主线2：森林清剿 —— 消灭3个森林怪物
    private static QuestSO CreateQuest02_ForestPurge()
    {
        QuestSO q = CreateInstance<QuestSO>();
        q.questName = "森林清剿";
        q.questDescription = "森林中出现了怪物，威胁着绿丘的安全。前往森林消灭3个怪物，保护这片土地。\n提示：通过传送点可前往森林场景（ARPG）。";
        q.questLevel = 2;

        q.objectives = new List<QuestObjective>
        {
            new QuestObjective
            {
                description = "消灭森林中的怪物",
                objectiveType = ObjectiveType.Kill,
                requiredAmount = 3
            }
        };

        q.rewards = new List<QuestReward>
        {
            new QuestReward { itemSo = goldItem, quantity = 10 },
            new QuestReward { itemSo = meatItem, quantity = 3 },
            new QuestReward { itemSo = expItem, quantity = 5 }
        };

        SaveAsset(q, QUEST_OUT_PATH + "MainQuest_02_ForestPurge.asset");
        return q;
    }

    // 主线3：物资征集 —— 收集蘑菇、金币、箭矢
    private static QuestSO CreateQuest03_SupplyGathering()
    {
        QuestSO q = CreateInstance<QuestSO>();
        q.questName = "物资征集";
        q.questDescription = "为了后续冒险，请收集以下物资：3个激素蘑菇（森林中生长），积累至少10金币（可打怪掉落或在商店出售物品），1支箭矢（在森林商店购买）。\n提示：与森林中的ShopKeeper交互可打开商店。";
        q.questLevel = 3;

        q.objectives = new List<QuestObjective>
        {
            new QuestObjective
            {
                description = "收集激素蘑菇",
                objectiveType = ObjectiveType.Collect,
                target = mushroomItem,
                requiredAmount = 3
            },
            new QuestObjective
            {
                description = "积累金币（打怪/出售物品）",
                objectiveType = ObjectiveType.Collect,
                target = goldItem,
                requiredAmount = 10
            },
            new QuestObjective
            {
                description = "获得箭矢",
                objectiveType = ObjectiveType.Collect,
                target = arrowItem,
                requiredAmount = 1
            }
        };

        q.rewards = new List<QuestReward>
        {
            new QuestReward { itemSo = goldItem, quantity = 15 },
            new QuestReward { itemSo = wolkItem, quantity = 5 }
        };

        SaveAsset(q, QUEST_OUT_PATH + "MainQuest_03_SupplyGathering.asset");
        return q;
    }

    // 主线4：绿丘的秘密 —— 访问绿丘、寻找路牌和稻草人
    private static QuestSO CreateQuest04_GreenHillsSecrets()
    {
        QuestSO q = CreateInstance<QuestSO>();
        q.questName = "绿丘的秘密";
        q.questDescription = "绿丘深处藏着两件上古遗物——路牌和稻草人。路牌是魔法道具可以指引道路，稻草人可以驱散怪物。找到它们。\n提示：路牌和稻草人可能在森林的角落或怪物掉落物中。";
        q.questLevel = 4;

        q.objectives = new List<QuestObjective>
        {
            new QuestObjective
            {
                description = "访问绿丘",
                objectiveType = ObjectiveType.Visit,
                target = greenHills,
                requiredAmount = 1
            },
            new QuestObjective
            {
                description = "找到路牌",
                objectiveType = ObjectiveType.Collect,
                target = luPaiItem,
                requiredAmount = 1
            },
            new QuestObjective
            {
                description = "找到稻草人",
                objectiveType = ObjectiveType.Collect,
                target = daoRenItem,
                requiredAmount = 1
            }
        };

        q.rewards = new List<QuestReward>
        {
            new QuestReward { itemSo = pickaxeItem, quantity = 1 },
            new QuestReward { itemSo = expItem, quantity = 5 }
        };

        SaveAsset(q, QUEST_OUT_PATH + "MainQuest_04_GreenHillsSecrets.asset");
        return q;
    }

    // 主线5：英雄试炼 —— 积累金币、击败怪物
    private static QuestSO CreateQuest05_HerosTrial()
    {
        QuestSO q = CreateInstance<QuestSO>();
        q.questName = "英雄试炼";
        q.questDescription = "最后的试炼！积累至少30金币证明你的交易能力，然后用你擅长的武器击败1个怪物。\n提示：尝试使用弓箭（按ChangeEquipment切换武器）在远程击败敌人，或使用技能树解锁挥砍技能强化近战。";
        q.questLevel = 5;

        q.objectives = new List<QuestObjective>
        {
            new QuestObjective
            {
                description = "积累金币至30",
                objectiveType = ObjectiveType.Collect,
                target = goldItem,
                requiredAmount = 30
            },
            new QuestObjective
            {
                description = "击败怪物",
                objectiveType = ObjectiveType.Kill,
                requiredAmount = 1
            }
        };

        q.rewards = new List<QuestReward>
        {
            new QuestReward { itemSo = goldItem, quantity = 30 },
            new QuestReward { itemSo = expItem, quantity = 10 },
            new QuestReward { itemSo = wolkItem, quantity = 10 }
        };

        SaveAsset(q, QUEST_OUT_PATH + "MainQuest_05_HerosTrial.asset");
        return q;
    }

    // 兜底任务：QuestBoard未配置questToOffer时显示"当前没有可接取的任务"
    private static void CreateFallbackNoQuest()
    {
        QuestSO q = CreateInstance<QuestSO>();
        q.questName = "暂无任务";
        q.questDescription = "当前没有可接取的任务，请稍后再来看看。";
        q.questLevel = 0;
        q.objectives = new List<QuestObjective>();
        q.rewards = new List<QuestReward>();

        SaveAsset(q, QUEST_OUT_PATH + "Quest_NoAvailableQuest.asset");
    }

    #endregion

    #region 创建Blue Bob和Yellow Bob对话链

    // Blue Bob对话：简单的自我介绍
    private static List<DialogueSO> CreateBlueBobDialogues()
    {
        List<DialogueSO> list = new List<DialogueSO>();

        // Blue Bob 问候对话
        DialogueSO d1 = CreateInstance<DialogueSO>();
        d1.name = "Blue_Story_01_Greeting";
        d1.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = blueBob,
                text = "你好，勇者！我是Blue Bob，Purple的二弟。很高兴见到你！"
            },
            new DialogueLine
            {
                speaker = blueBob,
                text = "我哥哥Purple一定在找你。你见到Yellow了吗？他就喜欢在角落里唱歌，有时候不愿意理陌生人呢。"
            }
        };
        d1.removeAfterPlay = true;
        SaveAsset(d1, DIALOGUE_OUT_PATH + "Blue_Story_01_Greeting.asset");
        list.Add(d1);

        return list;
    }

    // Yellow Bob对话：先拒后迎，需要先认识Blue才会好好说话
    private static List<DialogueSO> CreateYellowBobDialogues()
    {
        List<DialogueSO> list = new List<DialogueSO>();

        // 对话1（优先级高）：认识Blue后才理你
        DialogueSO d1 = CreateInstance<DialogueSO>();
        d1.name = "Yellow_Story_02_NiceToMeetYou";
        d1.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = yellowBob,
                text = "哦！你见过我哥哥Blue了？那就是朋友了。我是Yellow Bob，三兄弟里最小的。"
            },
            new DialogueLine
            {
                speaker = yellowBob,
                text = "Purple大哥肯定等急了，你快回去找他吧。我们三兄弟都会支持你的！"
            }
        };
        d1.requiredNPCs = new ActorSO[] { blueBob };
        d1.removeAfterPlay = true;
        SaveAsset(d1, DIALOGUE_OUT_PATH + "Yellow_Story_02_NiceToMeetYou.asset");
        list.Add(d1);

        // 对话2（兜底）：不认识Blue时不理人，可重复触发
        DialogueSO d2 = CreateInstance<DialogueSO>();
        d2.name = "Yellow_Story_01_GoAway";
        d2.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = yellowBob,
                text = "……（Yellow Bob 沉浸在自己的歌声中，完全不理你）"
            }
        };
        // 无前置条件，只要还没认识Blue就会匹配到这个对话
        d2.removeAfterPlay = false;
        SaveAsset(d2, DIALOGUE_OUT_PATH + "Yellow_Story_01_GoAway.asset");
        list.Add(d2);

        return list;
    }

    #endregion

    #region 创建Purple Bob对话链

    private static List<DialogueSO> CreatePurpleBobDialogues(
        QuestSO q1, QuestSO q2, QuestSO q3, QuestSO q4, QuestSO q5)
    {
        List<DialogueSO> list = new List<DialogueSO>();

        // 对话1：开场白，提供主线1
        DialogueSO d1 = CreateInstance<DialogueSO>();
        d1.name = "Purple_Story_01_Intro";
        d1.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "勇者，欢迎来到绿丘！我是Purple Bob，三兄弟中的大哥。黑暗正在侵蚀这片土地，我们需要团结一致才能对抗它。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "首先，请找到我的两个弟弟——Blue Bob和Yellow Bob。他们就在附近。三兄弟重聚后，我才能告诉你接下来的计划。"
            }
        };
        d1.offerquestOnEnd = q1;
        d1.removeAfterPlay = true;
        SaveAsset(d1, DIALOGUE_OUT_PATH + "Purple_Story_01_Intro.asset");
        list.Add(d1);

        // 对话2：找到Blue和Yellow后，提交主线1
        DialogueSO d2 = CreateInstance<DialogueSO>();
        d2.name = "Purple_Story_02_Quest1TurnIn";
        d2.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "太好了！我们三兄弟终于重聚了。感谢你，勇者。这是给你的奖励。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "但危机还没有结束。森林里出现了怪物，它们威胁着村庄的安全。接下来需要你的武力。"
            }
        };
        d2.requiredNPCs = new ActorSO[] { blueBob, yellowBob };
        d2.requiredCompleteQuests = new QuestSO[] { q1 };
        d2.turnInQuestOnEnd = q1;
        d2.removeAfterPlay = true;
        SaveAsset(d2, DIALOGUE_OUT_PATH + "Purple_Story_02_Quest1TurnIn.asset");
        list.Add(d2);

        // 对话3：提供主线2（需要主线1已提交）
        DialogueSO d3 = CreateInstance<DialogueSO>();
        d3.name = "Purple_Story_03_Quest2Offer";
        d3.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "森林中的怪物越来越多了。它们原本是温和的生物，但被黑暗力量侵蚀后变得狂暴。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "请通过传送点前往森林，消灭至少3个怪物。沿途收集掉落的物品，它们会派上用场的。按Slash键可以使用近战攻击。"
            }
        };
        d3.requiredCompletedQuests = new QuestSO[] { q1 };
        d3.offerquestOnEnd = q2;
        d3.removeAfterPlay = true;
        SaveAsset(d3, DIALOGUE_OUT_PATH + "Purple_Story_03_Quest2Offer.asset");
        list.Add(d3);

        // 对话4：提交主线2
        DialogueSO d4 = CreateInstance<DialogueSO>();
        d4.name = "Purple_Story_04_Quest2TurnIn";
        d4.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "做得好！你的战斗力超出我的预期。怪物暂时被压制住了。这是你的奖励。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "不过战斗只是第一步。一个真正的勇者还需要懂得收集资源、与商人交易。准备好接受下一个任务了吗？"
            }
        };
        d4.requiredCompleteQuests = new QuestSO[] { q2 };
        d4.turnInQuestOnEnd = q2;
        d4.removeAfterPlay = true;
        SaveAsset(d4, DIALOGUE_OUT_PATH + "Purple_Story_04_Quest2TurnIn.asset");
        list.Add(d4);

        // 对话5：提供主线3（需要主线2已提交）
        DialogueSO d5 = CreateInstance<DialogueSO>();
        d5.name = "Purple_Story_05_Quest3Offer";
        d5.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "现在该学习如何管理物资了。森林中生长着一种激素蘑菇，能在短时间内提升速度和生命上限。收集3个回来。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "另外，去森林里的商店看看。和ShopKeeper交互可以买卖物品。卖掉多余的物品换取金币，再买一支箭矢。箭矢可以搭配弓箭在远程攻击中使用。"
            }
        };
        d5.requiredCompletedQuests = new QuestSO[] { q2 };
        d5.offerquestOnEnd = q3;
        d5.removeAfterPlay = true;
        SaveAsset(d5, DIALOGUE_OUT_PATH + "Purple_Story_05_Quest3Offer.asset");
        list.Add(d5);

        // 对话6：提交主线3
        DialogueSO d6 = CreateInstance<DialogueSO>();
        d6.name = "Purple_Story_06_Quest3TurnIn";
        d6.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "物资准备充分！你已经学会了战斗、采集和交易——这些是冒险者必备的技能。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "接下来我会告诉你绿丘深处隐藏的秘密……"
            }
        };
        d6.requiredCompleteQuests = new QuestSO[] { q3 };
        d6.turnInQuestOnEnd = q3;
        d6.removeAfterPlay = true;
        SaveAsset(d6, DIALOGUE_OUT_PATH + "Purple_Story_06_Quest3TurnIn.asset");
        list.Add(d6);

        // 对话7：提供主线4（需要主线3已提交）
        DialogueSO d7 = CreateInstance<DialogueSO>();
        d7.name = "Purple_Story_07_Quest4Offer";
        d7.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "绿丘深处埋藏着两件上古遗物。第一件是'路牌'——一个魔法道具，据说可以指引持有者找到隐藏的道路。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "第二件是'稻草人'——可以驱散低级怪物，让你在探索时更安全。去绿丘找到它们，它们散落在森林的各处。"
            }
        };
        d7.requiredCompletedQuests = new QuestSO[] { q3 };
        d7.offerquestOnEnd = q4;
        d7.removeAfterPlay = true;
        SaveAsset(d7, DIALOGUE_OUT_PATH + "Purple_Story_07_Quest4Offer.asset");
        list.Add(d7);

        // 对话8：提交主线4（需要持有路牌+稻草人）
        DialogueSO d8 = CreateInstance<DialogueSO>();
        d8.name = "Purple_Story_08_Quest4TurnIn";
        d8.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "你找到了路牌和稻草人！路牌可以在迷宫中指引你，稻草人则能保护你免受弱小怪物的侵扰。它们现在都属于你了。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "只剩最后一项试炼了。完成它，你将成为绿丘真正的守护者。"
            }
        };
        d8.requiredItems = new ItemSo[] { luPaiItem, daoRenItem };
        d8.requiredCompleteQuests = new QuestSO[] { q4 };
        d8.turnInQuestOnEnd = q4;
        d8.removeAfterPlay = true;
        SaveAsset(d8, DIALOGUE_OUT_PATH + "Purple_Story_08_Quest4TurnIn.asset");
        list.Add(d8);

        // 对话9：提供主线5（需要主线4已提交）
        DialogueSO d9 = CreateInstance<DialogueSO>();
        d9.name = "Purple_Story_09_Quest5Offer";
        d9.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "最后的试炼来了！你已经积累了不少经验。现在打开技能树面板（按ToggleSkillTree），用技能点解锁'挥砍'技能，或者提升你的生命上限。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "然后，试试切换武器（按ChangeEquipment）使用弓箭。积累至少30金币证明你的经济能力，再用任意武器击败一个怪物。去吧！"
            }
        };
        d9.requiredCompletedQuests = new QuestSO[] { q4 };
        d9.offerquestOnEnd = q5;
        d9.removeAfterPlay = true;
        SaveAsset(d9, DIALOGUE_OUT_PATH + "Purple_Story_09_Quest5Offer.asset");
        list.Add(d9);

        // 对话10：提交主线5（大结局）
        DialogueSO d10 = CreateInstance<DialogueSO>();
        d10.name = "Purple_Story_10_Finale";
        d10.lines = new DialogueLine[]
        {
            new DialogueLine
            {
                speaker = purpleBob,
                text = "你做到了！你完成了所有五项试炼——重聚兄弟、清剿怪物、征集物资、探索秘密、证明实力。"
            },
            new DialogueLine
            {
                speaker = purpleBob,
                text = "从今天起，你就是绿丘的守护者！三兄弟会一直在这里支持你。愿光明与你同在，勇者！"
            }
        };
        d10.requiredCompleteQuests = new QuestSO[] { q5 };
        d10.turnInQuestOnEnd = q5;
        d10.removeAfterPlay = true;
        SaveAsset(d10, DIALOGUE_OUT_PATH + "Purple_Story_10_Finale.asset");
        list.Add(d10);

        return list;
    }

    #endregion

    #region 自动挂接对话到场景NPC

    private static void WireDialoguesToNPC(string npcName, List<DialogueSO> conversations)
    {
        // 在所有已加载的场景中查找NPC
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            GameObject[] roots = scene.GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                GameObject npc = FindChildRecursive(root, npcName);
                if (npc == null) continue;

                NPC_Talk npcTalk = npc.GetComponent<NPC_Talk>();
                if (npcTalk == null) continue;

                SerializedObject so = new SerializedObject(npcTalk);
                SerializedProperty convosProp = so.FindProperty("conversations");

                // 收集现有对话
                List<DialogueSO> existingList = new List<DialogueSO>();
                for (int j = 0; j < convosProp.arraySize; j++)
                {
                    DialogueSO d = convosProp.GetArrayElementAtIndex(j).objectReferenceValue as DialogueSO;
                    if (d != null) existingList.Add(d);
                }

                // 新剧情对话插入到列表最前面（NPC_Talk从上到下优先匹配）
                int addedCount = 0;
                for (int idx = conversations.Count - 1; idx >= 0; idx--)
                {
                    DialogueSO convo = conversations[idx];
                    if (!existingList.Contains(convo))
                    {
                        existingList.Insert(0, convo);
                        addedCount++;
                    }
                }

                // 回写列表
                convosProp.arraySize = existingList.Count;
                for (int j = 0; j < existingList.Count; j++)
                {
                    convosProp.GetArrayElementAtIndex(j).objectReferenceValue = existingList[j];
                }

                if (addedCount > 0)
                {
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(npcTalk);
                    Debug.Log($"已将 {addedCount} 个对话添加到场景 '{scene.name}' 的 NPC '{npcName}'");
                }
            }
        }
    }

    private static GameObject FindChildRecursive(GameObject parent, string name)
    {
        if (parent.name == name) return parent;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject found = FindChildRecursive(parent.transform.GetChild(i).gameObject, name);
            if (found != null) return found;
        }
        return null;
    }

    #endregion

    #region 工具方法

    private static void SaveAsset(Object asset, string path)
    {
        // 如果已存在同路径资产则先删除
        Object existing = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        AssetDatabase.CreateAsset(asset, path);
    }

    #endregion
}
