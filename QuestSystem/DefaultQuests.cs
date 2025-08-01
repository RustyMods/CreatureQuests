using static CreatureQuests.QuestSystem.QuestManager;
using static CreatureQuests.QuestSystem.QuestManager.Quest.QuestData;

namespace CreatureQuests.QuestSystem;

public static class DefaultQuests
{
    private static void MeadowQuests()
    {
        Quest BoarsCraving = new Quest("Boar's Craving", "Satisfy your primal hunger by gathering raspberries.", QuestType.Harvest);
        BoarsCraving.RequiredBiome = Heightmap.Biome.Meadows;
        BoarsCraving.RequiredShapeshiftForm = "Boar";
        BoarsCraving.Duration = 600f;
        BoarsCraving.Data.Set("RaspberryBush", 50);
        BoarsCraving.Rewards.Set("Shapeshift_Boar_item", 1);
        BoarsCraving.AddStartText("The hunger gnaws at you... a craving older than memory.");
        BoarsCraving.AddStartText("Take on the form of the wild boar, and seek the fruit that once sustained the first herd.");
        BoarsCraving.AddStartText("Will you let the beast within guide your path to the raspberries?");
        BoarsCraving.AddStartAction("Let the transformation begin!", "Well, perhaps another time");
        BoarsCraving.AddCancelText("The craving fades... but the wild still whispers your name.");
        BoarsCraving.AddCancelAction("Do you abandon the form of the boar and turn from the fruit of the forest?", "Good luck!");
        BoarsCraving.AddCompletedText("You return, belly full and tusks stained with berry juice.");
        BoarsCraving.AddCompletedText("The wild boar slumbers within, satisfied—for now.");
        BoarsCraving.AddCompletedAction("Will you accept your reward and let the hunger rest?", "Call upon me before the wyrdform times out to collect your reward!");

        Quest BoarLove = new Quest("Boarheart’s Bond", "Find a mate", QuestType.Tame);
        BoarLove.RequiredShapeshiftForm = "Boar";
        BoarLove.Duration = 600f;
        BoarLove.Data.Set("Boar", 1);
        BoarLove.Rewards.Set("Shapeshift_Boar_item", 1);
        BoarLove.AddStartText("The meadows are no place for the lonely. Even a beast needs a companion beneath the stars.");
        BoarLove.AddStartText("In the wilds, love is a shield and strength. Will you seek the sow destined for your snout?");
        BoarLove.AddStartAction("Shift into your boar-self and prove your worth in the sacred dance of tusks!", "Bah, perhaps love can wait... for now.");
        BoarLove.AddCancelText("You retreat? Does your heart fear the battlefield of affection?");
        BoarLove.AddCancelAction("Is your will so weak you abandon the hunt for love?", "No! A true boar fights for love!");
        BoarLove.AddCompletedText("By Odin’s beard! You’ve wooed your match with honor!");
        BoarLove.AddCompletedAction("Take this gift, love-born warrior. Even the gods smile upon you.", "Return before your tusks grow weary!");

        Quest BoarMate = new Quest("Boarborn", "Secure your future, prove your love.", QuestType.Procreate);
        BoarMate.RequiredShapeshiftForm = "Boar";
        BoarMate.Duration = 600f;
        BoarMate.Data.Set("Boar", 1);
        BoarMate.Rewards.Set("Shapeshift_Boar_item", 1);
        BoarMate.AddStartText("Now that your heart is bound, it’s time to forge a legacy worthy of Valhalla!");
        BoarMate.AddStartText("The blood of boars must run strong. Go forth and sow the seed of your saga.");
        BoarMate.AddStartAction("Embrace your lover, wild one, and let your strength echo through generations!", "My flame wanes... another time, perhaps.");
        BoarMate.AddCancelText("Even beasts carry the burden of legacy. Will yours be forgotten?");
        BoarMate.AddCancelAction("Will you let your line perish like frost on stone?", "Nay! Your spirit is strong, your bloodline must thrive!");
        BoarMate.AddCompletedText("The meadows will sing songs of your lineage!");
        BoarMate.AddCompletedAction("Take this reward, great boar of destiny. You’ve earned your place in the saga.", "Don’t let the mead dull your tusks—return soon!");

        Quest Sprinter = new Quest("Whispers of the Wind", "Race the breeze across the open Meadows, swift as thought and silent as shadow.", QuestType.Run);
        Sprinter.RequiredShapeshiftForm = "Deer";
        Sprinter.RequiredBiome = Heightmap.Biome.Meadows;
        Sprinter.Duration = 600f;
        Sprinter.Data.Set(3000); // Distance to run, adjust as needed
        Sprinter.Rewards.Set("Shapeshift_Deer_item", 1, 1, 0);
        Sprinter.AddStartText("The grass parts before your hooves—run, child of the wind.");
        Sprinter.AddStartText("There is no chase, no fear—only freedom in the sprint.");
        Sprinter.AddStartAction("Dash across the meadows. Let your spirit outpace the sun.", "The breeze can wait a while longer...");
        Sprinter.AddCancelText("The wind moves on. Your hooves stay still.");
        Sprinter.AddCancelAction("Do you forget the joy of running wild?", "No—I will race the wind again!");
        Sprinter.AddCompletedText("The wind bows to you now. Your run has stirred the land.");
        Sprinter.AddCompletedText("You’ve flown over fields like a whisper—untouchable and alive.");
        Sprinter.AddCompletedAction("Take your reward, swift one of the glade.", "Let me catch my breath before I claim it.");

        Quest NeckTrial = new Quest("Trial of the Marshspawn", "Eliminate your kin to prove your strength.", QuestType.Kill);
        NeckTrial.RequiredShapeshiftForm = "Neck";
        NeckTrial.Duration = 600f; // 10 minutes
        NeckTrial.Data.Set("Neck", 5); // Kill 5 Neck creatures
        NeckTrial.Rewards.Set("Shapeshift_Neck_item", 1, 1, 0); // Or reward with something more exotic later
        NeckTrial.AddStartText("The marshes call to you, hatchling. A trial awaits.");
        NeckTrial.AddStartText("Only by culling the weak may you claim your place among the true spawn.");
        NeckTrial.AddStartText("Transform into your primal self and assert your dominance.");
        NeckTrial.AddStartAction("Begin the trial of blood and slime.", "No, not yet...");
        NeckTrial.AddCancelText("You turn your back on the marshes, sparing the kin you once knew.");
        NeckTrial.AddCancelAction("Do you abandon the rite of the Marshspawn?", "Good luck!");
        NeckTrial.AddCompletedText("You return victorious, the slime of your foes drying on your scales.");
        NeckTrial.AddCompletedText("The marsh accepts your dominance. The trial is complete.");
        NeckTrial.AddCompletedAction("Claim your reward before your wyrdform fades.", "Return before time runs out!");

        Quest DandelionRitual = new Quest("Whispers on the Wind", "Gather dandelions scattered across the fields.", QuestType.Harvest);
        DandelionRitual.RequiredShapeshiftForm = "Greyling";
        DandelionRitual.Duration = 600f;
        DandelionRitual.Data.Set("Pickable_Dandelion", 20); // Harvest 20 dandelions
        DandelionRitual.Rewards.Set("Shapeshift_Greyling_item", 1, 1, 0);
        DandelionRitual.AddStartText("A breeze stirs through the grass... it carries forgotten whispers.");
        DandelionRitual.AddStartText("Take the form of the Greyling and honor the forest by gathering its softest seed.");
        DandelionRitual.AddStartText("Will you walk the meadows as one of the unseen, and listen to the wind?");
        DandelionRitual.AddStartAction("Let the ritual begin.", "Not right now.");
        DandelionRitual.AddCancelText("The forest quiets. Its breath slows. The ritual may yet return.");
        DandelionRitual.AddCancelAction("Do you abandon the whispers and break the wind’s trust?", "Good luck!");
        DandelionRitual.AddCompletedText("You return, hands golden with the forest’s breath.");
        DandelionRitual.AddCompletedText("The wind sings your name, and the meadows sway in thanks.");
        DandelionRitual.AddCompletedAction("Will you accept the reward granted by the breeze?", "Only if the ritual completes in time...");
        
        Quest MeadowVigil = new Quest("Vigil of the Meadow", "Drive back the Greydwarfs who trespass where forest ends and meadow begins.", QuestType.Kill);
        MeadowVigil.RequiredDefeatKey = "defeated_eikthyr";
        MeadowVigil.RequiredShapeshiftForm = "Eikthyr";
        MeadowVigil.Duration = 500f;
        MeadowVigil.Data.Set("Greydwarf", 10);
        MeadowVigil.Data.AddEventSpawn("Greydwarf");
        MeadowVigil.Data.EventDuration = 500f;
        MeadowVigil.Rewards.Set("Shapeshift_Eikthyr_item", 1, 1, 0);

        MeadowVigil.AddStartText("The meadows tremble. The forest creeps too far.");
        MeadowVigil.AddStartText("Greydwarfs cross the threshold. You must be the antlered answer.");
        MeadowVigil.AddStartAction("They’ll learn where the forest ends.", "Let them roam… for now.");
        MeadowVigil.AddCancelText("The wild things still march. The meadow remains unguarded.");
        MeadowVigil.AddCancelAction("Will you leave the open lands to choke?", "Another time. The grass still bends.");
        MeadowVigil.AddCompletedText("The meadows breathe again. The invaders retreat to their roots.");
        MeadowVigil.AddCompletedText("With each fallen Greydwarf, the sky widened and the meadow sighed.");
        MeadowVigil.AddCompletedAction("Take this, horned warden of the wind-touched fields.", "Return when the hooves of war echo once more.");
    }

