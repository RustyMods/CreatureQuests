using ShapeShiftManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CreatureQuests.QuestSystem.Behaviors;

public class DropRelic : MonoBehaviour
{
    public float m_dropAreaRange = 0.5f;
    public void OnDestroy()
    {
        if (CreatureQuestsPlugin._canDropRelics.Value is CreatureQuestsPlugin.Toggle.Off) return;
        if (!Player.m_localPlayer) return;
        if (CreatureFormManager.GetCreature(name.Replace("(Clone)",string.Empty)) is not { ConsumeItem: { } itemDrop }) return;
        if (!Player.m_localPlayer.IsMaterialKnown(itemDrop.m_itemData.m_shared.m_name)) return;
        var chance = Random.Range(0, 100f);
        if (chance > CreatureQuestsPlugin._relicDropChance.Value) return;
        var centerPos = transform.position;
        var random = Random.insideUnitSphere * m_dropAreaRange;
        var item = ItemDrop.DropItem(itemDrop.m_itemData, 1, centerPos + random, Quaternion.identity);
        if (!item.TryGetComponent(out Rigidbody rigidbody)) return;
        Vector3 insideUnitSphere = Random.insideUnitSphere;
        if (insideUnitSphere.y < 0.0)
        {
            insideUnitSphere.y = -insideUnitSphere.y;
        }
        rigidbody.AddForce(insideUnitSphere * 5f, ForceMode.VelocityChange);
    }
}