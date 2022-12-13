using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector3Int Position { get; private set; }
    protected BlockHandler blockHandler;
    public virtual void Initialize (Vector3Int pos,BlockHandler blockHandler)
    {
        this.Position = pos;
        this.blockHandler = blockHandler;
    }

    public virtual void Tick (Block tickComingFrom)
    {

    }

    public virtual void OnDestroyed ()
    {

    }
}
