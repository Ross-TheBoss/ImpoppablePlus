using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.Scenarios;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Unity.UI_New.Main.DifficultySelect;
using Il2CppAssets.Scripts.Unity.UI_New.Main.MapSelect;
using Il2CppAssets.Scripts.Unity.UI_New.Main.ModeSelect;
using Il2CppNinjaKiwi.Common;
using ImpoppablePlus;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(ImpoppablePlus.ImpoppablePlus), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace ImpoppablePlus;

public class ImpoppablePlus : BloonsTD6Mod
{
    public static bool impoppableDifficultySelected = false;

    public static bool impoppableSandboxSelected = false;

    public override void OnApplicationStart()
    {
        ModHelper.Msg<ImpoppablePlus>("ImpoppablePlus loaded!");
    }

    public override void OnNewGameModel(GameModel result)
    {
        if (!impoppableSandboxSelected) return;
        impoppableSandboxSelected = false;

        result.startRound = 6;
        result.towers.Do((towerModel) => towerModel.cost *= 10f / 9f);
    }

    public override void OnUpdate()
    {
        #if DEBUG
        CashCalculator.OnUpdate();
        #endif
    }

    public static void exportImage(Image image, string fileName) {
        image.SaveToPNG(fileName);

        var texRect = image.sprite.textureRect;
        #if DEBUG
        ModHelper.Msg<ImpoppablePlus>($"{fileName}: {texRect.width}x{texRect.height}+{texRect.x}+{texRect.y}");
        #endif
    }

    [HarmonyPatch(typeof(DifficultySelectScreen), nameof(DifficultySelectScreen.Open))]
    internal static class DifficultySelectScreen_Open
    {
        [HarmonyPrefix]
        private static void Prefix(DifficultySelectScreen __instance)
        {
            #if DEBUG
            ModHelper.Msg<ImpoppablePlus>("Injecting into difficulty select screen...");
            #endif

            GameObject difficulties = __instance.gameObject;

            // Modify Other Difficulties Positions
            var easyDifficulty = difficulties.GetComponentInChildrenByName<Transform>("Easy").gameObject;
            easyDifficulty.transform.localPosition = easyDifficulty.transform.localPosition.WithX(-1200);

            var mediumDifficulty = difficulties.GetComponentInChildrenByName<Transform>("Medium").gameObject;
            mediumDifficulty.transform.localPosition = mediumDifficulty.transform.localPosition.WithX(-400);

            var hardDifficulty = difficulties.GetComponentInChildrenByName<Transform>("Hard").gameObject;
            hardDifficulty.transform.localPosition = hardDifficulty.transform.localPosition.WithX(400);

            // Add Impoppable Difficulty
            var proto = difficulties.GetComponentInChildrenByName<Transform>("Hard").gameObject;

            var newButton = Object.Instantiate(proto, proto.transform.parent);
            newButton.name = "Impoppable";
            newButton.transform.localPosition = newButton.transform.localPosition.WithX(1200);

            newButton.gameObject.GetComponentInChildrenByName<Image>("ModeIcon").SetSprite(VanillaSprites.ImpoppableBtn);
            newButton.gameObject.GetComponentInChildrenByName<Image>("BadgeImage").SetSprite(VanillaSprites.MedalImpoppable);

            newButton.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Difficulty").localizeKey =
                "Mode Impoppable";

            newButton.gameObject.GetComponentInChildrenByName<Button>("Button").SetOnClick(() =>
            {
                #if DEBUG
                ModHelper.Msg<ImpoppablePlus>("Impoppable Button Clicked!");
                #endif
                __instance.OpenModeSelectUi("Hard");
                impoppableDifficultySelected = true;
            }
            );

            var matchScale = newButton.AddComponent<MatchScale>();
            matchScale.transformToCopy = proto.transform;
        }
    }