    private static void BlackForestQuests()
    {
        Quest TrollsToll = new Quest("The Troll's Toll", "Strike copper veins with earth-shaking fury.", QuestType.Destructible);
        TrollsToll.RequiredDefeatKey = "KilledTroll";
        TrollsToll.RequiredShapeshiftForm = "Troll";
        TrollsToll.Duration = 1200f; // 20 minutes
        TrollsToll.Data.Set("rock4_copper", 50); // Hit copper vein 50 times
        TrollsToll.Rewards.Set("Shapeshift_Troll_item", 1, 1, 0);
        TrollsToll.AddStartText("The bones of the earth lie rich beneath the stone.");
        TrollsToll.AddStartText("Take the form of a Troll, and with mighty hands, awaken the copper slumbering below.");
        TrollsToll.AddStartAction("Let the mountain feel your fury!", "I’ll crush stone another time.");
        TrollsToll.AddCancelText("The mountain grows silent once more...");
        TrollsToll.AddCancelAction("Do you forsake the strength of the deep?", "I still feel the tremors, I’ll return.");
        TrollsToll.AddCompletedText("The earth groans in satisfaction. You have struck true.");
        TrollsToll.AddCompletedText("Copper veins gleam where your wrath once landed.");
        TrollsToll.AddCompletedAction("Take this token, giant of stone and might.", "May your fists never dull!");

        Quest EchoesInTheOre = new Quest("Echoes in the Ore", "Strike tin with death’s patience.", QuestType.Destructible);
        EchoesInTheOre.RequiredShapeshiftForm = "Skeleton";
        EchoesInTheOre.Duration = 900f; // 15 minutes
        EchoesInTheOre.Data.Set("MineRock_Tin", 10); // Hit tin deposit 10 times
        EchoesInTheOre.Rewards.Set("Shapeshift_Skeleton_item", 1, 1, 0);
        EchoesInTheOre.AddStartText("Even in death, the veins of the world call to you.");
        EchoesInTheOre.AddStartText("Take the shape of bone and shadow, and draw tin from the living stone.");
        EchoesInTheOre.AddStartAction("Let the clatter of your strikes echo eternal.", "Bones can wait… for now.");
        EchoesInTheOre.AddCancelText("The mine grows cold, untouched by your skeletal hand.");
        EchoesInTheOre.AddCancelAction("Have you lost the will to unearth the echoes?", "No, I’ll return with a rattle.");
        EchoesInTheOre.AddCompletedText("The ore is gathered, and the silence returns.");
        EchoesInTheOre.AddCompletedText("Your bony touch has awakened forgotten veins.");
        EchoesInTheOre.AddCompletedAction("Take this reward, forged from patience and decay.", "May your sockets always gleam.");
        
        Quest SeedOfTheAncients = new Quest("Seed of the Ancients", "Root a new life in the forest. Guard it, lest the world forget its name.", QuestType.Farm);
        SeedOfTheAncients.RequiredShapeshiftForm = "Greydwarf";
        SeedOfTheAncients.Duration = 600f;
        SeedOfTheAncients.Data.Set("FirTree_Sapling", 10); 
        SeedOfTheAncients.Rewards.Set("Shapeshift_Greydwarf_item", 1, 1, 0);
        SeedOfTheAncients.AddStartText("The forest speaks in whispers—one tree must stand where none dare grow.");
        SeedOfTheAncients.AddStartText("As Greydwarf, plant the sacred fir and defend it from fire, axe, and claw.");
        SeedOfTheAncients.AddStartAction("Let the roots take hold. I will guard it with my life.", "The forest can wait…");
        SeedOfTheAncients.AddCancelText("The soil remains bare, and silence falls where life might have sung.");
        SeedOfTheAncients.AddCancelAction("Will you leave the grove to rot?", "I… I’m not ready to plant hope.");
        SeedOfTheAncients.AddCompletedText("The sapling stands tall, untouched. A new root in ancient soil.");
        SeedOfTheAncients.AddCompletedText("Life stirs in the shadow of your ward. The forest remembers.");
        SeedOfTheAncients.AddCompletedAction("Take your reward, warden of the wilds.", "Return when the land calls again.");
        
        Quest ThistleRite = new Quest("Rite of Thorns", "Draped in bark and shadow, heed the call of the forest's breath. Seek the bitter bloom that wards decay and feeds forgotten rites.", QuestType.Harvest);
        ThistleRite.RequiredShapeshiftForm = "Greydwarf_Shaman";
        ThistleRite.Duration = 600f;
        ThistleRite.Data.Set("Pickable_Thistle", 30);
        ThistleRite.Rewards.Set("Shapeshift_Greydwarf_Shaman_item", 1, 1, 0);
        ThistleRite.AddStartText("The forest bleeds blue. Thistle binds wounds the world cannot see.");
        ThistleRite.AddStartText("As a shaman, you know its worth. Gather the sacred stalks for the rites to begin.");
        ThistleRite.AddStartAction("I’ll walk the path of pain and growth.", "Another time. The thorns still sting.");
        ThistleRite.AddCancelText("Without thistle, the forest's rites wither and fade.");
        ThistleRite.AddCancelAction("Turn away from the old ways?", "The ritual must wait.");
        ThistleRite.AddCompletedText("Your satchel brims with bitter bloom. The forest readies its rites.");
        ThistleRite.AddCompletedText("The thistle yields. The spirits stir. You have done well.");
        ThistleRite.AddCompletedAction("Take this, druid of dusk and thorn.", "Return when the forest thirsts again.");

        Quest BoneBreaker = new Quest("Bonebreaker", "Let the fury of the forest swell within your limbs. Crush the undead trespassers and grind their bones into the roots.", QuestType.Kill);
        BoneBreaker.RequiredShapeshiftForm = "Greydwarf_Elite";
        BoneBreaker.Duration = 600f;
        BoneBreaker.Data.Set("Skeleton", 10);
        BoneBreaker.Rewards.Set("Shapeshift_Greydwarf_Elite_item", 1, 1, 0);
        BoneBreaker.AddStartText("The dead walk where trees once stood. Their bones mock the silence of the wild.");
        BoneBreaker.AddStartText("Let rage be your roots. Let no skeleton rise from this soil.");
        BoneBreaker.AddStartAction("Their bones will shatter beneath my fists.", "Even fury must rest… for now.");
        BoneBreaker.AddCancelText("The forest groans. The dead still linger.");
        BoneBreaker.AddCancelAction("Leave the bones unbroken?", "Another time. My rage is not yet ripe.");
        BoneBreaker.AddCompletedText("The silence returns. No bones stir. The forest thanks its brute fist.");
        BoneBreaker.AddCompletedText("The dead fall like brittle leaves. You are the storm that breaks them.");
        BoneBreaker.AddCompletedAction("Take this, champion of root and wrath.", "Return when death forgets its place.");
        
        Quest WrathOfTheWood = new Quest("Wrath of the Wood", "Crush the forest’s foes beneath root and stone.", QuestType.Kill);
        WrathOfTheWood.RequiredDefeatKey = "defeated_gdking";
        WrathOfTheWood.RequiredShapeshiftForm = "gd_king";
        WrathOfTheWood.Duration = 800f; // ~13 minutes
        WrathOfTheWood.Data.Set("Troll", 2); // Kill 2 Trolls
        WrathOfTheWood.Rewards.Set("Shapeshift_gd_king_item", 1, 1, 0);
        WrathOfTheWood.AddStartText("The trolls trample sacred ground. The forest cries out.");
        WrathOfTheWood.AddStartText("Become the wrathful root, the living stone. Let the intruders know the price of desecration.");
        WrathOfTheWood.AddStartAction("I rise with bark and fury.", "Their size is great... but not today.");
        WrathOfTheWood.AddCancelText("The trolls mock the stillness. Your silence feeds their fire.");
        WrathOfTheWood.AddCancelAction("Will you let them sunder the old groves?", "Let them roam—for now.");
        WrathOfTheWood.AddCompletedText("The trolls lie broken. The groves breathe easy once more.");
        WrathOfTheWood.AddCompletedText("Your bark is scarred, but your duty fulfilled. The forest whispers your name.");
        WrathOfTheWood.AddCompletedAction("Take your reward, guardian of the deep woods.", "Return when the roots are threatened again.");
    }

