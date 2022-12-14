using System.Collections.Generic;
using UnityEngine;


public class Scaffolding : Block
{
    private const int BREAK_DISTANCE = 7;

    protected virtual bool isExtender => false;

    public int Distance { get; private set; }
    public int ExtendedDistance { get; private set; }

    private MeshRenderer mRenderer;

    public override void Initialize(Vector3Int pos, BlockHandler blockHandler)
    {
        base.Initialize(pos, blockHandler);

        blockHandler.ScheduleTick(this);
        blockHandler.UpdateSurroundings(Position);

        mRenderer = GetComponent<MeshRenderer>();
    }

    public override void Tick()
    {
        base.Tick();

        int distanceNormal = GetDistance(Position, isExtender, false);
        int distanceToAdditive = GetDistance(Position, isExtender, true);

        if (isExtender)
        {
            if (blockHandler.GetSurroundings(Position, out List<Block> blocks))
            {
                int[] distances = new int[blocks.Count];
                for (int i = 0; i < blocks.Count; i++)
                {
                    var bl = blocks[i];
                    if (bl is not Scaffolding scaff)
                        continue;

                    if (scaff.isExtender)
                    {
                        blockHandler.RemoveBlock(Position);
                        return;
                    }

                    distances[i] = GetDistance(scaff.Position, scaff.isExtender, false, bl);

#if UNITY_EDITOR
                    blockHandler.SpawnText(blocks[i].Position, distances[i].ToString(), Color.cyan);
#endif

                }
                int closestRealDistance = Mathf.Min(distances);

                if (closestRealDistance == BREAK_DISTANCE)
                {
                    blockHandler.RemoveBlock(Position);
                    return;
                }
            }

            if (distanceNormal == BREAK_DISTANCE)
            {
                blockHandler.RemoveBlock(Position);
                return;
            }
        }

        if (distanceNormal == BREAK_DISTANCE && distanceToAdditive == BREAK_DISTANCE)
        {
            blockHandler.RemoveBlock(Position);
            return;
        }
        else if (distanceNormal != Distance || distanceToAdditive != ExtendedDistance)
        {
            Distance = distanceNormal;
            ExtendedDistance = distanceToAdditive;

            mRenderer.materials[0].color = Color.Lerp(Color.green, Color.red, ((float)Distance + 1) / BREAK_DISTANCE);
            mRenderer.materials[1].color = Color.Lerp(Color.green, Color.red, ((float)ExtendedDistance + 1) / BREAK_DISTANCE);

            blockHandler.UpdateSurroundings(Position);
        }
    }

    private int GetDistance(Vector3Int pos, bool forceSource, bool lookForAdditiveToo, params Block[] exceptions)
    {
        int i = BREAK_DISTANCE;

        var blockUnder = blockHandler.GetBlock(pos + Vector3Int.down);

        //altýnda blok varsa onun deðerini al...
        if (blockUnder is Scaffolding underScaff)
        {
            i = underScaff.Distance;
        }
        else if (!IsInAir())
        {
            //altýnda blok yoksa ve ve yere deyiyor isek biz kaynaðýz.
            return 0;
        }

        //etrafýndaki bloklar bir kaynaða daha yakýn ise direkt onu kullan.
        if (blockHandler.GetSurroundings(pos, out List<Block> blocks))
        {
            foreach (var block in blocks)
            {
                bool skip = false;
                for (int ix = 0; ix < exceptions.Length; ix++)
                {
                    if (exceptions[ix] == block)
                    {
                        skip = true;
                        break;
                    }
                }

                if (skip)
                    continue;

                if (block is Scaffolding scaff)
                {
                    if (forceSource)
                        return 0;

                    if (!lookForAdditiveToo && scaff.isExtender)
                    {
                        continue;
                    }

                    i = Mathf.Min(i, scaff.Distance + 1);
                    if (lookForAdditiveToo)
                    {
                        i = Mathf.Min(i, scaff.ExtendedDistance + 1);
                    }
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
