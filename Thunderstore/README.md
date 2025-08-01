# 🐾 CreatureQuests
CreatureQuests is a gameplay-enhancing plugin that allows players to shapeshift into Valheim's vanilla creatures and take on immersive, form-specific quests. It introduces a flexible quest system designed to create engaging side missions that evolve with the player’s transformation—whether you're hunting as a wolf, swimming as a neck, or prowling as a troll.

Perfect for story-driven servers, progression-based playthroughs, or simply adding new challenges to your Viking adventures.

Call upon the overseer raven to begin your firsts quests use the hotkey `F4`

# 🧭 QuestMaker System

The `QuestMaker` class allows you to define, read, and sync custom creature quests from `.yml` files for your Valheim mod. It uses Odin's raven to distribute quests to players, parsing your YAML files into structured, synced quest data.

## 📂 Folder Structure

At launch, the mod auto-generates a folder here:

```
BepInEx/config/CreatureQuests/Quests
```

Any `.yml` file placed inside this folder (except `Defaults.yml`) will be automatically loaded, parsed into quest blocks, and synchronized with clients.

## 📜 YAML Quest Format

Each quest must be written in a specific format inside a `.yml` file. A single file can contain multiple quests. Each quest block starts with a title line in square brackets:  
```yaml
[10.Moonburn]
```

### ✅ Valid Quest Block Fields:

| Field         | Description                                                                 |
|---------------|-----------------------------------------------------------------------------|
| `Description:`| Text that describes the quest.                                               |
| `CreatureForm:`| Required shapeshift form to undertake the quest.                            |
| `Duration:`   | Quest duration in seconds.                                                   |
| `QuestType:`  | Defines the quest goal, with arguments. Supports `Kill`, `Harvest`, `Run`, etc. |
| `Event:`      | Optional spawns for event-style quests. Format: `Prefab, MinLevel, MaxLevel`. |
| `EventDuration:` | How long the event should last.                                         |
| `Reward:`     | Reward format: `Item(name, amount, quality, variant)` or `Skill(type, xp)`. |
| `Start:`      | Dialog text shown when accepting the quest. Can include commands.            |
| `Cancel:`     | Dialog shown on cancellation.                                                |
| `Completed:`  | Dialog shown on completion.                                                  |

## ✍️ Example Quest

```yaml
[20.Moonburn]
Description: Cast out from the firelit caves, you prowl the peaks with frozen fury.
CreatureForm: Fenring
Duration: 1000
Event: Fenring_Cultist, 1, 3 | Ulv, 1, 2
EventDuration: 1000
QuestType: Kill(Fenring_Cultist, 10, 1)
Reward: Item(Shapeshift_Fenring_item, 2, 1, 0) | Skill(Run, 10)
Start: They called you feral. You called them kin.
Start: Now their fire burns where the moon once ruled.
Start: They’ll bleed for every howl they silenced. | Not yet. The hunt waits. | Command(Accept)
Cancel: Leave vengeance buried? | Even wolves can fear fire. | Command(Cancel)
Completed: The cultists lie broken. The cold returns to your caves.
Completed: Take this, moonborn avenger. | Return when old scars reopen. | Command(Collect)
```
```yaml
[<int:Index>.<string:Title>]
Description: <string:Description>
CreatureForm: <string:ShapeshiftFormPrefab>
Duration: <float:Seconds>

# Optional: Spawns creatures during the quest
Event: <string:PrefabName>, <int:MinLevel>, <int:MaxLevel> | <...additional entries>
EventDuration: <float:Seconds>

# Required: Defines the quest's goal
QuestType: Kill(<string:PrefabName>, <int:Amount>, <int:RequiredLevel>)
QuestType: Harvest(<string:PrefabName>, <int:Amount>)
QuestType: Farm(<string:PrefabName>, <int:Amount>)
QuestType: Destructible(<string:PrefabName>, <int:Amount>)
QuestType: Tame(<string:PrefabName>, <int:Amount>)
QuestType: Procreate(<string:PrefabName>, <int:Amount>)
QuestType: Run(<int:Distance>)
QuestType: Jump(<int:Times>)
QuestType: Fly(<int:Times>)
QuestType: Attach(<int:Times>) // only used with tick, as they attach to creatures when attacking
QuestType: Travel(<int:Times>)

# Optional: Rewards granted upon quest completion
Reward: Item(<string:PrefabName>, <int:Amount>, <int:Quality>, <int:Variant>)
Reward: Skill(<string:SkillType>, <float:ExperienceAmount>)

```
## 📣 How Events Work
- Triggered on Quest Start: When a player accepts a quest that includes an Event, the event is immediately triggered.
- Dynamic Encounter Zone: The event area is centered on and follows the player who accepted the quest, allowing for dynamic encounters that move with the player.
- Failure on Death: If the player dies before completing the quest, the event is automatically canceled.
- Cleanup on Failure: All creatures spawned by the event are instantly removed if the event ends prematurely, ensuring the world remains clean and balanced.