    private static void SwampQuests()
    {
        Quest RemainAmongTheLiving = new Quest("Remain Among the Living", "Slay the risen dead before their curse claims you.", QuestType.Kill);
        RemainAmongTheLiving.RequiredShapeshiftForm = "Draugr";
        RemainAmongTheLiving.Duration = 600f; // ~16 minutes
        RemainAmongTheLiving.Data.Set("Skeleton", 50); // Kill 50 skeletons
        RemainAmongTheLiving.Rewards.Set("Shapeshift_Draugr_item", 1, 1, 0);
        RemainAmongTheLiving.AddStartText("The line between you and them grows thin...");
        RemainAmongTheLiving.AddStartText("Don the form of a Draugr and cull the cursed before you join their ranks.");
        RemainAmongTheLiving.AddStartAction("Steel yourself and step into the grave’s shadow!", "I’m not ready to face the dead...");
        RemainAmongTheLiving.AddCancelText("You hesitate, and the dead press closer.");
        RemainAmongTheLiving.AddCancelAction("Will you let their rot claim your soul?", "No... not today!");
        RemainAmongTheLiving.AddCompletedText("The crypts are silent. The dead will not rise tonight.");
        RemainAmongTheLiving.AddCompletedText("You walk still, free of the curse—for now.");
        RemainAmongTheLiving.AddCompletedAction("Claim your reward, revenant slayer.", "Return, if your bones begin to itch...");

        Quest SlimeAscension = new Quest("Slime Ascension", "You are formless, boneless—yet even you seek the sky. The jump is not for speed, but for glory.", QuestType.Jump);
        SlimeAscension.RequiredShapeshiftForm = "Blob";
        SlimeAscension.Duration = 600f;
        SlimeAscension.Data.Set(50); // Number of jumps
        SlimeAscension.Rewards.Set("Shapeshift_Blob_item", 1, 1, 0);
        SlimeAscension.AddStartText("Who says slime can’t soar?");
        SlimeAscension.AddStartText("The sky mocks you. Show it the meaning of bounce.");
        SlimeAscension.AddStartAction("I’ll defy gravity with every jiggle.", "The ground feels... safe.");
        SlimeAscension.AddCancelText("The sky remains untouched. The blob remains earthbound.");
        SlimeAscension.AddCancelAction("Give up the leap?", "Another time. The wind isn’t ready.");
        SlimeAscension.AddCompletedText("You’ve bounced, flopped, and flown. The sky knows your name.");
        SlimeAscension.AddCompletedText("Fifty leaps. A new altitude for the ancient ooze.");
        SlimeAscension.AddCompletedAction("Take this, jumper of the formless path.", "Return when gravity grows bold again.");

        Quest AshesBeneathTheRoots = new Quest("Ashes Beneath the Roots", "Purge the swamp of fire-born Surtlings.", QuestType.Kill);
        AshesBeneathTheRoots.RequiredShapeshiftForm = "Abomination";
        AshesBeneathTheRoots.Duration = 1100f; // ~18 minutes
        AshesBeneathTheRoots.Data.Set("Surtling", 50); // Kill 50 Surtlings
        AshesBeneathTheRoots.Rewards.Set("Shapeshift_Abomination_item", 8, 1, 0);
        AshesBeneathTheRoots.AddStartText("The swamp weeps beneath flames. The fire holes boil with invaders.");
        AshesBeneathTheRoots.AddStartText("Become the ancient bark, and strangle the fire from these lands.");
        AshesBeneathTheRoots.AddStartAction("Let root and rot rise against the flame!", "The fires may rage another day...");
        AshesBeneathTheRoots.AddCancelText("The roots wither as the fire demons dance unchecked.");
        AshesBeneathTheRoots.AddCancelAction("Will you let the flames devour the mire?", "No, the bog still breathes through me!");
        AshesBeneathTheRoots.AddCompletedText("The fires sputter. The swamp exhales in peace.");
        AshesBeneathTheRoots.AddCompletedText("Your limbs carry the silence of ash. The roots thank you.");
        AshesBeneathTheRoots.AddCompletedAction("Take this reward, protector of the peat.", "Return when the bogs burn again.");
    }

    private static void MountainQuests()
    {
        Quest WolfHunt = new Quest("Fangs of the First Hunt", "Take the form of a Wolf and satisfy the call of the hunt by collecting Deer Meat.", QuestType.Kill);
        WolfHunt.RequiredShapeshiftForm = "Wolf";
        WolfHunt.Duration = 600f;
        WolfHunt.Data.Set("Deer", 20); // Collect 10 deer meat (adjust amount as needed)
        WolfHunt.Rewards.Set("Shapeshift_Wolf_item", 5, 1, 0);
        WolfHunt.AddStartText("The moon rises. The wind carries the scent of prey.");
        WolfHunt.AddStartText("Take the form of fang and fury. Let the forest echo with your hunt.");
        WolfHunt.AddStartText("Will you answer the ancient howl and feast upon the flesh of the deer?");
        WolfHunt.AddStartAction("The hunt begins. Let instinct guide your paws.", "The prey can wait for another night...");
        WolfHunt.AddCancelText("The howl dies in your throat. The herd escapes.");
        WolfHunt.AddCancelAction("Do you turn from the hunt and shame the pack?", "No, the wild still stirs within!");
        WolfHunt.AddCompletedText("Your fangs are wet, your belly full. The spirits of the hunt rejoice.");
        WolfHunt.AddCompletedText("You’ve honored the ancient pact between predator and prey.");
        WolfHunt.AddCompletedAction("Take your reward, midnight stalker.", "Let me collect before the full moon fades.");

        Quest WolfBond = new Quest("Bond of the Pack", "Take the form of a Wolf and forge a bond with another of your kind.", QuestType.Tame);
        WolfBond.RequiredShapeshiftForm = "Wolf";
        WolfBond.Duration = 600f;
        WolfBond.Data.Set("Wolf", 1); // Tame one wolf
        WolfBond.Rewards.Set("Shapeshift_Wolf_item", 6, 1, 0);
        WolfBond.AddStartText("The hunt fed your body—now feed your soul.");
        WolfBond.AddStartText("Even the lone wolf longs for a howl beside their own.");
        WolfBond.AddStartAction("Go. Find kin. Let the bond of fang and fur be formed.", "Another time. The wilds are still too quiet...");
        WolfBond.AddCancelText("The pack waits, but you do not come.");
        WolfBond.AddCancelAction("Do you turn your back on brotherhood beneath the moon?", "No—I’ll return to find my kin!");
        WolfBond.AddCompletedText("A second howl joins yours. The bond is sealed.");
        WolfBond.AddCompletedText("Together, your shadows lengthen across the snow.");
        WolfBond.AddCompletedAction("Take your reward, alpha in the making.", "Let me take it before the bond fades...");

        Quest WolfMate = new Quest("Bond of the Pack: New Life", "Take the form of a Wolf and mate with your bonded partner to continue the legacy.", QuestType.Procreate);
        WolfMate.RequiredShapeshiftForm = "Wolf";
        WolfMate.Duration = 1200f; // You can adjust duration as needed
        WolfMate.Data.Set("Wolf", 1); // Mating with bonded wolf partner
        WolfMate.Rewards.Set("Shapeshift_Wolf_cub_item", 10, 1, 0);
        WolfMate.AddStartText("The bond you forged grows stronger, life awaits.");
        WolfMate.AddStartText("Now is the time to create new life under the silver moon.");
        WolfMate.AddStartAction("Seek your bonded mate and begin the sacred ritual.", "Not yet—there is more to prepare.");
        WolfMate.AddCancelText("The call of legacy goes unanswered.");
        WolfMate.AddCancelAction("Will you abandon your kin’s future?", "No—I must find my mate and fulfill the bond!");
        WolfMate.AddCompletedText("New life stirs within the pack.");
        WolfMate.AddCompletedText("Together, you usher in the dawn of a new generation.");
        WolfMate.AddCompletedAction("Receive your reward, progenitor of the pack.", "Let me gather strength before I claim my prize...");

        Quest DrakeDogfight = new Quest("Trial of the Skies", "Take the form of a Hatchling and prove your worth in the sky by defeating your kin.", QuestType.Kill);
        DrakeDogfight.RequiredShapeshiftForm = "Hatchling";
        DrakeDogfight.Duration = 900f;
        DrakeDogfight.Data.Set("Hatchling", 10); // Kill 10 other Hatchlings
        DrakeDogfight.Rewards.Set("Shapeshift_Hatchling_item", 12, 1, 0);
        DrakeDogfight.AddStartText("The sky is not shared—it is claimed in blood.");
        DrakeDogfight.AddStartText("Only one hatchling rises above the storm.");
        DrakeDogfight.AddStartAction("Climb high and unleash your fury on those who would steal your flight.", "No... I am not ready to meet my siblings in battle.");
        DrakeDogfight.AddCancelText("Your wings grow cold with hesitation.");
        DrakeDogfight.AddCancelAction("Will you yield the sky to hatchlings stronger than you?", "Never. I’ll return to show them who truly soars.");
        DrakeDogfight.AddCompletedText("You reign in the frozen thermals, wings unchallenged.");
        DrakeDogfight.AddCompletedText("The air sings with your victory cries—your kin lie scattered beneath the clouds.");
        DrakeDogfight.AddCompletedAction("Claim your reward, fledgling no more.", "Let me taste triumph before it cools.");

        Quest Moonburn = new Quest("Moonburn", "Cast out from the firelit caves, you prowl the peaks with frozen fury. As Fenring, hunt the cultists who turned fang against fang and scorched the old ways.", QuestType.Kill);
        Moonburn.RequiredShapeshiftForm = "Fenring";
        Moonburn.Duration = 600f;
        Moonburn.Data.Set("Fenring_Cultist", 10);
        Moonburn.Rewards.Set("Shapeshift_Fenring_item", 2, 1, 0);
        Moonburn.Data.AddEventSpawn("Fenring_Cultist");
        Moonburn.Data.EventDuration = 600f;
        Moonburn.AddStartText("They called you feral. You called them kin.");
        Moonburn.AddStartText("Now their fire burns where the moon once ruled. Take back the night.");
        Moonburn.AddStartAction("They’ll bleed for every howl they silenced.", "Not yet. The hunt waits.");
        Moonburn.AddCancelText("Their flames still crackle in your place. The caves remain cursed.");
        Moonburn.AddCancelAction("Leave vengeance buried?", "Even wolves can fear fire.");
        Moonburn.AddCompletedText("The cultists lie broken. The cold returns to your caves.");
        Moonburn.AddCompletedText("No more fire chants. No more exile. The mountain howls with you again.");
        Moonburn.AddCompletedAction("Take this, moonborn avenger.", "Return when old scars reopen.");

        Quest GutNest = new Quest("Gut Nest", "The swamp writhes with walking dead. As Ulv, tear the entrails from Draugr corpses and drag them back to weave your nest of bone and bile.", QuestType.Kill);
        GutNest.RequiredShapeshiftForm = "Ulv";
        GutNest.Duration = 600f;
        GutNest.Data.Set("Draugr", 10);
        GutNest.Rewards.Set("Shapeshift_Ulv_item", 2, 1, 0);

        GutNest.AddStartText("The nest is cold. It needs flesh.");
        GutNest.AddStartText("Draugr stink of death, but their guts are strong. Tear, drag, build.");
        GutNest.AddStartAction("I’ll rip and bring. Nest will rise.", "Nest can wait. Hunger stays.");
        GutNest.AddCancelText("No guts. No nest. No warmth.");
        GutNest.AddCancelAction("Leave the mud-things to rot?", "Nest not ready. I hide instead.");
        GutNest.AddCompletedText("Entrails twist and knot. Nest is warm. Nest is mine.");
        GutNest.AddCompletedText("Draugr fall. Swamp feeds the beast. Nest grows.");
        GutNest.AddCompletedAction("Take this, nest-builder of fang and filth.", "Return when gut-hunger stirs.");
        
        Quest CindersOfThePack = new Quest("Cinders of the Pack", "The Ulvs you once bred with fire and shadow now snarl with madness. As a Cultist, you must end them. Burn their fury. Silence their howls. Only ashes remain loyal.", QuestType.Kill);
        CindersOfThePack.RequiredShapeshiftForm = "Fenring_Cultist";
        CindersOfThePack.Duration = 600f;
        CindersOfThePack.Data.Set("Ulv", 10);
        CindersOfThePack.Rewards.Set("Shapeshift_Fenring_Cultist_item", 5, 1, 0);
        CindersOfThePack.Data.AddEventSpawn("Ulv");
        CindersOfThePack.Data.EventDuration = 600f;
        CindersOfThePack.AddStartText("The pack is broken. Their rage no longer obeys.");
        CindersOfThePack.AddStartText("You made them. Now you must unmake them.");
        CindersOfThePack.AddStartAction("They will burn by the same flame that birthed them.", "Not yet. Let the cursed howl a little longer.");
        CindersOfThePack.AddCancelText("Their claws still tear at the mountain. Their eyes still burn.");
        CindersOfThePack.AddCancelAction("Let them live… even wild?", "No. Not yet.");
        CindersOfThePack.AddCompletedText("The howls have quieted. Their rage sleeps in smoke.");
        CindersOfThePack.AddCompletedText("Ashes scatter across the peaks. Your creations have returned to dust.");
        CindersOfThePack.AddCompletedAction("Take this, keeper of the last flame.", "Return when your creations rise again.");

        Quest StoneWrath = new Quest("Wrath of Stone", "Take the form of a StoneGolem and crush the skies by felling 10 Deathsquitos.", QuestType.Kill);
        StoneWrath.RequiredBiome = Heightmap.Biome.Plains;
        StoneWrath.RequiredShapeshiftForm = "StoneGolem";
        StoneWrath.Duration = 1200f;
        StoneWrath.Data.Set("Deathsquito", 10); // Kill 10 Deathsquitos
        StoneWrath.Rewards.Set("Shapeshift_Deathsquito_item", 5, 1, 0);
        StoneWrath.AddStartText("Tiny wings mock you from above. Silence them with stone.");
        StoneWrath.AddStartText("Let the plains quake as your fists reach the sky.");
        StoneWrath.AddStartAction("Crush them, one by one—make their buzzing cease.", "Not yet. Let them fly a little longer...");
        StoneWrath.AddCancelText("The skies still buzz. Your wrath waits, unfinished.");
        StoneWrath.AddCancelAction("Do you let gnats escape the wrath of the mountain?", "No—I will return, and they will fall.");
        StoneWrath.AddCompletedText("The buzzing is no more. The air bows to your silence.");
        StoneWrath.AddCompletedText("Your fists paint the plains in insect ruin.");
        StoneWrath.AddCompletedAction("Take your reward, stone reaper of the skies.", "Let me feel the weight of my victory.");
    }

