using HarmonyLib;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace CreatureQuests.QuestSystem;

public static class UI
{
    private static Font? font;
    private static GameObject? m_root;
    private static Text m_title = null!;
    private static Text m_description = null!;
    private static Text m_text = null!;

    private static VerticalLayoutGroup m_group = null!;
    private static RectTransform m_rect = null!;
    private static RectTransform m_titleRect = null!;
    private static RectTransform m_descRect = null!;
    private static RectTransform m_textRect = null!;

    private static bool m_active;
    
    private static void ResizePanelToFitContent()
    {
        Canvas.ForceUpdateCanvases();
        float titleHeight = GetTextPreferredHeight(m_title, m_titleRect);
        float descriptionHeight = GetTextPreferredHeight(m_description, m_descRect);
        float textHeight = GetTextPreferredHeight(m_text, m_textRect);
        
        float totalContentHeight = titleHeight + descriptionHeight + textHeight;
        totalContentHeight += m_group.padding.top + m_group.padding.bottom;
        totalContentHeight += m_group.spacing * 2; 
        float minHeight = 100f;
        float maxHeight = 300f;
        
        float finalHeight = Mathf.Clamp(totalContentHeight, minHeight, maxHeight);
        
        m_rect.sizeDelta = new Vector2(200f, finalHeight);
    }

    private static float GetTextPreferredHeight(Text text, RectTransform rect)
    {
        if (string.IsNullOrEmpty(text.text)) return 0f;
        
        TextGenerator textGen = text.cachedTextGenerator;
        
        var settings = text.GetGenerationSettings(rect.rect.size);
        float preferredHeight = textGen.GetPreferredHeight(text.text, settings);
        
        return preferredHeight;
    }

    public static void Show(string title, string description, string text, bool resize = true)
    {
        if (m_root is null) return;
        m_root.SetActive(true);
        m_title.text = $"<color=orange>{title}</color>";
        m_description.text = description;
        m_text.text = text;
        if (resize) ResizePanelToFitContent();
        m_active = true;
    }

    public static void UpdateProgress(string text) => m_text.text = text;

    public static void Hide()
    {
        if (m_root is null) return;
        m_root.SetActive(false);
        m_title.text = "";
        m_description.text = "";
        m_text.text = "";
        m_active = false;
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
    private static class Hud_Awake_Patch
    {
        private static void Postfix(Hud __instance)
        {
            if (!__instance) return;
            font = FontManager.GetFont(FontManager.FontOptions.AveriaSerifLibre);
            m_root = CreateRoot(__instance);
            m_title = CreateTextElement(m_root.transform, 16, TextAnchor.MiddleLeft, out m_titleRect);
            m_title.text = "";
            m_description = CreateTextElement(m_root.transform, 14, TextAnchor.UpperLeft, out m_descRect);
            m_description.text = "";
            m_text = CreateTextElement(m_root.transform, 14, TextAnchor.UpperLeft, out m_textRect);
            m_text.text = "";
            m_root.SetActive(false);

            m_root.transform.localScale = Vector3.one * CreatureQuestsPlugin._UIScale.Value;
            CreatureQuestsPlugin._UIScale.SettingChanged += (_, _) =>
            {
                m_root.transform.localScale = Vector3.one * CreatureQuestsPlugin._UIScale.Value;
            };
        }

        
        private static GameObject CreateRoot(Hud __instance)
        {
            GameObject root = new GameObject("QuestHud");
            RectTransform rect = root.AddComponent<RectTransform>();
            m_rect = rect;
            rect.SetParent(__instance.transform);
            rect.anchoredPosition = CreatureQuestsPlugin._hudPosition.Value;
            CreatureQuestsPlugin._hudPosition.SettingChanged +=
                (_, _) => rect.anchoredPosition = CreatureQuestsPlugin._hudPosition.Value;
            
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            
            rect.sizeDelta = new Vector2(200f, 100f);
            Image image = root.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.5f);
            var layout = root.AddComponent<VerticalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.padding.left = 5;
            layout.padding.top = 5;
            layout.padding.bottom = 5;
            layout.padding.right = 5;
            m_group = layout;
            return root;
        }

        private static Text CreateTextElement(Transform parent, int fontSize, TextAnchor alignment, out RectTransform rect)
        {
            GameObject element = new GameObject("element");
            rect = element.AddComponent<RectTransform>();
            rect.SetParent(parent);
            Text text = element.AddComponent<Text>();
            text.alignment = alignment;
            text.font = font;
            text.fontSize = fontSize;
            return text;
        }
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.SetVisible))]
    private static class Hud_SetVisible_Patch
    {
        private static void Postfix(bool visible)
        {
            if (m_root == null) return;
            m_root.SetActive(m_active && visible && (Player.m_localPlayer && !Player.m_localPlayer.IsTeleporting()) && Minimap.instance.m_mode != Minimap.MapMode.Large);
        }
    }
}