    [HarmonyPatch(typeof(ModeScreen), nameof(ModeScreen.Open))]
    internal static class ModeScreen_Open
    {
        private static void ReplaceArrow(GameObject obj, GameObject proto) {
            // Add the proto arrow
            var protoLockedState = proto.GetComponentInChildrenByName<Transform>("LockedState").gameObject;
            var objLockedState = obj.GetComponentInChildrenByName<Transform>("LockedState").gameObject;

            var protoArrow = protoLockedState.GetComponentInChildrenByName<Transform>("Arrow").gameObject;
            objLockedState.GetComponentInChildrenByName<Transform>("Arrow").gameObject.Destroy();

            var newObjArrow = Object.Instantiate(protoArrow, objLockedState.transform);
            newObjArrow.name = "Arrow";
        }
        private static void ReplaceGameMode<T>(GameObject standard, GameObject proto, T gameMode,
                                               string unlockMode, string medal) where T: ModGameMode {
            // Replace proto with the new game mode.
            var obj = Object.Instantiate(standard, standard.transform.parent);

            obj.name = gameMode.Id;
            obj.transform.localPosition = proto.transform.localPosition;
            obj.GetComponent<Image>().LoadSprite(gameMode.IconReference);

            obj.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Mode").localizeKey =
                "Mode " + gameMode.Id;

            obj.gameObject.GetComponentInChildrenByName<Image>("Image").LoadSprite(
                ModContent.GetSpriteReferenceOrDefault<ImpoppablePlus>(medal)
            );

            var button = obj.GetComponent<ModeButton>();
            button.modeType = gameMode.Id;
            button.unlockMode = unlockMode;

            // Add the proto arrow
            ReplaceArrow(obj, proto);

            var matchScale = obj.AddComponent<MatchScale>();
            matchScale.transformToCopy = standard.transform;
        }

        [HarmonyPrefix]
        private static void Prefix(ModeScreen __instance)
        {
            if (!impoppableDifficultySelected) return;

            #if DEBUG
            ModHelper.Msg<ImpoppablePlus>("Impoppable Mode Screen Opened!");
            #endif

            var impoppableModes = __instance.hardModes.gameObject;
            impoppableModes.name = "ImpoppableModes";

            var impoppableModeObj = impoppableModes.GetComponentInChildrenByName<Transform>("Impoppable").gameObject;

            // Replace Magic Only with Support Only
            var supportOnlyGameMode = ModContent.GetInstance<SupportOnly>();
            var mmoModeProto = impoppableModes.GetComponentInChildrenByName<Transform>("MagicOnly").gameObject;

            ReplaceGameMode(impoppableModeObj, mmoModeProto, supportOnlyGameMode,
                            "Impoppable", "SupportOnlyMedal");

            // Replace Double Hp Moabs with Double Speed
            var doubleSpeedGameMode = ModContent.GetInstance<DoubleSpeed>();
            var dhmModeProto = impoppableModes.GetComponentInChildrenByName<Transform>("DoubleHpMoabs").gameObject;

            ReplaceGameMode(impoppableModeObj, dhmModeProto, doubleSpeedGameMode,
                            ModContent.GameModeId<SupportOnly>(), "DoubleSpeedMedal");

            // Replace Impoppable with Mastery Mode
            var masteryGameMode = ModContent.GetInstance<Mastery>();
            ReplaceGameMode(impoppableModeObj, impoppableModeObj, masteryGameMode, "Clicks", "MasteryMedal");


            // Move Impoppable Mode to the Standard Mode Position. 
            var standardModeProto = impoppableModes.GetComponentInChildrenByName<Transform>("Standard").gameObject;
            impoppableModeObj.transform.localPosition = standardModeProto.transform.localPosition;

            // Remove the Impoppable Mode Arrow
            impoppableModeObj.GetComponentInChildrenByName<Transform>("Arrow").gameObject.Destroy();

            // Change the Impoppable Mode Icon to the Standard Icon 
            var impoppableModeButton = impoppableModeObj.GetComponentInChildren<ModeButton>();
            impoppableModeButton.GetComponent<Image>().SetSprite(VanillaSprites.BlueRoundPlayBtn);
            // exportImage(impoppableModeButton.GetComponent<Image>(), "BlueRoundPlayBtn.png");

            impoppableModeObj.gameObject.GetComponentInChildrenByName<NK_TextMeshProUGUI>("Mode").localizeKey =
                "Mode Standard";
            impoppableModeButton.unlockMode = ""; // Remove Alternate Bloons Rounds Unlock Requirement

            // Move CHIMPS Mode to the ABR Position.
            var abrModeProto = impoppableModes.GetComponentInChildrenByName<Transform>("AlternateBloonsRounds").gameObject;
            var chimpsModeObj = impoppableModes.GetComponentInChildrenByName<Transform>("Clicks").gameObject;
            chimpsModeObj.transform.localPosition = abrModeProto.transform.localPosition;

            // Replace the CHIMPS arrow with the ABR Arrow
            ReplaceArrow(chimpsModeObj, abrModeProto);

            // Modify Sandbox
            var sandboxObj = impoppableModes.GetComponentInChildrenByName<Transform>("Sandbox").gameObject;
            var sandboxButton = sandboxObj.GetComponent<ModeButton>();
            sandboxButton.unlockMode = "Impoppable";
            sandboxButton.button.AddOnClick(() =>
            {
                #if DEBUG
                ModHelper.Msg<ImpoppablePlus>("Sandbox Button Clicked!");
                #endif
                impoppableSandboxSelected = true;
            });

            // Remove the previous mode buttons.
            foreach (string mode in new string[] { "Standard", "MagicOnly", "DoubleHpMoabs", "HalfCash", "AlternateBloonsRounds" }) {
                impoppableModes.GetComponentInChildrenByName<Transform>(mode).gameObject.Destroy();
            }
        }

