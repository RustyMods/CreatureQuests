using System.Collections.Generic;
using HarmonyLib;
using ShapeShiftManager;
using TMPro;
using UnityEngine;

namespace CreatureQuests.Source;

public static class HideName
{
    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.ShowHud))]
    private static class EnemyHud_ShowHud_Patch
    {
        private static bool Prefix(EnemyHud __instance, Character c, bool isMount)
        {
            if (CreatureQuestsPlugin._hidePlayerName.Value is CreatureQuestsPlugin.Toggle.Off) return true;
            if (__instance.m_huds.TryGetValue(c, out EnemyHud.HudData _)) return true;
            if (!CreatureFormManager.TryGetCreature(c.name, out CreatureFormManager.CreatureForm data)) return true;
            
            GameObject original = !isMount ? __instance.m_baseHud : __instance.m_baseHudMount;
            EnemyHud.HudData hudData = new EnemyHud.HudData();
            hudData.m_character = c;
            hudData.m_ai = c.GetComponent<BaseAI>();
            hudData.m_gui = Object.Instantiate(original, __instance.m_hudRoot.transform);
            hudData.m_gui.SetActive(true);
            hudData.m_healthFast = hudData.m_gui.transform.Find("Health/health_fast").GetComponent<GuiBar>();
            hudData.m_healthSlow = hudData.m_gui.transform.Find("Health/health_slow").GetComponent<GuiBar>();
            Transform transform = hudData.m_gui.transform.Find("Health/health_fast_friendly");
            if ((bool) (Object) transform)
                hudData.m_healthFastFriendly = transform.GetComponent<GuiBar>();
            if (isMount)
            {
                hudData.m_stamina = hudData.m_gui.transform.Find("Stamina/stamina_fast").GetComponent<GuiBar>();
                hudData.m_staminaText = hudData.m_gui.transform.Find("Stamina/StaminaText").GetComponent<TextMeshProUGUI>();
                hudData.m_healthText = hudData.m_gui.transform.Find("Health/HealthText").GetComponent<TextMeshProUGUI>();
            }
            hudData.m_level2 = hudData.m_gui.transform.Find("level_2") as RectTransform;
            hudData.m_level3 = hudData.m_gui.transform.Find("level_3") as RectTransform;
            hudData.m_alerted = hudData.m_gui.transform.Find("Alerted") as RectTransform;
            hudData.m_aware = hudData.m_gui.transform.Find("Aware") as RectTransform;
            hudData.m_name = hudData.m_gui.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            hudData.m_name.text = Localization.instance.Localize(data.CreatureSharedName);
            hudData.m_isMount = isMount;
            __instance.m_huds.Add(c, hudData);
            return false;
        }
    }

    [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.UpdateHuds))]
    private static class EnemyHud_UpdateHuds_Patch
    {
      private static bool Prefix(EnemyHud __instance, Player player, Sadle sadle, float dt)
      {
        if (CreatureQuestsPlugin._hidePlayerName.Value is CreatureQuestsPlugin.Toggle.Off) return true;
        var mainCamera = Utils.GetMainCamera();
        if (mainCamera == null) return false;
        var character1 = (bool)(Object)sadle ? sadle.GetCharacter() : null;
        var character2 = (bool)(Object)player ? player.GetHoverCreature() : null;
        Character? key = null;
        foreach (KeyValuePair<Character, EnemyHud.HudData> hud in __instance.m_huds)
        {
          var hudData = hud.Value;
          if (hudData.m_character == null || !__instance.TestShow(hudData.m_character, true) ||
              hudData.m_character == character1)
          {
            if (key == null)
            {
              key = hudData.m_character;
              Object.Destroy(hudData.m_gui);
            }
          }
          else
          {
            if (hudData.m_character == character2) hudData.m_hoverTimer = 0.0f;
            hudData.m_hoverTimer += dt;
            var healthPercentage = hudData.m_character.GetHealthPercentage();
            // check if hud data character is shapeshifted
            var isShapeshifted = CreatureFormManager.TryGetCreature(hudData.m_character.name, out CreatureFormManager.CreatureForm data);
            
            if ((hudData.m_character.IsPlayer() && !isShapeshifted) || hudData.m_character.IsBoss() ||
                hudData.m_isMount || hudData.m_hoverTimer < (double)__instance.m_hoverShowDuration)
            {
              hudData.m_gui.SetActive(true);
              var level = isShapeshifted ? 3 : hudData.m_character.GetLevel();
              if (hudData.m_level2) hudData.m_level2.gameObject.SetActive(level == 2);
              if (hudData.m_level3) hudData.m_level3.gameObject.SetActive(level == 3);
              hudData.m_name.text =
                Localization.instance.Localize(isShapeshifted
                  ? data.CreatureSharedName
                  : hudData.m_character.GetHoverName());
              // creatures only
              if (!hudData.m_character.IsBoss() && !hudData.m_character.IsPlayer())
              {
                var flag1 = hudData.m_character.GetBaseAI().HaveTarget();
                var flag2 = hudData.m_character.GetBaseAI().IsAlerted();
                hudData.m_alerted.gameObject.SetActive(flag2);
                hudData.m_aware.gameObject.SetActive(!flag2 & flag1);
              }
              
              // make sure to remove alerted and aware thingies
              if (isShapeshifted)
              {
                hudData.m_alerted.gameObject.SetActive(false);
                hudData.m_aware.gameObject.SetActive(false);
              }
            }
            else
            {
              hudData.m_gui.SetActive(false);
            }

            hudData.m_healthSlow.SetValue(healthPercentage);
            if (hudData.m_healthFastFriendly)
            {
              var flag = !player || BaseAI.IsEnemy(player, hudData.m_character);
              hudData.m_healthFast.gameObject.SetActive(flag);
              hudData.m_healthFastFriendly.gameObject.SetActive(!flag);
              hudData.m_healthFast.SetValue(healthPercentage);
              hudData.m_healthFastFriendly.SetValue(healthPercentage);
            }
            else
            {
              hudData.m_healthFast.SetValue(healthPercentage);
            }

            if (hudData.m_isMount)
            {
              var stamina = sadle.GetStamina();
              var maxStamina = sadle.GetMaxStamina();
              hudData.m_stamina.SetValue(stamina / maxStamina);
              var healthText = hudData.m_healthText;
              var num = Mathf.CeilToInt(hudData.m_character.GetHealth());
              var str1 = num.ToString();
              healthText.text = str1;
              var staminaText = hudData.m_staminaText;
              num = Mathf.CeilToInt(stamina);
              var str2 = num.ToString();
              staminaText.text = str2;
            }

            if (!hudData.m_character.IsBoss() && hudData.m_gui.activeSelf)
            {
              var worldPos = !hudData.m_character.IsPlayer()
                ? !hudData.m_isMount
                  ? hudData.m_character.GetTopPoint()
                  : player.transform.position - player.transform.up * 0.5f
                : hudData.m_character.GetHeadPoint() + Vector3.up * 0.3f;
              var screenPointScaled = mainCamera.WorldToScreenPointScaled(worldPos);
              if (screenPointScaled.x < 0.0 || screenPointScaled.x > (double)Screen.width ||
                  screenPointScaled.y < 0.0 || screenPointScaled.y > (double)Screen.height ||
                  screenPointScaled.z > 0.0)
              {
                hudData.m_gui.transform.position = screenPointScaled;
                hudData.m_gui.SetActive(true);
              }
              else
              {
                hudData.m_gui.SetActive(false);
              }
            }
          }
        }

        if (!(key != null)) return false;
        __instance.m_huds.Remove(key);
        return false;
      }
    }
}