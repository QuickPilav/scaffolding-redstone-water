using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] private Transform representer;
    [SerializeField] private Vector3Int represents;
    //[SerializeField] private BlockType blockType;

    private BlockHandler handler;

    private void Awake()
    {
        handler = GetComponent<BlockHandler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            represents.x += -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            represents.x += 1;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            represents.z += 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            represents.z += -1;
        }

        /*
        if (Input.GetKeyDown(KeyCode.F1))
        {
            blockType = BlockType.Base;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            blockType = BlockType.Platform;
        }
        */

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Place();
        }
        
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Remove();
        }
        

        representer.position = represents;
        representer.position += Vector3.up * .1f;
    }

    [ContextMenu("Place!!!")]
    public void Place()
    {
        handler.PlaceScaffolding(represents);
    }

    [ContextMenu("Place!!!")]
    public void Remove()
    {
        try
        {
            handler.RemoveBlock(represents);
        }
        catch (System.Exception)
        {

        }
    }
}
