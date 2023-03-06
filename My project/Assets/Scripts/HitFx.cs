using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFx : MonoBehaviour
{
    [HideInInspector] public int fxType;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fxType = 0;
    }
    private void OnEnable()
    {
        animator.SetInteger("fxType", fxType);
    }
    private void Disable_Object()
    {
        HitFxPool.Instance.AddToPool(gameObject);
    }
}
