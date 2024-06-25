using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Scenarios;
using BTD_Mod_Helper;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Models;
using System.Collections.Generic;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Simulation.Track.RoundManagers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace ImpoppablePlus;

public class MasteryRounds : ModRoundSet
{
    public override string BaseRoundSet => RoundSetType.Default;
    public override int DefinedRounds => BaseRounds.Count;
    public override string DisplayName => "Mastery Mode";
    public override string Icon => "Mastery";

    private static readonly Dictionary<string, string> promotionMap = new Dictionary<string, string>()
     {
        { "Red", "Blue" },
        { "RedCamo", "BlueCamo" },
        { "RedRegrow", "BlueRegrow" },
        { "RedRegrowCamo", "BlueRegrowCamo" },

        { "Blue", "Green" },
        { "BlueCamo", "GreenCamo" },
        { "BlueRegrow", "GreenRegrow" },
        { "BlueRegrowCamo", "GreenRegrowCamo" },

        { "Green", "Yellow" },
        { "GreenCamo", "YellowCamo" },
        { "GreenRegrow", "YellowRegrow" },
        { "GreenRegrowCamo", "YellowRegrowCamo" },

        { "Yellow", "Pink" },
        { "YellowCamo", "PinkCamo" },
        { "YellowRegrow", "PinkRegrow" },
        { "YellowRegrowCamo", "PinkRegrowCamo" },

        { "Pink", "Black" },
        { "PinkCamo", "BlackCamo" },
        { "PinkRegrow", "BlackRegrow" },
        { "PinkRegrowCamo", "BlackRegrowCamo" },

        { "Black", "Zebra" },
        { "BlackCamo", "ZebraCamo" },
        { "BlackRegrow", "ZebraRegrow" },
        { "BlackRegrowCamo", "ZebraRegrowCamo" },

        { "White", "Purple" },
        { "WhiteCamo", "PurpleCamo" },
        { "WhiteRegrow", "PurpleRegrow" },
        { "WhiteRegrowCamo", "PurpleRegrowCamo" },

        { "Purple", "LeadFortified" },
        { "PurpleCamo", "LeadFortifiedCamo" },
        { "PurpleRegrow", "LeadRegrowFortified" },
        { "PurpleRegrowCamo", "LeadRegrowFortifiedCamo" },

        { "Lead", "Rainbow" },
        { "LeadCamo", "RainbowCamo" },
        { "LeadRegrow", "RainbowRegrow" },
        { "LeadRegrowCamo", "RainbowRegrowCamo" },
        { "LeadFortified", "RainbowRegrowCamo" },
        { "LeadRegrowFortified", "RainbowRegrowCamo" },
        { "LeadFortifiedCamo", "RainbowRegrowCamo" },
        { "LeadRegrowFortifiedCamo", "RainbowRegrowCamo" },

        { "Zebra", "Rainbow" },
        { "ZebraCamo", "RainbowCamo" },
        { "ZebraRegrow", "RainbowRegrow" },
        { "ZebraRegrowCamo", "RainbowRegrowCamo" },

        { "Rainbow", "Ceramic" },
        { "RainbowCamo", "CeramicCamo" },
        { "RainbowRegrow", "CeramicRegrow" },
        { "RainbowRegrowCamo", "CeramicRegrowCamo" },

        { "Ceramic", "Moab" },
        { "CeramicCamo", "Moab" },
        { "CeramicRegrow", "Moab" },
        { "CeramicRegrowCamo", "Moab" },
        { "CeramicFortified", "MoabFortified" },
        { "CeramicFortifiedCamo", "MoabFortified" },
        { "CeramicRegrowFortified", "MoabFortified" },
        { "CeramicRegrowFortifiedCamo", "MoabFortified" },

        { "Moab", "Bfb" },
        { "MoabFortified", "BfbFortified" },

        { "Bfb", "DdtCamo" },
        { "BfbFortified", "DdtFortifiedCamo" },

        { "DdtCamo", "Zomg" },
        { "DdtFortifiedCamo", "ZomgFortified" },

        { "Zomg", "Bad" },
        { "ZomgFortified", "BadFortified" },

        { "Bad", "BadFortified" },
        { "BadFortified", "BadFortified" }
    };

    public static string PromoteBloon(string bloon)
    {
        //if (bloon.Contains("Pink") || bloon.Contains("Lead")) return bloon;
        string temp = bloon;
        promotionMap.TryGetValue(bloon, out temp);
        return temp;
    }

