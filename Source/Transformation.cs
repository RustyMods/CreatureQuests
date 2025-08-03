using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using CreatureQuests.QuestSystem;
using HarmonyLib;
using ItemManager;
using Shapeshift.Source;
using ShapeShiftManager;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Helpers = Shapeshift.Source.Helpers;

namespace CreatureQuests.Source;

public static class Transformation
{
    private static bool m_loaded;

    private static string RootFolder = Paths.ConfigPath + Path.DirectorySeparatorChar + "CreatureQuests";
    private static string CreatureFolder = RootFolder + Path.DirectorySeparatorChar + "CustomCreatures";
    private static string ExportFolder = CreatureFolder + Path.DirectorySeparatorChar + "Exported";
    
    private static ZNetScene m_scene = null!;
    private static GameObject? FindPrefab(string name) => m_scene.m_prefabs.Find(x => x.name == name);
    private static readonly Dictionary<CreatureFormManager.CreatureForm, CreatureFormConfigs> Configs = new();
    public static readonly Dictionary<string, CreatureFormManager.CreatureForm> m_sharedNameToCreature = new();
    
    public class CreatureFormConfigs
    {
        public readonly CreatureFormManager.CreatureForm Data;
        public readonly ConfigEntry<Toggle> Enabled;
        public readonly ConfigEntry<float> Duration;

        public CreatureFormConfigs(CreatureFormManager.CreatureForm creature, string configGroup)
        {
            Data = creature;
            Enabled = CreatureQuestsPlugin.plugin.config(configGroup, "Enabled", Toggle.On, "If on, creature form is enabled");
            Duration = CreatureQuestsPlugin.plugin.config(configGroup, "Duration", Data.StatusEffect.m_ttl, "Set duration of item status effect");
            Duration.SettingChanged += (sender, args) =>
            {
                Data.StatusEffect.m_ttl = Duration.Value;
            };
            Configs[creature] = this;
        }
    }
    public static void Setup()
    {
        foreach (var creature in CreatureFormManager.GetAllRegisteredCreatures())
        {
            creature.GenerateItem = true;
            creature.OnSetupFinish = data =>
            {
                if (data.ConsumeItem == null) return;
                string englishName = "";
                Item item = new Item(data.ConsumeItem.gameObject);
                var name = Helpers.SplitCamelCase(data.SourcePrefabName);
                if (name == "Dverer Mage")
                {
                    var lastWord = Helpers.GetLastWord(Helpers.SplitCamelCase(data.OverrideName));
                    englishName = name + " " + lastWord + " Relic";
                }
                else englishName = name + " Relic";

                item.Name.English(englishName);
                item.Description.English("Take the form of " + name);
                item.RequiredItems.Add("SwordCheat", 1);
                data.Prefab.AddComponent<EventSystem>();
                data.StatusEffect.OnStop = () =>
                {
                    if (QuestManager.m_currentQuest is not { } quest) return;
                    quest.OnDeath();
                };

                var localizedName = Localization.instance.Localize(creature.CreatureSharedName);
                m_sharedNameToCreature[localizedName.ToLower()] = creature;
                
                var configs = new CreatureFormConfigs(data, englishName);
            };
        }
    }

    [HarmonyPriority(Priority.Last)]
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class ObjectDB_Awake_Patch
    {
        private static void Postfix()
        {
            if (!ZNetScene.instance) return;
            Read();
        }
    }

