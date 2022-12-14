using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : StaticInstance<LayerManager>
{
    public static LayerMask HitLayer { get; private set; }

    [SerializeField] private LayerMask hitLayer;

    protected override void Awake()
    {
        base.Awake();

        HitLayer = hitLayer;
    }
}
