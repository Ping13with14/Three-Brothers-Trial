using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// 诊断工具：测试FontEngine对CJK汉字的支持情况
/// 菜单：Window > 字体引擎诊断
/// </summary>
public class FontEngineDiagnostic : EditorWindow
{
    private Font testFont;
    private string resultLog = "";

    [MenuItem("Window/字体引擎诊断")]
    public static void ShowWindow()
    {
        GetWindow<FontEngineDiagnostic>("字体引擎诊断");
    }

    private void OnGUI()
    {
        testFont = (Font)EditorGUILayout.ObjectField("测试字体", testFont, typeof(Font), false);

        if (GUILayout.Button("运行诊断", GUILayout.Height(40)) && testFont != null)
        {
            RunDiagnostic();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("诊断结果", EditorStyles.boldLabel);
        EditorGUILayout.TextArea(resultLog, GUILayout.Height(300));
    }

    private void RunDiagnostic()
    {
        string fontPath = AssetDatabase.GetAssetPath(testFont);
        resultLog = "字体路径: " + fontPath + "\n\n";

        // 加载字体
        FontEngineError error = FontEngine.LoadFontFace(fontPath);
        resultLog += "LoadFontFace 结果: " + error + "\n\n";

        if (error != FontEngineError.Success)
        {
            resultLog += "【失败】字体加载失败，无法继续诊断！\n";
            return;
        }

        // 测试不同区间的字符
        TestRange("基本拉丁字符 (0020-007F)", 0x0020, 0x007F);
        TestRange("CJK标点符号 (3000-303F)", 0x3000, 0x303F);
        TestRange("全角字符 (FF00-FFEF)", 0xFF00, 0xFFEF);
        TestRange("基本汉字 (4E00-9FFF)", 0x4E00, 0x9FFF);

        // 额外测试：随机抽样汉字
        resultLog += "\n--- 随机抽样汉字测试 ---\n";
        TestSingleCharacter(0x4E2D, "中");
        TestSingleCharacter(0x6587, "文");
        TestSingleCharacter(0x7269, "物");
        TestSingleCharacter(0x54C1, "品");
        TestSingleCharacter(0x6E38, "游");
        TestSingleCharacter(0x620F, "戏");
        TestSingleCharacter(0x5B57, "字");
        TestSingleCharacter(0x4F53, "体");
        TestSingleCharacter(0x4E00, "一");
        TestSingleCharacter(0x9AD8, "高");

        Repaint();
    }

    private void TestRange(string label, uint start, uint end)
    {
        uint total = end - start + 1;
        uint found = 0;
        uint missing = 0;

        for (uint code = start; code <= end; code++)
        {
            if (FontEngine.TryGetGlyphIndex(code, out uint _))
                found++;
            else
                missing++;
        }

        resultLog += string.Format("{0}\n  总数: {1},  找到: {2},  缺失: {3}\n\n",
            label, total, found, missing);
    }

    private void TestSingleCharacter(uint unicode, string label)
    {
        bool found = FontEngine.TryGetGlyphIndex(unicode, out uint glyphIndex);
        resultLog += string.Format("  U+{0:X4} ({1}): {2}  (glyph:{3})\n",
            unicode, label, found ? "有 ✓" : "缺 ✗", found ? glyphIndex.ToString() : "N/A");
    }
}