    private static void Read()
    {
        if (!Directory.Exists(CreatureFolder)) return;
        var files = Directory.GetFiles(CreatureFolder, "*.yml");
        if (files.Length == 0) return;
        var deserializer = new DeserializerBuilder().Build();
        foreach (var file in files)
        {
            try
            {
                var info = deserializer.Deserialize<CustomCreature>(File.ReadAllText(file));
                if (ZNetScene.instance.GetPrefab(info.PrefabName) is not { } prefab)
                {
                    CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Failed to find prefab: " + info.PrefabName);
                    return;
                }
                var creature = new CreatureFormManager.CreatureForm(prefab, info.OverrideName);
                creature.OnStartAttack = (data, instance, secondary) => data.OverrideAttack(instance, secondary, info.PrimaryAttack, info.SecondaryAttack);
                creature.OnSetControls = (data, instance, block, jump) => data.OverrideSetControls(instance, block, jump, info.BlockAttack);
                creature.UsePlayerAnimator = info.UsePlayerAnimator;
                creature.HideWeapons = info.HideWeapons;
                creature.HideArmor = info.HideArmor;
                creature.HideHelmet = info.HideHelmet;
                creature.RequireWeapon = info.RequireWeaponEquipped;
                creature.RequireRanged = info.RequireRangedWeaponEquipped;
                creature.WaterCreature = info.WaterCreature;
                creature.CanFly = info.FlyingCreature;
                creature.CameraMaxDistance = info.CameraMaxDistance;
                creature.HeadTransform = info.HeadTransform;
                creature.EnableUseItem = info.EnableUseItem;
                creature.UseItem = info.UseItem;
                creature.Duration = info.Duration;
                creature.Setup();
                if (!ZNetScene.instance.m_prefabs.Contains(creature.Prefab)) ZNetScene.instance.m_prefabs.Add(creature.Prefab);
                ZNetScene.instance.m_namedPrefabs[creature.Prefab.name.GetStableHashCode()] = creature.Prefab;
                if (!ObjectDB.instance.m_StatusEffects.Contains(creature.StatusEffect)) ObjectDB.instance.m_StatusEffects.Add(creature.StatusEffect);
                CreatureFormManager.m_sourceToCreatures[info.OverrideName.IsNullOrWhiteSpace() ? info.PrefabName : info.OverrideName] = creature;
                CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo("Registered custom creature: " + creature.PrefabName);
            }
            catch
            {
                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Failed to read custom creature file: " + Path.GetFileName(file));
            }
        }
    }