    private static void OceanQuests()
    {
        Quest DepthClaim = new Quest("Claim the Depths", "You are the coil beneath the waves, the nightmare of sailors, the echo of the world-serpent’s hiss. Hunt another of your kind and drown it in the deep — there is room for only one ruler of the sea.", QuestType.Kill);
        DepthClaim.RequiredShapeshiftForm = "Serpent";
        DepthClaim.Duration = 600f;
        DepthClaim.Data.Set("Serpent", 1);
        DepthClaim.Rewards.Set("Shapeshift_Serpent_item", 5, 1, 0);

        DepthClaim.AddStartText("The sea is not wide enough for two myths.");
        DepthClaim.AddStartText("Another serpent coils through your waters. Drag it to the dark.");
        DepthClaim.AddStartAction("I will strike from beneath and make the sea boil.", "Let it swim. For now.");
        DepthClaim.AddCancelText("The rival still breathes. The sea is split.");
        DepthClaim.AddCancelAction("Spare the false serpent?", "The depths can wait.");
        DepthClaim.AddCompletedText("The waves are still. Your rival is no more. The ocean sings your name.");
        DepthClaim.AddCompletedText("One serpent sinks. One serpent reigns. The sea is yours again.");
        DepthClaim.AddCompletedAction("Take this, lord of the deeps.", "Return when the tides rise in challenge.");
    }

