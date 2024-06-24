using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
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

    private void Update()
    {
        Image image = GetComponent<Image>();
        // ModHelper.Msg<ImpoppablePlus>(image.sprite.name);

        if (image.sprite.name == "MainBgPanelHematite(Clone)"){
            image.SetSprite(basicBackground);
            Sprite spr = Sprite.Create(textureToCopy, 
                                       new Rect(0.0f, 0.0f, image.sprite.texture.width, image.sprite.texture.height),
                                       new Vector2(0.5f, 0.5f),
                                       image.sprite.pixelsPerUnit,
                                       0,
                                       SpriteMeshType.FullRect,
                                       image.sprite.border);
            image.SetSprite(spr);
        }
    }
}