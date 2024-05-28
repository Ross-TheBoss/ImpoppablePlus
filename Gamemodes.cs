using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Scenarios;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Difficulty;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using BTD_Mod_Helper;
using Il2CppAssets.Scripts.Simulation.SimulationBehaviors;

namespace ImpoppablePlus;

public class SupportOnly : ModGameMode
{
    public override string Difficulty => "Impoppable";

    public override string BaseGameMode => GameModeType.Impoppable;

    public override string DisplayName => "Support Monkeys Only";

    public override string Icon => "SupportOnly";

    public override void ModifyBaseGameModeModel(ModModel gameModeModel)
    {
        gameModeModel.LockTowerSet(TowerSet.Primary);
        gameModeModel.LockTowerSet(TowerSet.Military);
        gameModeModel.LockTowerSet(TowerSet.Magic);
    }
}

public class DoubleSpeed : ModGameMode 
{
    public override string Difficulty => "Impoppable";

    public override string BaseGameMode => GameModeType.Impoppable;

    public override string DisplayName => "Double Speed";

    public override string Icon => VanillaSprites.FasterBloonsIcon;

    public override void ModifyBaseGameModeModel(ModModel gameModeModel)
    {
        
    }

    public override void ModifyGameModel(GameModel gameModel)
    {
        gameModel.bloons.ForEach((bloon) =>
        {
            bloon.speed *= 2.0f;
        });
    }
}