    private static void PlainsQuests()
    {
        Quest LoxHarvest = new Quest("Bounty of the Plains", "Gather the sweet strength of the land by collecting Cloud berries.", QuestType.Harvest);
        LoxHarvest.RequiredBiome = Heightmap.Biome.Plains;
        LoxHarvest.RequiredShapeshiftForm = "Lox";
        LoxHarvest.Duration = 600f;
        LoxHarvest.Data.Set("CloudberryBush", 30); // Collect 30 cloudberries
        LoxHarvest.Rewards.Set("Shapeshift_Lox_item", 1, 1, 0);
        LoxHarvest.AddStartText("The plains are heavy with sun and fruit. Feed your kin.");
        LoxHarvest.AddStartText("Your tusks break no bones today—but the land offers its riches.");
        LoxHarvest.AddStartAction("Trample low and gather the golden fire of the bush.", "Another time—let the berries ripen longer...");
        LoxHarvest.AddCancelText("The fruit withers. Hunger grows.");
        LoxHarvest.AddCancelAction("Will you let your herd starve beneath the sky?", "No—there is still warmth in the earth!");
        LoxHarvest.AddCompletedText("The bounty is gathered. The herd will thrive another season.");
        LoxHarvest.AddCompletedText("You have walked slow, eaten deep, and honored the land.");
        LoxHarvest.AddCompletedAction("Take your reward, for the plains remember their stewards.", "Let me taste victory like the berry’s juice.");

        Quest LoxBond = new Quest("Bond of the Herd", "Tame another of your kind beneath the open sky.", QuestType.Tame);
        LoxBond.RequiredBiome = Heightmap.Biome.Plains;
        LoxBond.RequiredShapeshiftForm = "Lox";
        LoxBond.Duration = 600f;
        LoxBond.Data.Set("Lox", 1); // Tame one lox
        LoxBond.Rewards.Set("Shapeshift_Lox_item", 1, 1, 0);
        LoxBond.AddStartText("Even the strongest beast walks further with a herd.");
        LoxBond.AddStartText("The wind shifts—another heart calls to yours.");
        LoxBond.AddStartAction("Approach with patience and strength. Let trust be your harness.", "Not now—the plains are too quiet...");
        LoxBond.AddCancelText("Your hooves leave no trail beside another.");
        LoxBond.AddCancelAction("Do you fear connection more than battle?", "No—I will return to find my companion!");
        LoxBond.AddCompletedText("Another walks beside you. The herd has grown stronger.");
        LoxBond.AddCompletedText("No longer alone, your steps stir the dust in tandem.");
        LoxBond.AddCompletedAction("Take your reward, keeper of bonds.", "Let me receive it before the moment fades.");

        Quest LoxMate = new Quest("Bond of the Herd: New Lineage", "Mate with your bonded partner to carry on your strength.", QuestType.Procreate);
        LoxMate.RequiredBiome = Heightmap.Biome.Plains;
        LoxMate.RequiredShapeshiftForm = "Lox";
        LoxMate.Duration = 1200f;
        LoxMate.Data.Set("Lox", 1); // Procreate with bonded lox
        LoxMate.Rewards.Set("Shapeshift_Lox_Calf_item", 1, 1, 0);
        LoxMate.AddStartText("Legacy thunders louder than battle. Now is the time.");
        LoxMate.AddStartText("The future takes root when two become more.");
        LoxMate.AddStartAction("Share strength, seed the earth with your line.", "Not yet—the herd must wait a little longer...");
        LoxMate.AddCancelText("The herd holds its breath. New life does not come.");
        LoxMate.AddCancelAction("Will you let your power die with you?", "No—I will return and pass it on!");
        LoxMate.AddCompletedText("A new roar echoes from the grasslands.");
        LoxMate.AddCompletedText("Through you, the plains remember and rejoice.");
        LoxMate.AddCompletedAction("Take your reward, bearer of the next age.", "Let me rest before the next storm rises.");
        
        Quest BeastBreaker = new Quest("Beast Breaker", "You are small, but fury burns hot in your blood. Bring down the mighty Lox.", QuestType.Kill);
        BeastBreaker.RequiredShapeshiftForm = "Goblin";
        BeastBreaker.Duration = 600f;
        BeastBreaker.Data.Set("Lox", 3);
        BeastBreaker.Rewards.Set("Shapeshift_Goblin_item", 1, 1, 0);
        BeastBreaker.AddStartText("Lox stomp and snort like kings. Time to knock off crowns.");
        BeastBreaker.AddStartText("You want fur for your fire and meat for your gut. Big beasts, little problem.");
        BeastBreaker.AddStartAction("I’ll bring them down with tooth and torch!", "Eh… maybe later. They're... big.");
        BeastBreaker.AddCancelText("The Lox still roam. Their hooves mock your silence.");
        BeastBreaker.AddCancelAction("Scared of a little stomp?", "I’ll wait until they’re asleep...");
        BeastBreaker.AddCompletedText("Three giants fall. Their strength now echoes in your step.");
        BeastBreaker.AddCompletedText("Flesh roasted, hides claimed. The goblin grows bold.");
        BeastBreaker.AddCompletedAction("Take this, breaker of beasts.", "Return when the ground shakes again.");

        Quest Hexfire = new Quest("Hexfire", "They called you mad. They broke your staff, burned your hut, and banished you from the tribe. Strike down ten of your former kin.", QuestType.Kill);
        Hexfire.RequiredShapeshiftForm = "GoblinShaman";
        Hexfire.Duration = 600f;
        Hexfire.Data.Set("Goblin", 10);
        Hexfire.Rewards.Set("Shapeshift_GoblinShaman_item", 1, 1, 0);
        Hexfire.AddStartText("They mocked your spells. Now they will choke on smoke.");
        Hexfire.AddStartText("You were exiled for knowledge. Now knowledge burns.");
        Hexfire.AddStartAction("Let them feel the wrath of a heretic's flame.", "Not yet. Let fear spread first.");
        Hexfire.AddCancelText("The tribe still laughs. Their huts still stand.");
        Hexfire.AddCancelAction("Spare them, after all this?", "No... but not today.");
        Hexfire.AddCompletedText("Ashes mark your passage. No chants rise to stop you now.");
        Hexfire.AddCompletedText("Ten fall to fire. The tribe remembers... too late.");
        Hexfire.AddCompletedAction("Take this, shaman of smoke and vengeance.", "Return when heresy burns bright again.");
        
        // Deathsquito attack is broken...
        
        Quest TarRevenge = new Quest("Tar Revenge", "A Lox crushed your kin. Find one—and end it.", QuestType.Kill);
        TarRevenge.RequiredShapeshiftForm = "BlobTar";
        TarRevenge.Duration = 600f;
        TarRevenge.Data.Set("Lox", 1);
        TarRevenge.Rewards.Set("Shapeshift_BlobTar_item", 1, 1, 0);

        TarRevenge.AddStartText("The Lox still walks. That must change.");
        TarRevenge.AddStartAction("Smother it. For your kin.", "Sink it into the earth.");
        TarRevenge.AddCancelText("Vengeance drips, still unclaimed.");
        TarRevenge.AddCancelAction("The Lox breathes another day.", "A stain left unwashed.");
        TarRevenge.AddCompletedText("The Lox falls. Justice seeps into the soil.");
        TarRevenge.AddCompletedAction("It is done. For the fallen.", "Their memory lingers no more.");


        Quest ThroneOfAsh = new Quest("Throne of Ash", "Fulings lost your totems, forgot your name, and let your power rot in dust. Smite twenty of your unworthy descendants.", QuestType.Kill);
        ThroneOfAsh.RequiredDefeatKey = GlobalKeys.defeated_goblinking.ToString();
        ThroneOfAsh.RequiredShapeshiftForm = "GoblinKing";
        ThroneOfAsh.Duration = 600f;
        ThroneOfAsh.Data.Set("Goblin", 20);
        ThroneOfAsh.Rewards.Set("Shapeshift_GoblinKing_item", 1, 1, 0);

        ThroneOfAsh.AddStartText("Your bones remember. Your fire still burns.");
        ThroneOfAsh.AddStartText("They let your totems fall. Let their bodies follow.");
        ThroneOfAsh.AddStartAction("I will rain fire and bone upon them.", "Let them tremble… for now.");
        ThroneOfAsh.AddCancelText("The tribe still breathes. The throne stays buried.");
        ThroneOfAsh.AddCancelAction("Spare the cowards?", "No… but the fire waits.");
        ThroneOfAsh.AddCompletedText("The earth is scorched. Your name is spoken again — in fear.");
        ThroneOfAsh.AddCompletedText("Their screams built your pyre. You rise from ash to ash.");
        ThroneOfAsh.AddCompletedAction("Take this, King of Cinders and Wrath.", "Return when memory fades again.");
    }

