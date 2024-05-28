using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ImpoppablePlus;

[RegisterTypeInIl2Cpp(false)]
public class MatchImage : MonoBehaviour
{
    public GameObject imageToCopy;

    public MatchImage(System.IntPtr ptr) : base(ptr)
    {

    }

    private void LateUpdate()
    {
        GetComponent<Image>().color = imageToCopy.GetComponent<Image>().color;
    }
}