## 🔁 Live Reloading

The system watches for `.yml` file changes. When a file is added, updated, or removed:

- The server re-reads all files.
- Data is parsed and synced across clients using a `CustomSyncedValue`.

No need to restart your server after editing quests!

## 🧪 Creating New Quests

You can create a new `.yml` file with a name like `Wolves.yml`, and add one or more quest blocks:

```yaml
[01.FirstHunt]
Description: Prove yourself by taking the form of a wolf and hunting boar.
CreatureForm: Wolf
Duration: 600
QuestType: Kill(Boar, 5, 1)
Reward: Item(Meat, 5, 1, 0)
Start: The forest tests its young. Will you pass? | Maybe another time | Command(Accept)
Completed: You return, muzzle red and heart proud.
```

## 💾 Defaults File

- The mod will auto-generate a `Defaults.yml` to store all quests currently loaded into memory.
- This is useful for backups or reference.
- If you make changes, make sure to rename this file, so it is not overwritten

## ⚠️ Notes

- Dialog lines with `|` denote branching or optional command prompts (`Accept`, `Cancel`, `Collect`).
- Events spawn mobs temporarily during the quest.
- Only servers execute quest reloads and broadcast updates.

# 🧭 Items
Transformation items
```
Shapeshift_Boar_item
Shapeshift_Neck_item
Shapeshift_Greyling_item
Shapeshift_Eikthyr_item
Shapeshift_Deer_item
Shapeshift_Ghost_item
Shapeshift_Greydwarf_item
Shapeshift_Greydwarf_Shaman_item
Shapeshift_Greydwarf_Elite_item
Shapeshift_Troll_item
Shapeshift_Skeleton_item
Shapeshift_Skeleton_Hildir_item
Shapeshift_Skeleton_Poison_item
Shapeshift_gd_king_item
Shapeshift_Abomination_item
Shapeshift_Leech_item
Shapeshift_Wraith_item
Shapeshift_Draugr_item
Shapeshift_Draugr_Ranged_item
Shapeshift_Draugr_Elite_item
Shapeshift_BogWitchKvastur_item
Shapeshift_Blob_item
Shapeshift_BlobElite_item
Shapeshift_Surtling_item
Shapeshift_Bonemass_item
Shapeshift_Fenring_item
Shapeshift_Fenring_Cultist_item
Shapeshift_Fenring_Cultist_Hildir_item
Shapeshift_Wolf_item
Shapeshift_Wolf_cub_item
Shapeshift_StoneGolem_item
Shapeshift_Bat_item
Shapeshift_Hatchling_item
Shapeshift_Ulv_item
Shapeshift_Dragon_item
Shapeshift_Deathsquito_item
Shapeshift_Goblin_item
Shapeshift_GoblinBrute_item
Shapeshift_GoblinShaman_item
Shapeshift_GoblinBruteBros_item
Shapeshift_BlobTar_item
Shapeshift_Lox_item
Shapeshift_Lox_Calf_item
Shapeshift_GoblinKing_item
Shapeshift_Hare_item
Shapeshift_Dverger_item
Shapeshift_DvergerMageFire_item
Shapeshift_DvergerMageIce_item
Shapeshift_DvergerMageSupport_item
Shapeshift_Chicken_item
Shapeshift_Hen_item
Shapeshift_Seeker_item
Shapeshift_SeekerBrute_item
Shapeshift_Gjall_item
Shapeshift_Tick_item
Shapeshift_SeekerBrood_item
Shapeshift_SeekerQueen_item
Shapeshift_Morgen_item
Shapeshift_FallenValkyrie_item
Shapeshift_TrollSummoned_item
Shapeshift_Asksvin_item
Shapeshift_Asksvin_hatchling_item
Shapeshift_Volture_item
Shapeshift_Fader_item
Shapeshift_CharredMelee_Dyrnwyn_item
Shapeshift_Charred_Melee_item
Shapeshift_Charred_Archer_item
Shapeshift_Charred_Mage_item
Shapeshift_Charred_Twitcher_item
Shapeshift_BonemawSerpent_item
Shapeshift_Serpent_item
```
Armor Set
```
HelmetValkyrie_RS
ArmorValkyrieChest_RS
ArmorValkyrieLegs_RS
CapeValkyrie_RS
```
Set Effect:
Transform into creatures at will., in chat, say: `/wyrdform Boar` or just `/wyrdform` to revert

