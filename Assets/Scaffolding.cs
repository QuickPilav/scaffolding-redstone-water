using System.Collections.Generic;
using UnityEngine;


public class Scaffolding : Block
{
    private const int BREAK_DISTANCE = 7;

    public int Distance { get; private set; }

    public override void Initialize(Vector3Int pos, BlockHandler blockHandler)
    {
        base.Initialize(pos, blockHandler);

        blockHandler.ScheduleTick(this);
        blockHandler.UpdateSurroundings(Position);
    }


    public override void Tick(Block tickComingFrom)
    {
        base.Tick(tickComingFrom);

        int i = GetDistance(tickComingFrom.Position);


        if (i == BREAK_DISTANCE)
        {
            if (Distance == BREAK_DISTANCE)
            {
                //d��
            }
            else
            {
                blockHandler.RemoveBlock(Position);
                //p_222020_.destroyBlock(bl2, true);
            }
        }
        else if (i != Distance)
        {
            Debug.Log("DE����ME U�RADIM!!!");
            Distance = i;

            GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.green, Color.red, ((float)Distance + 1) / BREAK_DISTANCE);
#if UNITY_EDITOR
#endif

            blockHandler.UpdateSurroundings(Position);
        }
        //blockHandler.UpdateSurroundings(Position);
    }

    /*
    private bool CheckDistance()
    {
        SetDistance();


        Debug.Log($"k�rm�z� m�y�m? {distance}");
        if (distance == BREAK_DISTANCE)
        {
            blockHandler.RemoveBlock(Position);
            return false;
        }
        return true;
    }
    */

    private int GetDistance(Vector3Int pos)
    {
        int i = BREAK_DISTANCE;

        var blockUnder = blockHandler.GetBlock(pos + Vector3Int.down);

        //alt�nda blok varsa onun de�erini al...
        if (blockUnder is Scaffolding underScaff)
        {
            i = underScaff.Distance;
        }
        else if (!IsInAir())
        {
            //alt�nda blok yoksa ve ve yere deyiyor isek biz kayna��z.
            return 0;
        }


        //etraf�ndaki bloklar bir kayna�a daha yak�n ise direkt onu kullan.
        if (blockHandler.GetSurroundings(pos, out List<Block> blocks))
        {
            foreach (var block in blocks)
            {
                if (block is Scaffolding scaff)
                {
                    i = Mathf.Min(i, scaff.Distance + 1);
                    if (i == 1)
                    {
                        break;
                    }
                }
            }
        }


        return i;
    }

    public bool IsInAir()
    {
        return !Physics.Raycast(Position, Vector3.down, 1f);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        blockHandler.UpdateSurroundings(Position);
    }

}
