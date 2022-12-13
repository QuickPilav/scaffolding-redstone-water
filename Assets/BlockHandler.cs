using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockHandler : MonoBehaviour
{
    public Dictionary<Vector3Int, Block> Blocks = new Dictionary<Vector3Int, Block>();

    [SerializeField] private Scaffolding scaffoldingPrefab;

    public void PlaceScaffolding(Vector3Int pos)
    => PlaceBlock(pos, scaffoldingPrefab);

    public void PlaceBlock(Vector3Int pos, Block block)
    {
        if (Blocks.ContainsKey(pos))
        {
            return;
        }

        var blockSpawned = Instantiate(block, pos, Quaternion.identity);

        Blocks.Add(pos, blockSpawned);
        blockSpawned.Initialize(pos, this);
    }

    public void RemoveBlock(Vector3Int pos)
    {
        if (!Blocks.TryGetValue(pos, out var block))
        {
            return;
        }

        block.OnDestroyed();

        Blocks.Remove(pos);

        Destroy(block.gameObject);
    }

    public Block GetBlock(Vector3Int pos)
    {
        Blocks.TryGetValue(pos, out var block);

        return block;
    }

    public bool GetSurroundings(Vector3Int pos, out List<Block> blocks)
    {
        blocks = new()
        {
            GetBlock(pos + Vector3Int.right),
            GetBlock(pos + Vector3Int.left),
            GetBlock(pos + Vector3Int.forward),
            GetBlock(pos + Vector3Int.back)
        };

        blocks = blocks.Where(item => item != null).ToList();

        if (blocks.Count == 0)
            return false;

        return true;
    }


    public void ScheduleTick(Block blockToTick)
    {
        tickScheduler.Enqueue(blockToTick);
    }

    public void UpdateSurroundings(Vector3Int pos)
    {
        if (GetSurroundings(pos, out List<Block> blocks))
        {
            blocks.ForEach(x => ScheduleTick(x));
        }
    }

    private readonly Queue<Block> tickScheduler = new Queue<Block>();
    private readonly float tickRate = .2f;

    private float tickTimer = 0f;

    [SerializeField] private GameObject go;

    public void Update()
    {
        if (tickScheduler.Count == 0)
        {
            return;
        }

        tickTimer += Time.deltaTime;

        if (tickTimer < tickRate)
            return;

        tickTimer -= tickRate;

        /*int i = 0;*/
        
        while (tickScheduler.Count != 0 /*&& i <20*/)
        {
            var dequeued = tickScheduler.Dequeue();
            if(dequeued == null)
            {
                return;
            }

            HandleTick(dequeued);
            //i++;
        }
        
    }

    private void HandleTick (Block blockToTick)
    {
        blockToTick.Tick(blockToTick);
#if UNITY_EDITOR
        Destroy(Instantiate(go,(Vector3)blockToTick.Position,Quaternion.identity),1f);
#endif
    }

}
