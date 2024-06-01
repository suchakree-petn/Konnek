using System.Collections;
using UnityEngine;
using DG.Tweening;

public partial class CardHolder
{
    [HideInInspector] public bool IsPopUp;
    
    [Header("Animation Reference")]
    [SerializeField] private Animator sheenAnim;
    [SerializeField] private Animator reflectAnim;
    private void ReflectAndSheen()
    {
        sheenAnim.Play("card_sheening");
        reflectAnim.Play("card_reflect");
    }
}