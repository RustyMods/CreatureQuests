using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CreatureQuests.QuestSystem;
using CreatureQuests.QuestSystem.Behaviors;
using CreatureQuests.Source;
using HarmonyLib;
using ServerSync;
using Shapeshift.QuestSystem.Behaviors;
using Shapeshift.Source;
using ShapeShiftManager;
using UnityEngine;

namespace CreatureQuests
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class CreatureQuestsPlugin : BaseUnityPlugin
    {
        internal const string ModName = "CreatureQuests";
        internal const string ModVersion = "0.0.3";
        internal const string Author = "RustyMods";
        private const string ModGUID = Author + "." + ModName;
        private static readonly string ConfigFileName = ModGUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource CreatureQuestsLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        public static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
        public enum Toggle { On = 1, Off = 0 }

        private static readonly AssetBundle Assets = GetAssetBundle("ravenquestbundle");
        public static readonly GameObject m_thirdEye = Assets.LoadAsset<GameObject>("thirdeye");
        private static AssetBundle GetAssetBundle(string fileName)
        {
            Assembly execAssembly = Assembly.GetExecutingAssembly();
            string resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
            using Stream? stream = execAssembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(stream);
        }
        public static CreatureQuestsPlugin plugin = null!;
        public static GameObject m_root = null!;
        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        public static GameObject m_raven = null!;
        public static ConfigEntry<Vector2> _hudPosition = null!;
        public static ConfigEntry<float> _UIScale = null!;
        public static ConfigEntry<KeyCode> _spawnRavenKey = null!;
        public static ConfigEntry<Toggle> _enableQuests = null!;
        public static ConfigEntry<Toggle> _hidePlayerName = null!;
        public static ConfigEntry<Toggle> _canDropRelics = null!;
        public static ConfigEntry<float> _relicDropChance = null!;
        public static ConfigEntry<KeyCode> _transformationTrigger = null!;
        public void Awake()
        {
            plugin = this;
            m_root = new GameObject("root");
            DontDestroyOnLoad(m_root);
            m_root.SetActive(false);
            Localizer.Load();
            var manager = new CreatureFormManager(m_root);
            manager.LoadDefaultCreatures();
            Transformation.Setup();
            InitConfigs();
            DefaultQuests.Setup();
            QuestMaker.Setup();
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void InitConfigs()
        {
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
            
            _hudPosition = config("2 - Settings", "HUD Overlay", new Vector2(1680f, -250f), "Set position of Quest overlay");
            _UIScale = config("2 - Settings", "UI Scale", 1f, new ConfigDescription("Set scale of UI", new AcceptableValueRange<float>(0f, 2f)));
            _spawnRavenKey = config("2 - Settings", "Spawn Key", KeyCode.F4, "Set the key code to spawn/despawn quest raven", false);
            _enableQuests = config("2 - Settings", "Quest System", Toggle.On, "If on, quest system is active");
            _hidePlayerName = config("2 - Settings", "Hide Player Name", Toggle.Off, "If on, when player is a creature, hide name tag");
            _canDropRelics = config("2 - Settings", "Drop Relics", Toggle.On, "If on, creatures drop their relic once relic is known");
            _relicDropChance = config("2 - Settings", "Relic Drop Chance", 1f, new ConfigDescription("chance to drop relic", new AcceptableValueRange<float>(0f, 100f)));
            _transformationTrigger = config("2 - Settings", "Shapeshift Trigger", KeyCode.G,
                "Keycode to trigger transform when wearing creature set", false);
        }

        private void Update()
        {
            if (!Player.m_localPlayer) return;
            if (NotReady()) return;
            UpdateQuestRaven();
            UpdateSetTransformation();
        }

        private static bool NotReady()
        {
            return !Player.m_localPlayer || Menu.IsVisible() || Chat.instance.IsChatDialogWindowVisible() || Player.m_localPlayer.IsDead() ||
                   Player.m_localPlayer.InIntro() || Player.m_localPlayer.IsTeleporting() || InventoryGui.IsVisible() ||
                   Console.IsVisible() || StoreGui.IsVisible();
        }
        private static void UpdateSetTransformation()
        {
            if (!Input.GetKeyDown(_transformationTrigger.Value)) return;
            var shapeShifted = CreatureFormManager.IsCreatureForm(Player.m_localPlayer.name);
            if (shapeShifted) CreatureFormManager.Revert(Player.m_localPlayer);
            else CreatureFormManager.TriggerTransformation(Player.m_localPlayer, CreatureSet.m_selectedForm, 0f);
        }
        private static void UpdateQuestRaven()
        {
            if (_enableQuests.Value is Toggle.Off)
            {
                QuestRaven.ToggleFly(false);
            }
            else
            {
                if (!ZNetScene.instance || !ZNetScene.instance.enabled) return;
                
                if (!QuestManager.FirstQuestTriggered)
                {
                    QuestRaven.ToggleFly(true);
                    return;
                }

                if (!Input.GetKeyDown(_spawnRavenKey.Value)) return;
                if (QuestRaven.m_instance is null) QuestRaven.ToggleFly(true);
                else QuestRaven.ToggleFly(!QuestRaven.m_instance.IsSpawned());
            }
        }
        
        // used to invoke delayed
        public void ToggleFly()
        {
            if (!m_raven || !Player.m_localPlayer) return;
            if (QuestRaven.m_instance is null) Instantiate(m_raven, Player.m_localPlayer.transform.position, Quaternion.identity);
            if (QuestRaven.m_instance is null) return;
            QuestRaven.m_instance.m_active = !QuestRaven.m_instance.IsSpawned();
        }
        
        public void SpawnBounty()
        {
            if (QuestManager.m_currentQuest is not { Data.BountyData.m_critter: {} creature } quest) return;
            GameObject critter = Instantiate(creature, quest.Data.BountyData.m_pos, Quaternion.identity);
            critter.name = "Bounty";
            Bounty component = critter.AddComponent<Bounty>();
            component.ApplyModifiers(quest.Data.BountyData);
            Bounty.DoneSpawnEffectList.Create(critter.transform.position, Quaternion.identity);
            quest.m_currentBounty = component;
        }

        public void Spawn()
        {
            if (QuestManager.m_currentQuest is not { } quest) return;
            if (ZNetScene.instance.GetPrefab(quest.Data.PrefabName) is not { } prefab) return;
            Instantiate(prefab, Player.m_localPlayer.transform.position, Quaternion.identity);
            Bounty.DoneSpawnEffectList.Create(prefab.transform.position, Quaternion.identity);
        }

        public static void InitRaven()
        {
            GameObject? ravens = Resources.FindObjectsOfTypeAll<GameObject>().ToList().Find(x => x.name == "Ravens" && x.transform.GetChild(0).name == "Hugin");
            if (ravens is null) return;
            m_raven = Instantiate(ravens.transform.GetChild(0).gameObject, m_root.transform, false);
            m_raven.name = "Overseer_QuestGiver";
            if (Utils.FindChild(m_raven.transform, "Jaw") is { } jaw)
            {
                GameObject thirdEye = Instantiate(m_thirdEye, jaw);
                thirdEye.transform.localPosition = new Vector3(0.0004f, 0.0033f, 0.0116f);
                thirdEye.transform.localRotation = new Quaternion(-51.949f, 134.846f, 61.205f, 0f);
                thirdEye.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
            QuestRaven questComponent = m_raven.AddComponent<QuestRaven>();
            if (m_raven.TryGetComponent(out Raven component))
            {
                questComponent.m_visual = component.m_visual;
                questComponent.m_exclamation = component.m_exclamation;
                questComponent.m_idleEffect = component.m_idleEffect;
                questComponent.m_deSpawnEffect = component.m_despawnEffect;
                Destroy(component);
            }
        }
        private void OnDestroy() => Config.Save();

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                CreatureQuestsLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                CreatureQuestsLogger.LogError($"There was an issue loading your {ConfigFileName}");
                CreatureQuestsLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        public ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        public ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        // private class ConfigurationManagerAttributes
        // {
        //     [UsedImplicitly] public int? Order;
        //     [UsedImplicitly] public bool? Browsable;
        //     [UsedImplicitly] public string? Category;
        //     [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer;
        // }
    }
}