        [HarmonyPostfix]
        private static void Postfix(ModeScreen __instance)
        {
            if (!impoppableDifficultySelected) {
                if (__instance.hardModes.activeSelf)
                {
                    var hardModes = __instance.hardModes.gameObject;
                    hardModes.GetComponentInChildrenByName<Transform>("Impoppable").gameObject.Destroy();
                    hardModes.GetComponentInChildrenByName<Transform>("Clicks").gameObject.Destroy();
                }
                return;
            }
            impoppableDifficultySelected = false;

            __instance.headerTxt.text = "Impoppable";
            __instance.medal.SetSprite(VanillaSprites.MedalImpoppable);

            __instance.subTxt.text =
                "On Impoppable Monkeys cost much more than normal, you start at round 6, the Bloons move a little faster and you only have 1 life. " +
                "Beating round 100 in Impoppable awards you an impoppable medal for that track.";
        }
    }

    [HarmonyPatch(typeof(MapSelectScreen), nameof(MapSelectScreen.Open))]
    internal static class MapSelectScreen_Open
    {
        [HarmonyPrefix]
        private static void Prefix(MapSelectScreen __instance) {
            __instance.gameObject.GetComponentsInChildren<MapButton>(includeInactive: true)
                .Do((MapButton mapButton) =>
                {
                    /*
                    Empty Gold:
                        Local Position: 105 45
                    Empty (8):
                        Local Position: 16 33
                        Relative Position: -89 -12
                    Empty (9):
                        Local Position: 191 32
                        Relative Position: 86 -13
                    Empty (10):
                        Local Position: 37 -21
                        Relative Position: -68 -66
                    Empty (11):
                        Local Position: 171 -19
                        Relative Position: 66 -64
                    
                    Empty Impoppable:
                        Local Position: 313 45
                    Empty (14):
                        Local Position: 376 -20
                        Relative Position: 63 -65
                    Empty (15):
                        Local Position: 248.7 -20
                        Relative Position: -64.3 -65

                    */
                    Vector3 impoppableMedalLocalPosition = new Vector3(313.0f, 45.0f);
                    Vector3 chimpsMedalRelativePosition = new Vector3(66.0f, -13.0f);
                    Vector3 supportOnlyRelativePosition = new Vector3(-69.0f, -12.0f);
                    Vector3 doubleSpeedRelativePosition = new Vector3(-48.0f, -66.0f);
                    Vector3 masteryRelativePosition = new Vector3(46.0f, -64.0f);

                    // Medals
                    var clicksMedal = mapButton.medals.GetComponentInChildrenByName<Transform>("Clicks").gameObject;
                    clicksMedal.transform.localPosition = impoppableMedalLocalPosition + chimpsMedalRelativePosition;

                    var specialClicksMedal = mapButton.medals.GetComponentInChildrenByName<Transform>("SpecialClicks").gameObject;
                    specialClicksMedal.transform.localPosition = impoppableMedalLocalPosition + chimpsMedalRelativePosition;

                    var supportOnlyMedal = Object.Instantiate(clicksMedal, clicksMedal.transform.parent);
                    supportOnlyMedal.transform.localPosition = impoppableMedalLocalPosition + supportOnlyRelativePosition;
                    supportOnlyMedal.name = ModContent.GameModeId<SupportOnly>();
                    supportOnlyMedal.gameObject.GetComponentInChildrenByName<Image>("Image").SetSprite(
                        ModContent.GetSprite<ImpoppablePlus>("SupportOnlyMedal")
                    );

                    var doubleSpeedMedal = Object.Instantiate(clicksMedal, clicksMedal.transform.parent);
                    doubleSpeedMedal.transform.localPosition = impoppableMedalLocalPosition + doubleSpeedRelativePosition;
                    doubleSpeedMedal.name = ModContent.GameModeId<DoubleSpeed>();
                    doubleSpeedMedal.gameObject.GetComponentInChildrenByName<Image>("Image").SetSprite(
                        ModContent.GetSprite<ImpoppablePlus>("DoubleSpeedMedal")
                    );

                    var masteryMedal = Object.Instantiate(clicksMedal, clicksMedal.transform.parent);
                    masteryMedal.transform.localPosition = impoppableMedalLocalPosition + masteryRelativePosition;
                    masteryMedal.name = ModContent.GameModeId<Mastery>();
                    masteryMedal.gameObject.GetComponentInChildrenByName<Image>("Image").SetSprite(
                        ModContent.GetSprite<ImpoppablePlus>("MasteryMedal")
                    );

                    // Containers
                    var clicksMedalContainer = mapButton.gameObject.GetComponentInChildrenByName<Transform>("Empty (14)").gameObject;
                    clicksMedalContainer.transform.localPosition = impoppableMedalLocalPosition + chimpsMedalRelativePosition;

                    var supportOnlyMedalContainer = Object.Instantiate(clicksMedalContainer, clicksMedalContainer.transform.parent);
                    supportOnlyMedalContainer.transform.localPosition = impoppableMedalLocalPosition + supportOnlyRelativePosition;
                    supportOnlyMedalContainer.name = "Empty (15)";
                    supportOnlyMedalContainer.transform.MoveAfterSibling(clicksMedalContainer.transform, false);
                    var supportOnlyMatchImage = supportOnlyMedalContainer.AddComponent<MatchImage>();
                    supportOnlyMatchImage.imageToCopy = clicksMedalContainer;

                    var doubleSpeedMedalContainer = Object.Instantiate(clicksMedalContainer, clicksMedalContainer.transform.parent);
                    doubleSpeedMedalContainer.transform.localPosition = impoppableMedalLocalPosition + doubleSpeedRelativePosition;
                    doubleSpeedMedalContainer.name = "Empty (16)";
                    doubleSpeedMedalContainer.transform.MoveAfterSibling(supportOnlyMedalContainer.transform, false);
                    var doubleSpeedMatchImage = doubleSpeedMedalContainer.AddComponent<MatchImage>();
                    doubleSpeedMatchImage.imageToCopy = clicksMedalContainer;

                    var masteryMedalContainer = Object.Instantiate(clicksMedalContainer, clicksMedalContainer.transform.parent);
                    masteryMedalContainer.transform.localPosition = impoppableMedalLocalPosition + masteryRelativePosition;
                    masteryMedalContainer.name = "Empty (17)";
                    masteryMedalContainer.transform.MoveAfterSibling(doubleSpeedMedalContainer.transform, false);
                    var masteryMatchImage = masteryMedalContainer.AddComponent<MatchImage>();
                    masteryMatchImage.imageToCopy = clicksMedalContainer;
                });
        }
    }
}