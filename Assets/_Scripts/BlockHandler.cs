using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockHandler : MonoBehaviour
{
    public Dictionary<Vector3Int, Block> Blocks = new Dictionary<Vector3Int, Block>();


    private readonly Queue<Block> tickScheduler = new Queue<Block>();
    private readonly Queue<GameObject> destroyQueue = new Queue<GameObject>();

    private readonly float tickRate = .2f;
    private float tickTimer = 0f;

    private readonly float destroyRate = .05f;
    private float destroyTimer = 0f;

#if UNITY_EDITOR
    [SerializeField] private GameObject go;
#endif
    [Space]
    [SerializeField] private BlockAudioHandler audioHandler;
    [SerializeField] private Scaffolding scaffoldingPrefab;
    [SerializeField] private ScaffoldingExtender scaffoldingExtenderPrefab;

    private void Awake()
    {
        audioHandler.Initialize();
    }
    public void PlaceScaffolding(Vector3Int pos)
    => PlaceBlock(pos, scaffoldingPrefab);

    public void PlaceScaffoldingExtender(Vector3Int pos)
    => PlaceBlock(pos, scaffoldingExtenderPrefab);

    public void PlaceBlock(Vector3Int pos, Block block)
    {
        if (Blocks.ContainsKey(pos))
        {
            return;
        }

        var blockSpawned = Instantiate(block, pos, Quaternion.LookRotation(Vector3.up));

        Blocks.Add(pos, blockSpawned);
        blockSpawned.Initialize(pos, this);

        audioHandler.PlayPlaceSound();
    }

    public void RemoveBlock(Vector3Int pos)
    {
        if (!Blocks.TryGetValue(pos, out var block))
        {
            return;
        }

        block.OnDestroyed();

        Blocks.Remove(pos);

        //Destroy(block.gameObject);

        destroyQueue.Enqueue(block.gameObject);
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

    public void Update()
    {
        Tickrate();
        DestroyRate();
    }

    private void Tickrate()
    {
        if (tickScheduler.Count == 0)
        {
            return;
        }

        tickTimer += Time.deltaTime;

        if (tickTimer < tickRate)
            return;

        tickTimer -= tickRate;

        while (tickScheduler.Count != 0)
        {
            var dequeued = tickScheduler.Dequeue();
            if (dequeued == null)
            {
                return;
            }

            HandleTick(dequeued);
        }
    }

    private void DestroyRate()
    {
        if (destroyQueue.Count == 0)
        {
            return;
        }

        destroyTimer += Time.deltaTime;

        if (destroyTimer < destroyRate)
            return;

        destroyTimer -= destroyRate;

        while (destroyQueue.Count != 0)
        {
            var dequeued = destroyQueue.Dequeue();
            if (dequeued == null)
            {
                return;
            }

            HandleDestroy(dequeued);
            return;
        }
    }

    private void HandleTick (Block blockToTick)
    {
        blockToTick.Tick();

#if UNITY_EDITOR
        Destroy(Instantiate(go,(Vector3)blockToTick.Position,Quaternion.identity),1f);
#endif
    }

    private void HandleDestroy(GameObject obj)
    {
        Destroy(obj);
        audioHandler.PlayDestroySound();
    }

#if UNITY_EDITOR
    public void SpawnText (Vector3Int pos,string text, Color col)
    {
        GameObject go = new GameObject("text", typeof(TMPro.TextMeshPro));

        pos += Vector3Int.up;

        go.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(Vector3.up));


        var t = go.GetComponent<TMPro.TextMeshPro>();
        t.text = text;
        t.fontSize = 12;
        t.color = col;
        t.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
        t.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;

        Destroy(go,5f);
    }
#endif

}
