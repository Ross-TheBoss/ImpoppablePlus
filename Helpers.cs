using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Models.Store;
using Il2CppAssets.Scripts.Models.Store.Loot;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ImpoppablePlus;

[RegisterTypeInIl2Cpp(false)]
public class MatchColor : MonoBehaviour
{
    #nullable enable
    public GameObject imageToCopy;

    public MatchColor(System.IntPtr ptr) : base(ptr)
    {

    }

    private void LateUpdate()
    {
        GetComponent<Image>().color = imageToCopy.GetComponent<Image>().color;
    }
}

[RegisterTypeInIl2Cpp(false)]
public class MatchTexture : MonoBehaviour
{
    #nullable enable
    public Texture2D textureToCopy;
    public SpriteReference basicBackground;

    public MatchTexture(System.IntPtr ptr) : base(ptr)
    {

    }

    private void Retexture(){
        Image image = GetComponent<Image>();
        // ModHelper.Msg<ImpoppablePlus>(image.sprite.name);

        if (image.sprite.name == "MainBgPanelHematite(Clone)"){
            Sprite spr = Sprite.Create(textureToCopy, 
                                       new Rect(0.0f, 0.0f, textureToCopy.width, textureToCopy.height),
                                       new Vector2(0.5f, 0.5f),
                                       5.4f,
                                       0,
                                       SpriteMeshType.FullRect,
                                       new Vector4(32f, 32f, 32f, 32f));
            image.SetSprite(spr);
        }
    }
    private void Update()
    {
        Retexture();
    }

    private void LateUpdate()
    {
        Retexture();
    }

    private void Start()
    {
        Retexture();
    }
}

[RegisterTypeInIl2Cpp(false)]
public class AlterRewards: MonoBehaviour {
    public DifficultySelectMmItems difficultySelectMmItems;

    public string selectedMapId;

    public string difficulty;

    public string[] modes;

     public AlterRewards(System.IntPtr ptr) : base(ptr)
    {

    }

    private void Start(){
        int maxReward = 0;
        foreach (string mode in modes){
            int reward = Game.instance.GetMonkeyMoneyReward(selectedMapId, difficulty, mode, Game.instance.model, true, false);
            #if DEBUG
            ModHelper.Msg<ImpoppablePlus>($"{selectedMapId} {difficulty}: {mode} = ${reward}");
            #endif
            if (reward > maxReward){
                maxReward = reward;
            }
        }

        var largeAmount = difficultySelectMmItems.gameObject.GetComponentInChildrenByName<Transform>("LargeAmount").gameObject;
        if (largeAmount.activeSelf){
            difficultySelectMmItems.largeTxt.text = $"${maxReward}";
        } else {
            difficultySelectMmItems.smallTxt.text = $"${maxReward}";
        }
    }

}