the trigger to transform uses the localized name of the creature `/wyrdform seeker soldier`

# Commands
```
shapeshift help
shapeshift set <string:PrefabName>
shapeshift reset
ravenquest help
ravenquest info <int:QuestIndex>
ravenquest list
ravenquest start <int:QuestIndex>
```

# 🐉 Custom Creatures – Export Guide
This feature allows you to export creature data to enable custom shapeshifting forms. 
Intended for advanced users who want to create creature-based transformations.

## 🔧 How to Export a Creature
Use the following console command:
`shapeshift export <PrefabName>`

This will generate the necessary files and folder structure for customizing the creature.

## 📂 Output Structure
After running the command, a folder named CustomCreatures will be created (if it doesn't already exist), with the following contents:
```
CustomCreatures/
└── Exported/
    ├── <PrefabName>.yml
    └── <PrefabName>_DATA.yml
```
## 📄 File Breakdown
`PrefabName.yml` contains the creature’s bone structure, a list of items found in the character component.

Use this file for reference when setting up your custom shapeshift behavior.

`PrefabName_DATA.yml` defines all parameters required for shapeshift behavior. You’ll need to edit this file to configure:

`Primary`, `secondary`, and `block` attacks

The correct head bone (if it’s not named `Head`)

Any other transformation logic

## 🚀 How to Enable the Custom Creature
Move the edited `_DATA.yml` file into the main CustomCreatures/ folder (i.e., outside Exported/).

It will then load automatically during startup.

## ⚙️ Example Configuration
Here is a sample _DATA.yml file for a custom creature from the MonsterLabz mod:

```yaml
PrefabName: FireSkeletonWarrior
OverrideName: ''
PrimaryAttack: ''
SecondaryAttack: ''
BlockAttack: ''
UsePlayerAnimator: true
HideWeapons: false
HideArmor: false
HideHelmet: false
RequireWeaponEquipped: false
RequireRangedWeaponEquipped: false
WaterCreature: false
FlyingCreature: false
CameraMaxDistance: 8
HeadTransform: Head
EnableUseItem: false
UseItem: ''
Duration: 600
```
## 💡 Notes
```yaml
OverrideName: Set this if you want to display a custom name for the form.
UseItem: Define an item that triggers the transformation (similar to relics).
Duration: Length of the transformation in seconds.
HeadTransform: Rename if the head bone is named differently in your model.
```
Weapons/armor can be hidden if the creature uses its own visual design.