    private readonly Dictionary<int, string> hints = new()
    {
        {1, "Mastery mode... red bloons become blue bloons."},
        {2, "Blue bloons become green bloons."},
        {5, "Green bloons become yellow bloons."},
        {37, "First 2 MOAB-Class Bloons next round."},
        {39, "MOABs become BFBs..."},
        {45, "Fortified MOABs coming up next."},
        {54, "BTD6 is awesome. Life is awesome too. Don't forget to have a break sometimes and do something else. Then play more BTD6!"},
        {59, "What is a DDT Bloon you may ask? Like a MOAB crossed with a Pink, Camo, Black and Lead Bloon. In all the bad ways."},
        {62, "Next level will be hard. Really hard."},
        {79, "No ZOMGs so far... not too BAD was it?"},
        {84, "It's about to get worse though..."},
        {96, "Fortified BADs - as bad as it gets."},
        {99, "Do you remember the final round of impoppable mode from BTD5?"},
        {100, "Congratulations on beating round 100! Enjoy your reward!"},
    };

    public override void ModifyRoundModels(RoundModel roundModel, int round)
    {
        if (round == 99){ // Round 100 patch (5 fortified BADs)
            roundModel.groups[0].count = 5;
            roundModel.groups[0].end = 1500;
        }
        
        for (int k = 0; k < roundModel.groups.Length; k++)
        {
            BloonGroupModel bloonGroup = roundModel.groups[k];
            bloonGroup.bloon = PromoteBloon(bloonGroup.bloon);
        }
    }

    public static bool IsCurrentRoundSet()
    {
        if (InGame.instance != null)
        {
            return InGame.instance.GetGameModel().roundSet.name == ModContent.RoundSetId<MasteryRounds>();
        }
        else
        {
            return false;
        }
    }

    public override string GetHint(int round)
    {
        return hints.GetValueOrDefault(round);
    }
}

public class Mastery : ModGameMode
{
    public override string Difficulty => "Impoppable";

    public override string BaseGameMode => GameModeType.Impoppable;

    public override string DisplayName => "Mastery Mode";

    public override string Icon => "Mastery";

    public override void ModifyBaseGameModeModel(ModModel gameModeModel)
    {
        gameModeModel.UseRoundSet<MasteryRounds>();
        #if DEBUG
        ModHelper.Msg<ImpoppablePlus>($"Mastery Mode: Reducing cash per pop {SimulationPatches.roundsEarly} rounds early.");
        #endif
    }
}

internal static class RoundEmissionsHooks
{
    private static Il2CppReferenceArray<BloonEmissionModel> PromoteEmissions(Il2CppReferenceArray<BloonEmissionModel> emissions)
    {
        if (MasteryRounds.IsCurrentRoundSet())
        {
            return emissions.Select(
                emissionModel => {
                    BloonEmissionModel promotedEmissionModel = emissionModel.Duplicate();
                    promotedEmissionModel.bloon = MasteryRounds.PromoteBloon(emissionModel.bloon);
                    return promotedEmissionModel;
                }
            ).ToIl2CppReferenceArray();
        }
        else
        {
            return emissions;
        }
    }

    [HarmonyPatch(typeof(FreeplayRoundManager), nameof(FreeplayRoundManager.GetRoundEmissions))]
    internal static class FreeplayRoundManager_GetRoundEmissionsHook
    {
        [HarmonyPostfix]
        private static void Postfix(FreeplayRoundManager __instance, int roundArrayIndex, ref Il2CppReferenceArray<BloonEmissionModel> __result)
        {
            // Bypass the RBE limit by promoting bloons after selecting the bloon groups.
            __result = PromoteEmissions(__result);
        }
    }
}

internal static class SimulationPatches
{
    private static int currentRound;
    public const int roundsEarly = 20;

    private static List<(int start, int end, double multiplier)> multipliers = new List<(int start, int end, double multiplier)>
    {
        (1, 50, 1.0),
        (51, 60, 0.5),
        (61, 85, 0.2),
        (86, 100, 0.1),
        (101, 120, 0.05),
        (121, int.MaxValue, 0.02)
    };

    public static List<(int start, int end, double multiplier)> getMasteryMultipliers(int roundsEarly)
    {
        // Cash from popping bloons is halved.
        return new List<(int start, int end, double multiplier)>{
            (1, 50-roundsEarly, 1.0),
            (51-roundsEarly, 60-roundsEarly, 0.5),
            (61-roundsEarly, 85-roundsEarly, 0.2),
            (86-roundsEarly, 100-roundsEarly, 0.1),
            (101-roundsEarly, 120, 0.05),
            (121, int.MaxValue, 0.02)
        };
    }

    [HarmonyPatch(typeof(Simulation), "RoundStart")]
    internal static class RoundStartHook
    {
        [HarmonyPrefix]
        private static void Prefix(int spawnedRound)
        {
            currentRound = spawnedRound + 1;
        }
    }

    [HarmonyPatch(typeof(Simulation), "AddCash")]
    internal static class AddCash_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(ref double c, ref Simulation.CashSource source)
        {
            if ((source == Simulation.CashSource.Normal) && MasteryRounds.IsCurrentRoundSet())
            {
                c *= getMasteryMultipliers(roundsEarly).First(range => (currentRound >= range.start) && (currentRound <= range.end)).multiplier /
                     multipliers.First(range => (currentRound >= range.start) && (currentRound <= range.end)).multiplier;
            }
            return true;
        }
    }
}