    private static void MistlandQuests()
    {
        Quest WhispersoftSteps = new Quest("Whispersoft Steps", "Harvest the dew-drenched Jotun Puffs.", QuestType.Harvest);
        WhispersoftSteps.RequiredBiome = Heightmap.Biome.Mistlands;
        WhispersoftSteps.RequiredShapeshiftForm = "Hare";
        WhispersoftSteps.Duration = 900f; // 15 minutes
        WhispersoftSteps.Data.Set("Pickable_Mushroom_JotunPuffs", 30); // Harvest 30 Jotun Puffs
        WhispersoftSteps.Rewards.Set("Shapeshift_Hare_item", 1, 1, 0);
        WhispersoftSteps.AddStartText("The mists are thick, and the giants’ breath clings to the earth...");
        WhispersoftSteps.AddStartText("Shapeshift into a Hare and gather what the old ones left behind—before it vanishes with the light.");
        WhispersoftSteps.AddStartAction("I will tread lightly and take what I need.", "Too perilous. I’ll wait for clearer skies.");
        WhispersoftSteps.AddCancelText("You retreat, and the morning mists swallow the puffs.");
        WhispersoftSteps.AddCancelAction("Will you let the bounty fade into myth?", "Let the fog have it… for now.");
        WhispersoftSteps.AddCompletedText("You return, pelt damp and basket full.");
        WhispersoftSteps.AddCompletedText("The forest stirs in thanks, and the giants’ breath nourishes no more.");
        WhispersoftSteps.AddCompletedAction("Accept your reward, silent forager.", "May the mist hide you again one day.");
        
        Quest PeckingOrder = new Quest("Pecking Order", "You must scratch and peck. Dandelions await—pluck twenty from the land.", QuestType.Harvest);
        PeckingOrder.RequiredBiome = Heightmap.Biome.Meadows;
        PeckingOrder.RequiredShapeshiftForm = "Hen";
        PeckingOrder.Duration = 600f;
        PeckingOrder.Data.Set("Pickable_Dandelion", 20);
        PeckingOrder.Rewards.Set("Shapeshift_Hen_item", 1, 1, 0);

        PeckingOrder.AddStartText("Time to strut and peck.");
        PeckingOrder.AddStartAction("Scratch the dirt, find the gold.", "Cluck your way to glory.");
        PeckingOrder.AddCancelText("No more pecking today.");
        PeckingOrder.AddCancelAction("Back to the coop?", "Even hens need rest.");
        PeckingOrder.AddCompletedText("Twenty picked. A proud hen indeed.");
        PeckingOrder.AddCompletedAction("Cluck! You've earned your perch.", "Feathers ruffled, task done.");

        Quest ChickenDash = new Quest("Chicken Run", "No time to think. Just run! Dash like your life depends on it—because it probably does.", QuestType.Run);
        ChickenDash.RequiredShapeshiftForm = "Chicken";
        ChickenDash.Duration = 600f;
        ChickenDash.Data.Set(1000);
        ChickenDash.Rewards.Set("Shapeshift_Chicken_item", 1, 1, 0);

        ChickenDash.AddStartText("The sky is falling! Run!");
        ChickenDash.AddStartAction("Feet flailing, feathers flying.", "Don’t stop. Something’s chasing.");
        ChickenDash.AddCancelText("You stopped. Something caught you… maybe.");
        ChickenDash.AddCancelAction("Too scared to run?", "Back to the coop you go.");
        ChickenDash.AddCompletedText("Still alive. Still clucking.");
        ChickenDash.AddCompletedAction("Well done, sprinter of the coop.", "You outran your fears.");

        Quest BroodBlooded = new Quest("Brood-Blooded", "Born crawling, wings still dreams. Your path begins in blood. Slay one of your own, and earn the right to grow.", QuestType.Kill);
        BroodBlooded.RequiredBiome = Heightmap.Biome.Mistlands;
        BroodBlooded.RequiredShapeshiftForm = "SeekerBrood";
        BroodBlooded.Duration = 600f;
        BroodBlooded.Data.Set("Seeker", 1);
        BroodBlooded.Rewards.Set("Shapeshift_SeekerBrood_item", 1, 1, 0);

        BroodBlooded.AddStartText("Even hatchlings are tested. The swarm shows no mercy.");
        BroodBlooded.AddStartText("To rise, you must feed the dirt with your kin.");
        BroodBlooded.AddStartAction("Tiny fangs. Big purpose.", "The air still frightens me.");
        BroodBlooded.AddCancelText("You cower. The swarm forgets the weak.");
        BroodBlooded.AddCancelAction("Refuse the rite of blood?", "Another brood will take your place.");
        BroodBlooded.AddCompletedText("One seeker falls. One brood survives. That is the way.");
        BroodBlooded.AddCompletedText("You’ve tasted power. The swarm watches.");
        BroodBlooded.AddCompletedAction("Take this, fledgling of fang and fate.", "Return when your wings ache to break free.");

        Quest SporesOfTheDeep = new Quest("Spores of the Deep", "Magecap glows faint and blue. Find what your kin cannot — the mushrooms that fuel their fragile magic.", QuestType.Harvest);
        SporesOfTheDeep.RequiredBiome = Heightmap.Biome.Mistlands;
        SporesOfTheDeep.RequiredShapeshiftForm = "DvergerMageSupport";
        SporesOfTheDeep.Duration = 600f;
        SporesOfTheDeep.Data.Set("Pickable_Mushroom_Magecap", 20);
        SporesOfTheDeep.Rewards.Set("Shapeshift_DvergerMageSupport_item", 1, 1, 0);

        SporesOfTheDeep.AddStartText("Look sharp, little one. The mist conceals, but Magecap still whispers from under root and rock.");
        SporesOfTheDeep.AddStartText("Your kin grow weak. Their spells falter. Fetch the blue caps before the mist takes more than magic.");
        SporesOfTheDeep.AddStartAction("Yes, fly into the haze. Find the glow, feed the flame.", "Leave them to starve? A cold choice…");
        SporesOfTheDeep.AddCancelText("No caps. No spells. The forge grows cold, and the clan colder still.");
        SporesOfTheDeep.AddCancelAction("Will you turn your wings from duty?", "Perhaps another gust will guide you.");
        SporesOfTheDeep.AddCompletedText("Good. The satchel glows like a lantern in the fog. They will chant your name in steam and stone.");
        SporesOfTheDeep.AddCompletedText("You walk through the mist, and from it, nourishment returns. The clan breathes easier.");
        SporesOfTheDeep.AddCompletedAction("Take your due, clever fetcher. The mists will call again.", "Until the next bloom, winged one.");
        
        Quest PuffOrNothing = new Quest("Puff or Nothing", "Hunt the Jotun Puffs — bold, blistered, and born of flame. Ten will sate you. Nothing else will.", QuestType.Harvest);
        PuffOrNothing.RequiredBiome = Heightmap.Biome.Mistlands;
        PuffOrNothing.RequiredShapeshiftForm = "DvergerMageFire";
        PuffOrNothing.Duration = 600f;
        PuffOrNothing.Data.Set("Pickable_Mushroom_JotunPuffs", 10);
        PuffOrNothing.Rewards.Set("Shapeshift_DvergerMageFire_item", 1, 1, 0);

        PuffOrNothing.AddStartText("Magecaps? Pfft. Fit for alchemists and stew-brewers.");
        PuffOrNothing.AddStartText("Only fools eat damp fungus. You seek the puff — hot, rare, and worthy of your flame.");
        PuffOrNothing.AddStartAction("Go on then, firetongue. Prove your taste.", "Back off? Might as well drink swamp water.");
        PuffOrNothing.AddCancelText("No puffs. Just shame. And a growling gut.");
        PuffOrNothing.AddCancelAction("Settle for Magecap slop?", "Not while you still burn.");
        PuffOrNothing.AddCompletedText("You found them — smoky, rich, defiant. Just like you.");
        PuffOrNothing.AddCompletedText("The fire’s fed. Your pride puffs higher than the mist.");
        PuffOrNothing.AddCompletedAction("Feast, flareborn. Even gods envy your palate.", "Fly back when hunger smolders again.");

        Quest FrozenWisdom = new Quest("Frozen Wisdom", "Craft ten bowls of Yggdrasil Porridge — not to feast, but to understand. Knowledge is chilled, and so is the Dverger way.", QuestType.Collect);
        FrozenWisdom.RequiredShapeshiftForm = "DvergerMageIce";
        FrozenWisdom.Duration = 600f;
        FrozenWisdom.Data.Set("YggdrasilPorridge", 10);
        FrozenWisdom.Rewards.Set("Shapeshift_DvergerMageIce_item", 1, 1, 0);

        FrozenWisdom.AddStartText("They do not burn. They do not crave. They endure.");
        FrozenWisdom.AddStartText("In frost and silence, the Dverger simmer their secrets in porridge.");
        FrozenWisdom.AddStartAction("Don the cold, stir the root, learn what they swallow.", "Or fly off and stay warm — like the weak.");
        FrozenWisdom.AddCancelText("No broth. No lesson. Just a bitter tongue.");
        FrozenWisdom.AddCancelAction("Turn from the chill path?", "Warmth is easy. Wisdom isn't.");
        FrozenWisdom.AddCompletedText("Ten bowls steeped in frost and patience. The cold has spoken.");
        FrozenWisdom.AddCompletedText("Now you know — the Dverger don’t eat to live. They eat to remember.");
        FrozenWisdom.AddCompletedAction("Take this, one who sipped the frost’s memory.", "Return when you hunger for understanding again.");

        Quest WingsOfTheUnknown = new Quest("Wings of the Unknown", "Glide through the mist for a thousand meters. Know their sky. Know their hunger.", QuestType.Fly);
        WingsOfTheUnknown.RequiredBiome = Heightmap.Biome.Mistlands;
        WingsOfTheUnknown.RequiredShapeshiftForm = "Seeker";
        WingsOfTheUnknown.Duration = 600f;
        WingsOfTheUnknown.Data.Set(5000);
        WingsOfTheUnknown.Rewards.Set("Shapeshift_Seeker_item", 1, 1, 0);
        WingsOfTheUnknown.AddStartText("They don’t soar. They scuttle the sky.");
        WingsOfTheUnknown.AddStartText("Feel the hum of wings built for nightmares, not nests.");
        WingsOfTheUnknown.AddStartAction("Crawl into the air and let the mist swallow you.", "Or stay grounded like a stump-bound toad.");
        WingsOfTheUnknown.AddCancelText("The air slips through your limbs. You fall. A shame.");
        WingsOfTheUnknown.AddCancelAction("Done with buzzing the clouds?", "Too strange for your feathers?");
        WingsOfTheUnknown.AddCompletedText("You’ve flown not as a raven, but as a horror. And yet... you lived.");
        WingsOfTheUnknown.AddCompletedText("The mists part for those who earn their wings — even crooked ones.");
        WingsOfTheUnknown.AddCompletedAction("Take this, twisted flier of the deep haze.", "Come back when the itch to buzz the veil returns.");
        
        Quest BellyOfTheSky = new Quest("Belly of the Sky", "Spawn rain terror upon the Dverger interlopers.", QuestType.Kill);
        BellyOfTheSky.RequiredBiome = Heightmap.Biome.Mistlands;
        BellyOfTheSky.RequiredShapeshiftForm = "Gjall";
        BellyOfTheSky.Duration = 1200f; // 20 minutes
        BellyOfTheSky.Data.Set("Dverger", 10); // Eliminate 10 Dverger
        BellyOfTheSky.Rewards.Set("Shapeshift_Gjall_item", 1, 1, 0);
        BellyOfTheSky.AddStartText("The sky groans, and war brews beneath your bloated form...");
        BellyOfTheSky.AddStartText("As Gjall, you drift unseen—until your young descend in fury. The Dverger must fall.");
        BellyOfTheSky.AddStartAction("Let them taste the dread of the clouds!", "Even I fear what I become...");
        BellyOfTheSky.AddCancelText("You recoil into the fog, your brood restless and unsated.");
        BellyOfTheSky.AddCancelAction("Will you deny your offspring their feast?", "Not this time… let them live a little longer.");
        BellyOfTheSky.AddCompletedText("The Dverger outposts smolder. Their weapons silenced.");
        BellyOfTheSky.AddCompletedText("Your spawn fed well. The sky is quiet—for now.");
        BellyOfTheSky.AddCompletedAction("Claim your spoils, skyborne harbinger.", "Return when the clouds darken once more.");
        
        Quest BloodLessons = new Quest("Blood Lessons", "Shrink down. Latch on. Linger. Only then will you taste what drives them — and what they fear.", QuestType.Attach);
        BloodLessons.RequiredShapeshiftForm = "Tick";
        BloodLessons.Duration = 600f;
        BloodLessons.Data.Set(10);
        BloodLessons.Rewards.Set("Shapeshift_Tick_item", 1, 1, 0);
        BloodLessons.AddStartText("A Tick drinks not just blood — it sips memory, fear, and warmth.");
        BloodLessons.AddStartText("Their world is small, but their hunger is vast.");
        BloodLessons.AddStartAction("Sink in. Hold tight. Become need itself.", "Not ready to feel that close?");
        BloodLessons.AddCancelText("You unlatched too soon. Hunger teaches patience.");
        BloodLessons.AddCancelAction("Creeped out already?", "Even ravens gag sometimes.");
        BloodLessons.AddCompletedText("The bond held. The blood flowed. You know more now, though you wish you didn’t.");
        BloodLessons.AddCompletedText("Ten seconds of stillness. Ten centuries of instinct, borrowed.");
        BloodLessons.AddCompletedAction("Take this, little horror of the haze.", "Return if you thirst for more unsettling truths.");

        Quest ProofByCarapace = new Quest("Proof by Carapace", "You walk, crush, and conquer. Slay ten of your own and learn the laws of the hive.", QuestType.Kill);
        ProofByCarapace.RequiredBiome = Heightmap.Biome.Mistlands;
        ProofByCarapace.RequiredShapeshiftForm = "SeekerBrute";
        ProofByCarapace.Duration = 600f;
        ProofByCarapace.Data.Set("Seeker", 10);
        ProofByCarapace.Rewards.Set("Shapeshift_SeekerBrute_item", 1, 1, 0);

        ProofByCarapace.AddStartText("The hive does not mourn. It tests.");
        ProofByCarapace.AddStartText("To be a soldier is to rise through broken wings.");
        ProofByCarapace.AddStartAction("I will crush and be counted.", "My mandibles are not yet ready.");
        ProofByCarapace.AddCancelText("You hesitated. The hive would not.");
        ProofByCarapace.AddCancelAction("Turn away from the swarm?", "Even the strong recoil at first.");
        ProofByCarapace.AddCompletedText("Their carapaces crack beneath you. You are one of them now.");
        ProofByCarapace.AddCompletedText("No morals. No mercy. Just the lesson of blood and chitin.");
        ProofByCarapace.AddCompletedAction("Take this, soldier of the swarm.", "Return if your hunger for power grows.");

        Quest UnburdenedBroodmother = new Quest("Unburdened Broodmother", "Bound by tunnels, worshiped in shadow. But not today. As a Seeker Queen, shed your burden of birthing, and walk the waking world.", QuestType.Travel);
        UnburdenedBroodmother.RequiredDefeatKey = "defeated_queen";
        UnburdenedBroodmother.RequiredShapeshiftForm = "SeekerQueen";
        UnburdenedBroodmother.Duration = 600f;
        UnburdenedBroodmother.Data.Set(3000); // Travel 3000 units
        UnburdenedBroodmother.Rewards.Set("Shapeshift_SeekerQueen_item", 1, 1, 0);

        UnburdenedBroodmother.AddStartText("She who crawls, commands. But even queens may wander.");
        UnburdenedBroodmother.AddStartText("The stone groans beneath you. Let it feel your weight beyond the nest.");
        UnburdenedBroodmother.AddStartAction("I am not just womb. I am will.", "The lair is louder than the wind.");
        UnburdenedBroodmother.AddCancelText("The earth pulls you home, back to the dark.");
        UnburdenedBroodmother.AddCancelAction("Let the hive reclaim you?", "No… the path is mine.");
        UnburdenedBroodmother.AddCompletedText("Each step echoed defiance. You are queen, still — but freer.");
        UnburdenedBroodmother.AddCompletedText("No brood. No burden. Just ground and sky and you between.");
        UnburdenedBroodmother.AddCompletedAction("Take this, walker of roads unwritten.", "Return when the stone no longer shakes beneath you.");
    }
    