    public static bool Export(string value)
    {
        if (!ZNetScene.instance || !ObjectDB.instance) return false; 
        if (ZNetScene.instance.GetPrefab(value) is not { } prefab || !prefab.GetComponent<Character>())
            return false;
        CreatureFormManager.CreatureForm creature = new CreatureFormManager.CreatureForm(prefab);
        creature.Setup();
        if (!ZNetScene.instance.m_prefabs.Contains(creature.Prefab)) ZNetScene.instance.m_prefabs.Add(creature.Prefab);
        ZNetScene.instance.m_namedPrefabs[creature.Prefab.name.GetStableHashCode()] = creature.Prefab;
        if (!ObjectDB.instance.m_StatusEffects.Contains(creature.StatusEffect)) ObjectDB.instance.m_StatusEffects.Add(creature.StatusEffect);
        CreatureFormManager.m_sourceToCreatures[prefab.name] = creature;
        if (creature.AllItems.Count > 0)
        {
            creature.OnStartAttack = (data, instance, secondary) =>
                data.OverrideAttack(instance, secondary, creature.AllItems.Keys.ToList()[0]);
        }

        var export = new Dictionary<string, object>();
        export["CreatureName"] = value;
        export["Items"] = creature.AllItems.Select(i => i.Value.name).ToList();
        var visual = prefab.transform.Find("Visual");

        Dictionary<string, object> BuildBoneHierarchy(Transform parent)
        {
            var result = new Dictionary<string, object>();
            foreach (Transform child in parent)
            {
                result[child.name] = child.childCount > 0
                    ? BuildBoneHierarchy(child)
                    : new Dictionary<string, object>();
            }

            return result;
        }

        export["Bones"] = BuildBoneHierarchy(visual);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(export);

        if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
        if (!Directory.Exists(CreatureFolder)) Directory.CreateDirectory(CreatureFolder);
        if (!Directory.Exists(ExportFolder)) Directory.CreateDirectory(ExportFolder);
        var filePath = ExportFolder + Path.DirectorySeparatorChar + prefab.name + ".yml";
        File.WriteAllText(filePath, yaml);
        var custom = new CustomCreature();
        custom.PrefabName = prefab.name;
        var customFile = ExportFolder + Path.DirectorySeparatorChar + prefab.name + "_DATA.yml";
        File.WriteAllText(customFile, serializer.Serialize(custom));
        
        CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo("Exported successfully: " + value);

        return true;
    }
    
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UseItem))]
    private static class Humanoid_UseItem_Patch
    {
        private static void Postfix(Humanoid __instance, Inventory inventory, ItemDrop.ItemData item)
        {
            if (__instance is not Player player) return;
            if (!CreatureFormManager.m_itemToCreatures.TryGetValue(item.m_shared.m_name, out var data)) return;
            if (!Configs.TryGetValue(data, out CreatureFormConfigs configs)) return;
            if (configs.Enabled.Value is Toggle.Off) return;
            if (!inventory.ContainsItem(item)) return;
            player.GetSEMan().AddStatusEffect(data.StatusEffect);
            inventory.RemoveOneItem(item);
        }
    }

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Awake))]
    private static class FejdStartup_Awake_Patch
    {
        private static void Postfix(FejdStartup __instance)
        {
            if (m_loaded) return;
            m_scene = __instance.m_objectDBPrefab.GetComponent<ZNetScene>();
            
            SetupValkyrieArmor();
            SetupBoarArmor();
            CreatureQuestsPlugin.InitRaven();
            Item.Patch_FejdStartup();
            m_loaded = true;
        }
    }

    private static void SetupValkyrieArmor()
    {
        var ValkyrieSet = ScriptableObject.CreateInstance<SE_ValkyrieSet>();
        ValkyrieSet.name = "SE_ValkyrieSet";
        ValkyrieSet.m_name = "$label_shapeshift";

        var HelmetValkyrie = new Item("shapeshiftbundle", "HelmetValkyrie_RS");
        HelmetValkyrie.Name.English("Wyrdform Helmet");
        HelmetValkyrie.Description.English(
            "Forged in the image of Odin’s watchful ravens, this helm grants clarity in battle and whispers of distant truths. It sees what the eye cannot");
        HelmetValkyrie.RequiredItems.Add("SwordCheat", 1);
        HelmetValkyrie.RequiredUpgradeItems.Add("LinenThread", 16);
        HelmetValkyrie.RequiredUpgradeItems.Add("Eitr", 15);
        HelmetValkyrie.RequiredUpgradeItems.Add("AskHide", 2);
        HelmetValkyrie.Crafting.Add(CraftingTable.Workbench, 1);
        var HelmetData = HelmetValkyrie.Prefab.GetComponent<ItemDrop>();
        HelmetData.m_itemData.m_durability = 100;
        HelmetData.m_itemData.m_shared.m_maxQuality = 4;
        HelmetData.m_itemData.m_shared.m_weight = 1;
        HelmetData.m_itemData.m_shared.m_eitrRegenModifier = 0.3f;
        HelmetData.m_itemData.m_shared.m_armor = 21;
        HelmetData.m_itemData.m_shared.m_armorPerLevel = 2;
        HelmetData.m_itemData.m_shared.m_setName = "valkyrie";
        HelmetData.m_itemData.m_shared.m_setSize = 4;
        HelmetData.m_itemData.m_shared.m_setStatusEffect = ValkyrieSet;
        ValkyrieSet.m_icon = HelmetData.m_itemData.GetIcon();

        var ChestValkyrie = new Item("shapeshiftbundle", "ArmorValkyrieChest_RS");
        ChestValkyrie.Name.English("Wyrdform Breastplate");
        ChestValkyrie.Description.English(
            "These weightless pauldrons channel the might of fallen shieldmaidens. With them, your spirit bears the strength of those who walk with the gods.");
        ChestValkyrie.RequiredItems.Add("SwordCheat", 1);
        ChestValkyrie.RequiredUpgradeItems.Add("CelestialFeather", 5);
        ChestValkyrie.RequiredUpgradeItems.Add("Eitr", 20);
        ChestValkyrie.RequiredUpgradeItems.Add("AskHide", 10);
        ChestValkyrie.RequiredUpgradeItems.Add("FlametalNew", 5);
        ChestValkyrie.Crafting.Add(CraftingTable.Workbench, 1);
        var ChestData = ChestValkyrie.Prefab.GetComponent<ItemDrop>();
        ChestData.m_itemData.m_durability = 100;
        ChestData.m_itemData.m_shared.m_maxQuality = 4;
        ChestData.m_itemData.m_shared.m_weight = 5;
        ChestData.m_itemData.m_shared.m_eitrRegenModifier = 0.5f;
        ChestData.m_itemData.m_shared.m_movementModifier = -0.02f;
        ChestData.m_itemData.m_shared.m_armor = 21;
        ChestData.m_itemData.m_shared.m_armorPerLevel = 2;
        ChestData.m_itemData.m_shared.m_setName = "valkyrie";
        ChestData.m_itemData.m_shared.m_setSize = 4;
        ChestData.m_itemData.m_shared.m_setStatusEffect = ValkyrieSet;

        var LegsValkyrie = new Item("shapeshiftbundle", "ArmorValkyrieLegs_RS");
        LegsValkyrie.Name.English("Wyrdform Skirt");
        LegsValkyrie.Description.English(
            "Woven with strands of sun-gold, this armored skirt honors the road to Valhalla. Each step rings with the resolve of the worthy.");
        LegsValkyrie.RequiredItems.Add("SwordCheat", 1);
        LegsValkyrie.RequiredUpgradeItems.Add("LinenThread", 20);
        LegsValkyrie.RequiredUpgradeItems.Add("Eitr", 20);
        LegsValkyrie.RequiredUpgradeItems.Add("AskHide", 10);
        LegsValkyrie.RequiredUpgradeItems.Add("FlametalNew", 5);
        LegsValkyrie.Crafting.Add(CraftingTable.Workbench, 1);
        var LegData = LegsValkyrie.Prefab.GetComponent<ItemDrop>();
        LegData.m_itemData.m_durability = 100;
        LegData.m_itemData.m_shared.m_maxQuality = 4;
        LegData.m_itemData.m_shared.m_weight = 5;
        LegData.m_itemData.m_shared.m_eitrRegenModifier = 0.5f;
        LegData.m_itemData.m_shared.m_movementModifier = -0.02f;
        LegData.m_itemData.m_shared.m_armor = 21;
        LegData.m_itemData.m_shared.m_armorPerLevel = 2;
        LegData.m_itemData.m_shared.m_setName = "valkyrie";
        LegData.m_itemData.m_shared.m_setSize = 4;
        LegData.m_itemData.m_shared.m_setStatusEffect = ValkyrieSet;

        var CapeValkyrie = new Item("shapeshiftbundle", "CapeValkyrie_RS");
        CapeValkyrie.Name.English("Wyrdform Cape");
        CapeValkyrie.Description.English(
            "Plucked from the wings of Hugin and Munin, this cloak shields your back and veils your presence. It is said to carry the scent of the heavens.");
        CapeValkyrie.RequiredItems.Add("SwordCheat", 1);
        CapeValkyrie.RequiredUpgradeItems.Add("CelestialFeather", 10);
        CapeValkyrie.RequiredUpgradeItems.Add("Eitr", 15);
        CapeValkyrie.RequiredUpgradeItems.Add("AskHide", 2);
        CapeValkyrie.Crafting.Add(CraftingTable.Workbench, 1);

        var CapeData = CapeValkyrie.Prefab.GetComponent<ItemDrop>();
        CapeData.m_itemData.m_shared.m_equipStatusEffect =
            FindPrefab("CapeFeather")!.GetComponent<ItemDrop>().m_itemData.m_shared.m_equipStatusEffect;
        CapeData.m_itemData.m_durability = 400f;
        CapeData.m_itemData.m_shared.m_maxQuality = 4;
        CapeData.m_itemData.m_shared.m_weight = 4f;
        CapeData.m_itemData.m_shared.m_armor = 1f;
        CapeData.m_itemData.m_shared.m_armorPerLevel = 1;
        CapeData.m_itemData.m_shared.m_damageModifiers = new List<HitData.DamageModPair>
        {
            new() { m_type = HitData.DamageType.Fire, m_modifier = HitData.DamageModifier.VeryWeak },
            new() { m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.Resistant }
        };
        CapeData.m_itemData.m_shared.m_setName = "valkyrie";
        CapeData.m_itemData.m_shared.m_setSize = 4;
        CapeData.m_itemData.m_shared.m_setStatusEffect = ValkyrieSet;

        var valkyrieMaterial = FindPrefab("Valkyrie")!.GetComponentInChildren<SkinnedMeshRenderer>(true).sharedMaterials;
        var helmetModel = HelmetValkyrie.Prefab.transform.Find("model").GetComponent<MeshRenderer>();
        helmetModel.sharedMaterials = valkyrieMaterial;
        helmetModel.materials = valkyrieMaterial;
        var helmetMesh = HelmetValkyrie.Prefab.transform.Find("attach_skin/Helmet").GetComponent<SkinnedMeshRenderer>();
        helmetMesh.sharedMaterials = valkyrieMaterial;
        helmetMesh.materials = valkyrieMaterial;
        // ChestData.m_itemData.m_shared.m_armorMaterial =
        //     FindPrefab("ArmorFlametalChest").GetComponent<ItemDrop>().m_itemData.m_shared.m_armorMaterial;
        LegData.m_itemData.m_shared.m_armorMaterial =
            FindPrefab("ArmorFlametalLegs")!.GetComponent<ItemDrop>().m_itemData.m_shared.m_armorMaterial;
        var legsModel = LegsValkyrie.Prefab.transform.Find("log").GetComponent<MeshRenderer>();
        legsModel.materials = valkyrieMaterial;
        legsModel.sharedMaterials = valkyrieMaterial;
        var legsMesh = LegsValkyrie.Prefab.transform.Find("attach_skin/Belt").GetComponent<SkinnedMeshRenderer>();
        legsMesh.materials = valkyrieMaterial;
        legsMesh.sharedMaterials = valkyrieMaterial;
        var chestModel = ChestValkyrie.Prefab.transform.Find("log").GetComponent<MeshRenderer>();
        chestModel.sharedMaterials = valkyrieMaterial;
        chestModel.materials = valkyrieMaterial;
        var chestMesh = ChestValkyrie.Prefab.transform.Find("attach_skin/Chest").GetComponent<SkinnedMeshRenderer>();
        var chestMats = chestMesh.sharedMaterials;
        var leatherMat = chestMats[0];
        leatherMat.shader = valkyrieMaterial[0].shader;
        leatherMat.SetColor("_MetalColor", new Color32(245, 180, 36, 255));
        chestMats[0] = leatherMat;
        chestMats[1] = valkyrieMaterial[0];
        chestMats[2] = valkyrieMaterial[0];
        chestMesh.sharedMaterials = chestMats;
        chestMesh.materials = chestMats;
        var armBands = ChestValkyrie.Prefab.transform.Find("attach_skin/ArmBands")
            .GetComponent<SkinnedMeshRenderer>();
        armBands.materials = valkyrieMaterial;
        armBands.sharedMaterials = valkyrieMaterial;
        var shoulder = ChestValkyrie.Prefab.transform.Find("attach_skin/Shoulder")
            .GetComponent<SkinnedMeshRenderer>();
        shoulder.materials = new[] { leatherMat };
        shoulder.sharedMaterials = new[] { leatherMat };
        var capeModel = CapeValkyrie.Prefab.transform.Find("log").GetComponent<MeshRenderer>();
        capeModel.materials = valkyrieMaterial;
        capeModel.sharedMaterials = valkyrieMaterial;
        var capeMesh = CapeValkyrie.Prefab.transform.Find("attach_skin/Cape").GetComponent<SkinnedMeshRenderer>();
        var capeMats = new[] { valkyrieMaterial[0], valkyrieMaterial[0] };
        capeMesh.materials = capeMats;
        capeMesh.sharedMaterials = capeMats;
    }

    private static void SetupBoarArmor()
    {
        var BoarSE = ScriptableObject.CreateInstance<CreatureSet>();
        BoarSE.name = "SE_BoarSet";
        BoarSE.m_name = "$label_shapeshift $enemy_boar";
        BoarSE.m_staminaRegenMultiplier = 1.05f;
        
        var HelmetBoar = new Item("shapeshiftbundle", "HelmetBoar_RS");
        HelmetBoar.Name.English("Boar Hood");
        HelmetBoar.Description.English("A hood made from boar leather");
        HelmetBoar.RequiredItems.Add("Shapeshift_Boar_item", 1);
        HelmetBoar.RequiredUpgradeItems.Add("LeatherScraps", 5);
        HelmetBoar.Crafting.Add(CraftingTable.Workbench, 1);
        var HelmetData = HelmetBoar.Prefab.GetComponent<ItemDrop>();
        HelmetData.m_itemData.m_durability = 100;
        HelmetData.m_itemData.m_shared.m_maxQuality = 4;
        HelmetData.m_itemData.m_shared.m_weight = 1;
        HelmetData.m_itemData.m_shared.m_armor = 1;
        HelmetData.m_itemData.m_shared.m_armorPerLevel = 2;
        HelmetData.m_itemData.m_shared.m_setName = "Boar";
        HelmetData.m_itemData.m_shared.m_setSize = 3;
        HelmetData.m_itemData.m_shared.m_setStatusEffect = BoarSE;
        BoarSE.m_icon = FindPrefab("TrophyBoar")!.GetComponent<ItemDrop>().m_itemData.GetIcon();

        var ChestBoar = new Item("shapeshiftbundle", "ArmorBoarChest_RS");
        ChestBoar.Name.English("Boar Pauldrons");
        ChestBoar.Description.English("A bare chest for the true warriors");
        ChestBoar.RequiredItems.Add("Shapeshift_Boar_item", 1);
        ChestBoar.RequiredUpgradeItems.Add("LeatherScraps", 10);
        ChestBoar.Crafting.Add(CraftingTable.Workbench, 1);
        var ChestData = ChestBoar.Prefab.GetComponent<ItemDrop>();
        ChestData.m_itemData.m_durability = 100;
        ChestData.m_itemData.m_shared.m_maxQuality = 4;
        ChestData.m_itemData.m_shared.m_weight = 5;
        ChestData.m_itemData.m_shared.m_armor = 1;
        ChestData.m_itemData.m_shared.m_armorPerLevel = 2;
        ChestData.m_itemData.m_shared.m_setName = "Boar";
        ChestData.m_itemData.m_shared.m_setSize = 3;
        ChestData.m_itemData.m_shared.m_setStatusEffect = BoarSE;

        var LegBoar = new Item("shapeshiftbundle", "ArmorBoarLegs_RS");
        LegBoar.Name.English("Boar Greaves");
        LegBoar.Description.English("Rough and patchy pair of greaves");
        LegBoar.RequiredItems.Add("Shapeshift_Boar_item", 1);
        LegBoar.RequiredUpgradeItems.Add("LeatherScraps", 10);
        LegBoar.Crafting.Add(CraftingTable.Workbench, 1);
        var LegData = LegBoar.Prefab.GetComponent<ItemDrop>();
        LegData.m_itemData.m_durability = 100;
        LegData.m_itemData.m_shared.m_maxQuality = 4;
        LegData.m_itemData.m_shared.m_weight = 5;
        LegData.m_itemData.m_shared.m_armor = 1;
        LegData.m_itemData.m_shared.m_armorPerLevel = 2;
        LegData.m_itemData.m_shared.m_setName = "Boar";
        LegData.m_itemData.m_shared.m_setSize = 3;
        LegData.m_itemData.m_shared.m_setStatusEffect = BoarSE;

        if (FindPrefab("HelmetTrollLeather") is { } trollHelm)
        {
            var renderers = new List<Renderer>();
            renderers.Add(HelmetBoar.Prefab.GetComponentInChildren<SkinnedMeshRenderer>(true));
            renderers.Add(ChestBoar.Prefab.GetComponentInChildren<SkinnedMeshRenderer>(true));
            renderers.Add(LegBoar.Prefab.GetComponentInChildren<SkinnedMeshRenderer>(true));
            renderers.Add(HelmetBoar.Prefab.GetComponentInChildren<MeshRenderer>());
            renderers.Add(ChestBoar.Prefab.GetComponentInChildren<MeshRenderer>());
            renderers.Add(LegBoar.Prefab.GetComponentInChildren<MeshRenderer>());

            var shader = trollHelm.GetComponentInChildren<SkinnedMeshRenderer>(true).material.shader;

            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    mat.shader = shader;
                }
            }
        }
    }
}