using UnityEngine;

public class BlockPlacer : MonoBehaviour
{
    [SerializeField] private Transform representer;
    [SerializeField] private Vector3Int represents;

    private BlockHandler handler;

    private const float MOVE_SPEED = 5;

    private const float INPUT_RATE = .1f;
    private float inputTimer;

    private void Awake()
    {
        handler = GetComponent<BlockHandler>();
    }

    private void Update()
    {
        inputTimer += Time.deltaTime;

        if(inputTimer >= INPUT_RATE)
        {
            float h = Input.GetAxisRaw("Horizontal") * MOVE_SPEED;
            float v = Input.GetAxisRaw("Vertical") * MOVE_SPEED;

            if(Mathf.Abs(h) >= 1)
            {
                represents.x += (int)Mathf.Sign(h);
                inputTimer = 0;
            }

            if (Mathf.Abs(v) >= 1)
            {
                represents.z += (int)Mathf.Sign(v);
                inputTimer = 0;
            }
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

        if (Input.GetKey(KeyCode.Mouse2))
        {
            PlaceAdditive();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Remove();
        }
        

        representer.position = represents;
        representer.position += Vector3.up * .1f;
    }

    public void Place()
    {
        handler.PlaceScaffolding(represents);
    }

    public void PlaceAdditive()
    {
        handler.PlaceScaffoldingExtender(represents);
    }

    public void Remove()
    {
        handler.RemoveBlock(represents);
    }
}