    private static void AshlandQuests()
    {
        Quest EmberVine = new Quest("Ember Vine", "Rise from bone and gather what little life remains.", QuestType.Harvest);
        EmberVine.RequiredBiome = Heightmap.Biome.AshLands;
        EmberVine.RequiredShapeshiftForm = "Charred_Twitcher";
        EmberVine.Duration = 600f;
        EmberVine.Data.Set("VineAsh", 20);
        EmberVine.Rewards.Set("Shapeshift_Charred_Twitcher_item", 1, 1, 0);
        EmberVine.AddStartText("Bones don’t eat, but still they gather. Still they move.");
        EmberVine.AddStartText("Ash chokes the sky. Vines cling to ruin. Go now, twitcher — the harvest burns.");
        EmberVine.AddStartAction("No lungs, no thirst. Just duty.", "Even the dead tire in this place.");
        EmberVine.AddCancelText("You scatter like ash on the wind. No vine, no purpose.");
        EmberVine.AddCancelAction("Leave the Ashlands to smolder alone?", "No... not yet.");
        EmberVine.AddCompletedText("Your hands blacken, but they do not fail. The vines give what they can.");
        EmberVine.AddCompletedText("Berries plucked from flame-fed vines. Enough for today.");
        EmberVine.AddCompletedAction("Take this, ember-walker. The Ashlands remember all who serve.", "Return when the roots bleed smoke once more.");
        
        Quest AshAndAim = new Quest("Ash and Aim", "As a Charred Archer, take up your smoldering bow and strike down three Voltures, the thieving sky-scavengers who steal what little remains.", QuestType.Kill);
        AshAndAim.RequiredBiome = Heightmap.Biome.AshLands;
        AshAndAim.RequiredShapeshiftForm = "Charred_Archer";
        AshAndAim.Duration = 600f;
        AshAndAim.Data.Set("Volture", 3);
        AshAndAim.Rewards.Set("Shapeshift_Charred_Archer_item", 1, 1, 0);
        AshAndAim.AddStartText("No law in the Ash. Only fire and foes.");
        AshAndAim.AddStartText("A skeleton with a bow, a world without mercy. Prove you're more than cinders.");
        AshAndAim.AddStartAction("The Voltures circle. Loose your fury.", "The sky can wait. The flame still burns.");
        AshAndAim.AddCancelText("Ash settles. Bows unstrung.");
        AshAndAim.AddCancelAction("Let the Voltures steal again?", "They won't escape next time.");
        AshAndAim.AddCompletedText("Three downed thieves. The Ashlands whisper your name in soot.");
        AshAndAim.AddCompletedText("Feathers fall. Ash rises. You stand unbroken.");
        AshAndAim.AddCompletedAction("Take this, sharpsoul of the slagfields.", "Return when their wings blot the sky again.");

        Quest TwitcherBane = new Quest("Twitcher Bane", "Even among the dead, rivalries fester.", QuestType.Kill);
        TwitcherBane.RequiredBiome = Heightmap.Biome.AshLands;
        TwitcherBane.RequiredShapeshiftForm = "Charred_Melee";
        TwitcherBane.Duration = 600f;
        TwitcherBane.Data.Set("Charred_Twitcher", 5);
        TwitcherBane.Rewards.Set("Shapeshift_Charred_Melee_item", 1, 1, 0);
        TwitcherBane.AddStartText("Charred bones war among themselves. The Twitchers are the worst — noisy, wild, undisciplined.");
        TwitcherBane.AddStartText("Even the swordsmen hate them. I’ve seen it. Go, take their dust and learn the Ashlands’ bitter hierarchy.");
        TwitcherBane.AddStartAction("The Ashlands crackle... you’ll fit right in.", "The Twitchers won't stop taunting. Will you?");
        TwitcherBane.AddCancelText("So, you choose peace among the restless dead?");
        TwitcherBane.AddCancelAction("Still your blade?", "Just wait — they'll throw again.");
        TwitcherBane.AddCompletedText("Good. Silence returns — if only for a moment.");
        TwitcherBane.AddCompletedText("The Ashlands are quieter now. Even the wind seems pleased.");
        TwitcherBane.AddCompletedAction("Take this, bone-breaker. You’ve earned it.", "When the twitching starts anew, you’ll know what to do.");
        
        Quest FangsAndFruit = new Quest("Fangs and Fruit", "You may see only fangs and scales—but the Asksvin knows hunger, too. Take its form and scour the Ashlands for Smoke Puffs, the smoldering fruit it craves.", QuestType.Harvest);
        FangsAndFruit.RequiredBiome = Heightmap.Biome.AshLands;
        FangsAndFruit.RequiredShapeshiftForm = "Asksvin";
        FangsAndFruit.Duration = 600f;
        FangsAndFruit.Data.Set("Pickable_SmokePuff", 20);
        FangsAndFruit.Rewards.Set("Shapeshift_Asksvin_item", 1, 1, 0);
        FangsAndFruit.AddStartText("Not all beasts hunger for meat. The Ashlands offer sweeter things.");
        FangsAndFruit.AddStartText("Smoke Puffs grow where the fire passed. Find them, and learn the Asksvin's path.");
        FangsAndFruit.AddStartAction("Sniff the scorched earth. Let instinct guide you.", "Or stay hungry and blind.");
        FangsAndFruit.AddCancelText("No fruit, no fire. Just ash on your tongue.");
        FangsAndFruit.AddCancelAction("Give up the hunt?", "The Ashlands are patient. But not kind.");
        FangsAndFruit.AddCompletedText("Juice on your snout. Ash on your paws. The hunt was good.");
        FangsAndFruit.AddCompletedText("You walk the land like the Asksvin. Sharp, silent, satisfied.");
        FangsAndFruit.AddCompletedAction("Take this, fruit-hunter of the flame-choked vale.", "Come back when your stomach speaks again.");

        Quest OfScaleAndBond = new Quest("Of Scale and Bond", "Smoke Puffs fed your belly. Now feed your soul.", QuestType.Tame);
        FangsAndFruit.RequiredBiome = Heightmap.Biome.AshLands;
        OfScaleAndBond.RequiredShapeshiftForm = "Asksvin";
        OfScaleAndBond.Duration = 600f;
        OfScaleAndBond.Data.Set("Asksvin", 1);
        OfScaleAndBond.Rewards.Set("Shapeshift_Asksvin_item", 1, 1, 0);
        OfScaleAndBond.AddStartText("Even beasts seek companionship. The wild does not mean alone.");
        OfScaleAndBond.AddStartText("You’ve walked the Ashlands as one. Now walk it as two.");
        OfScaleAndBond.AddStartAction("Whistle low. Show no fear. Respect earns trust.", "Or shall you return alone, tail down?");
        OfScaleAndBond.AddCancelText("Your scent fades from the wind. No one followed.");
        OfScaleAndBond.AddCancelAction("Turn from the bond?", "Then you learn only half the lesson.");
        OfScaleAndBond.AddCompletedText("Two shadows move where once was one. The wild has answered.");
        OfScaleAndBond.AddCompletedText("Trust earned in fire lasts beyond flame. The Asksvin walks with you.");
        OfScaleAndBond.AddCompletedAction("Take this, kin-tamer of the coals.", "Return when the wild calls again.");

        Quest FiresOfTheFuture = new Quest("Fires of the Future", "You have fed. You have bonded. Now, carry the lineage forward.", QuestType.Procreate);
        FiresOfTheFuture.RequiredBiome = Heightmap.Biome.AshLands;
        FiresOfTheFuture.RequiredShapeshiftForm = "Asksvin";
        FiresOfTheFuture.Duration = 600f;
        FiresOfTheFuture.Data.Set("Asksvin", 1);
        FiresOfTheFuture.Rewards.Set("Shapeshift_Asksvin_item", 1, 1, 0);
        FiresOfTheFuture.Rewards.Set("ArmorValkyrieChest_RS");
        FiresOfTheFuture.AddStartText("Even in ash, life finds a way. The time has come.");
        FiresOfTheFuture.AddStartText("To know a creature, know how it loves, how it continues.");
        FiresOfTheFuture.AddStartAction("Let instinct guide you. Creation is its own magic.", "Or has the fire in you turned to coal?");
        FiresOfTheFuture.AddCancelText("No egg warms beneath you. The line ends, for now.");
        FiresOfTheFuture.AddCancelAction("Turn from the cycle?", "Then you remain but a mimic.");
        FiresOfTheFuture.AddCompletedText("A spark born from soot. Life coils, breathes, begins.");
        FiresOfTheFuture.AddCompletedText("The Ashlands now know your legacy. The next scales rise.");
        FiresOfTheFuture.AddCompletedAction("Take this, flame-bearer. You've lived a full Asksvin life.", "Return when more paths await.");
        
        Quest EyesOverAsh = new Quest("Eyes Over Ash", "Witness the scorched remains, the scraps of survival, and learn why the Volture feeds not by choice—but by necessity.", QuestType.Fly);
        EyesOverAsh.RequiredBiome = Heightmap.Biome.AshLands;
        EyesOverAsh.RequiredShapeshiftForm = "Volture";
        EyesOverAsh.Duration = 600f;
        EyesOverAsh.Data.Set(3000);
        EyesOverAsh.Rewards.Set("Shapeshift_Volture_item", 1, 1, 0);
        EyesOverAsh.AddStartText("The wind carries you. But it’s the hunger that teaches.");
        EyesOverAsh.AddStartText("To scavenge is not shameful—it is survival through sight.");
        EyesOverAsh.AddStartAction("Take wing. Learn what it means to live on what others leave behind.", "Or stay grounded, blind to truth.");
        EyesOverAsh.AddCancelText("Your wings falter. The sky remains unexplored.");
        EyesOverAsh.AddCancelAction("Too proud to feed from remnants?", "Then you know not the Volture.");
        EyesOverAsh.AddCompletedText("Ash below, wind above. You see now how the Volture survives.");
        EyesOverAsh.AddCompletedText("To scavenge is to endure. You understand the elegance in desperation.");
        EyesOverAsh.AddCompletedAction("Fly no more. You've earned your place among the wind-walkers.", "Return when the sky calls again.");
        
        Quest AshesAndArrogance = new Quest("Ashes and Arrogance", "As a Charred Mage, you are scorned by the Archers, who boast of precision and distance. Prove them wrong.", QuestType.Kill);
        AshesAndArrogance.RequiredBiome = Heightmap.Biome.AshLands;
        AshesAndArrogance.RequiredShapeshiftForm = "Charred_Mage";
        AshesAndArrogance.Duration = 600f;
        AshesAndArrogance.Data.Set("Charred_Archer", 5);
        AshesAndArrogance.Rewards.Set("CapeValkyrie_RS");
        AshesAndArrogance.Rewards.Set("Shapeshift_Charred_Mage_item", 1, 1, 0);
        AshesAndArrogance.AddStartText("Charred Archers think themselves artists. Let them feel the fire of true power.");
        AshesAndArrogance.AddStartText("They sneer at spellcasters. I say you melt their smug bones.");
        AshesAndArrogance.AddStartAction("Burn from behind the brimstone veil.", "Or leave them to their arrogance.");
        AshesAndArrogance.AddCancelText("Magic fades. Or was it courage?");
        AshesAndArrogance.AddCancelAction("No flames today?", "I thought you'd relish the fire.");
        AshesAndArrogance.AddCompletedText("Their bows are silent. Your will, loud.");
        AshesAndArrogance.AddCompletedText("No more arrows from shadows. Just the glow of your wrath.");
        AshesAndArrogance.AddCompletedAction("Well done, spark of vengeance.", "Return when more bones need humbling.");
        
        Quest MawAndMotion = new Quest("Maw and Motion", "From sinew and bone it crawls, from shadow it hungers...", QuestType.Kill);
        MawAndMotion.RequiredBiome = Heightmap.Biome.AshLands;
        MawAndMotion.RequiredShapeshiftForm = "Morgen";
        MawAndMotion.Duration = 600f;
        MawAndMotion.Data.Set("Asksvin", 10);
        MawAndMotion.Rewards.Set("Shapeshift_Morgen_item", 1, 1, 0);
        MawAndMotion.Rewards.Set("Shapeshift_BlobLava_item");
        MawAndMotion.Rewards.Set("ArmorValkyrieLegs_RS");
        MawAndMotion.AddStartText("Deep below, something writhes. Become it.");
        MawAndMotion.AddStartText("No words in its mind, no light in its world. Only hunger, only bone.");
        MawAndMotion.AddStartAction("It rolls, it grinds, it feeds.", "Even silence fears the Morgen.");
        MawAndMotion.AddCancelText("The cave sighs. The hunt forgotten.");
        MawAndMotion.AddCancelAction("Leave the bones to rot alone?", "Not today. Not yet.");
        MawAndMotion.AddCompletedText("The Asksvin fall. The coil of flesh twists on.");
        MawAndMotion.AddCompletedText("The prey is gone. The nightmare stirs elsewhere.");
        MawAndMotion.AddCompletedAction("Take this, cave-stalker. The dark remembers your shape.", "Return when the sinews tighten once more.");
        
        Quest SerpentSupremacy = new Quest("Serpent Supremacy", "Escape the ash and seize the sea. Prove your dominance over the serpents.", QuestType.Kill);
        SerpentSupremacy.RequiredShapeshiftForm = "BonemawSerpent";
        SerpentSupremacy.Duration = 600f;
        SerpentSupremacy.Data.Set("Serpent");
        SerpentSupremacy.Rewards.Set("Shapeshift_BonemawSerpent_item", 1, 1, 0);

        SerpentSupremacy.AddStartText("The ashlands burn, but the sea calls.");
        SerpentSupremacy.AddStartAction("Leave the flame for the flood.", "Take the sea. Show no mercy.");
        SerpentSupremacy.AddCancelText("The ocean eludes you. Ash clings to your scales.");
        SerpentSupremacy.AddCancelAction("Retreating to ruin?", "Then rot in lava.");
        SerpentSupremacy.AddCompletedText("The sea bows to your coil.");
        SerpentSupremacy.AddCompletedAction("You are no longer of ash, but of tide and terror.", "The ocean is yours now.");

        Quest WingsOfAsh = new Quest("Wings of Ash", "Prove you still fight for glory—defeat a Morgen.", QuestType.Kill);
        WingsOfAsh.RequiredShapeshiftForm = "FallenValkyrie";
        WingsOfAsh.Duration = 600f;
        WingsOfAsh.Data.Set("Morgen", 1);
        WingsOfAsh.Rewards.Set("Shapeshift_FallenValkyrie_item", 1, 1, 0);
        WingsOfAsh.Rewards.Set("HelmetValkyrie_RS");

        WingsOfAsh.AddStartText("Odin watches no more. But you still rise.");
        WingsOfAsh.AddStartAction("Burned wings, but your blade remembers.", "Face the beast. Find purpose in the ash.");
        WingsOfAsh.AddCancelText("Your sorrow weighs heavier than your sword.");
        WingsOfAsh.AddCancelAction("Another day lost in exile.", "Even fallen, rest must come.");
        WingsOfAsh.AddCompletedText("The Morgen falls. A flicker of grace returns.");
        WingsOfAsh.AddCompletedAction("Perhaps Odin felt that blow.", "Even in ruin, you still serve.");
    }

    public static void Setup()
    {
        MeadowQuests();
        BlackForestQuests();
        SwampQuests();
        MountainQuests();
        OceanQuests();
        PlainsQuests();
        MistlandQuests();
        AshlandQuests();
    }
}