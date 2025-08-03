using HarmonyLib;
using ShapeShiftManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CreatureQuests.Source;

public class CreatureSet : SE_Stats
{
    private static Vector3 m_gp_pos = new (-751f, -454f, 0f);
    private static GameObject m_power = null!;
    private static RectTransform m_transform = null!;
    private static Image m_powerIcon = null!;
    private static TMP_Text m_nameGUI = null!;
    private static TMP_Text m_timeText = null!;

    private CreatureFormManager.CreatureForm? m_data;
    public static string m_selectedForm = "";

    public override void Setup(Character character)
    {
        base.Setup(character);
        if (m_character is not Player player) return;
        if (player.m_helmetItem == null) return;
        var creature = player.m_helmetItem.m_shared.m_setName;
        m_selectedForm = creature;
        if (CreatureFormManager.GetCreature(creature) is { } data) m_data = data;
        if (m_data == null) return;
        Show(this);
    }

    public override void Stop()
    {
        base.Stop();
        m_selectedForm = "";
        Hide();
    }

    public override string GetTooltipString()
    {
        var tooltip = base.GetTooltipString();
        if (m_data == null) return tooltip;
        tooltip += $"$tooltip_allow_transform <color=orange>{m_data.CreatureSharedName}</color>\n";
        tooltip += $"$tooltip_hotkey: <color=orange>{CreatureQuestsPlugin._transformationTrigger.Value}</color>";
        return tooltip;
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
    private static class Hud_Awake_Patch
    {
        private static void Postfix(Hud __instance)
        {
            var root = __instance.transform.Find("hudroot");
            var guardian = root.Find("GuardianPower");
            m_gp_pos = guardian.localPosition;
            m_power = Instantiate(guardian.gameObject, root);
            m_transform = m_power.GetComponent<RectTransform>();
            m_power.name = "WyrdformPower";
            m_powerIcon = m_power.transform.Find("Icon").GetComponent<Image>();
            m_nameGUI = m_power.transform.Find("Name").GetComponent<TMP_Text>();
            m_timeText = m_power.transform.Find("TimeText").GetComponent<TMP_Text>();
        }
    }

    private static void Show(CreatureSet effect)
    {
        if (effect.m_data == null) return;
        m_power.SetActive(true);
        m_powerIcon.sprite = effect.m_icon;
        m_nameGUI.text = Localization.instance.Localize(effect.m_data.CreatureSharedName);
    }

    private static void Hide()
    {
        m_power.SetActive(false);
        m_powerIcon.sprite = null;
        m_nameGUI.text = "";
    }

    [HarmonyPatch(typeof(Hud), nameof(Hud.UpdateGuardianPower))]
    private static class Hud_UpdateGuardianPower_Patch
    {
        private static void Postfix(Player player) => Update(player);
    }

    private static void Update(Player player)
    {
        bool hasGuardianPower = player.m_guardianSE != null;
        m_transform.localPosition = hasGuardianPower ? new Vector3(-690f, -454f, 0f) : m_gp_pos;
        var shapeShifted = CreatureFormManager.IsCreatureForm(player.name);
        m_powerIcon.color = !shapeShifted ? Color.white : Hud.s_colorRedBlueZeroAlpha;
        m_timeText.text = Localization.instance.Localize(!shapeShifted ? "$hud_ready" : "$hud_revert");
    }
}