using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SokobanCommandController : MonoBehaviour
{
    private readonly Stack<BoardSnapshot> undoStack = new Stack<BoardSnapshot>();
    private BoardSnapshot latestStableSnapshot;
    private bool wasBoardIdle;
    private bool isRestoringSnapshot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeOnLoad()
    {
        EnsurePresent();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsurePresent();
    }

    private static void EnsurePresent()
    {
        if (Object.FindFirstObjectByType<SokobanCommandController>() != null)
        {
            return;
        }

        GameObject controllerObject = new GameObject("SokobanCommandController");
        controllerObject.AddComponent<SokobanCommandController>();
    }

    private void Start()
    {
        wasBoardIdle = AreAllBlocksIdle();
        if (wasBoardIdle)
        {
            latestStableSnapshot = BoardSnapshot.Capture();
        }
    }

    private void Update()
    {
        bool isBoardIdle = AreAllBlocksIdle();

        if (!isRestoringSnapshot && wasBoardIdle && !isBoardIdle && latestStableSnapshot != null)
        {
            undoStack.Push(latestStableSnapshot);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoLastCommand();
        }

        if (!isRestoringSnapshot && isBoardIdle)
        {
            latestStableSnapshot = BoardSnapshot.Capture();
        }

        wasBoardIdle = isBoardIdle;
    }

    private void UndoLastCommand()
    {
        if (undoStack.Count == 0 || !AreAllBlocksIdle())
        {
            return;
        }

        isRestoringSnapshot = true;
        undoStack.Pop().Restore();
        latestStableSnapshot = BoardSnapshot.Capture();
        wasBoardIdle = true;
        isRestoringSnapshot = false;
    }

    private bool AreAllBlocksIdle()
    {
        Block[] blocks = Object.FindObjectsByType<Block>(FindObjectsSortMode.None);
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!blocks[i].IsIdleState())
            {
                return false;
            }
        }

        return true;
    }
}

public class BoardSnapshot
{
    private readonly GridManager gridManager;
    private readonly List<BlockSnapshot> blockSnapshots;

    private BoardSnapshot(GridManager gridManager, List<BlockSnapshot> blockSnapshots)
    {
        this.gridManager = gridManager;
        this.blockSnapshots = blockSnapshots;
    }

    public static BoardSnapshot Capture()
    {
        Block[] blocks = Object.FindObjectsByType<Block>(FindObjectsSortMode.None);
        List<BlockSnapshot> snapshots = new List<BlockSnapshot>(blocks.Length);
        GridManager activeGridManager = null;

        for (int i = 0; i < blocks.Length; i++)
        {
            Block block = blocks[i];
            if (activeGridManager == null)
            {
                activeGridManager = block.GetComponentInParent<GridManager>();
                if (activeGridManager == null)
                {
                    activeGridManager = Object.FindFirstObjectByType<GridManager>();
                }
            }

            snapshots.Add(new BlockSnapshot(block, block.gridPos, block.transform.position, block.transform.rotation));
        }

        return new BoardSnapshot(activeGridManager, snapshots);
    }

    public void Restore()
    {
        if (gridManager == null)
        {
            return;
        }

        gridManager.ClearGridOccupants();

        for (int i = 0; i < blockSnapshots.Count; i++)
        {
            blockSnapshots[i].Restore(gridManager);
        }

        gridManager.UpdateGrid();
    }
}

public class BlockSnapshot
{
    private readonly Block block;
    private readonly Vector2Int gridPosition;
    private readonly Vector3 worldPosition;
    private readonly Quaternion worldRotation;

    public BlockSnapshot(Block block, Vector2Int gridPosition, Vector3 worldPosition, Quaternion worldRotation)
    {
        this.block = block;
        this.gridPosition = gridPosition;
        this.worldPosition = worldPosition;
        this.worldRotation = worldRotation;
    }

    public void Restore(GridManager gridManager)
    {
        GameObject cellObject = gridManager.gridList[gridPosition.x][gridPosition.y];
        if (block is Player player)
        {
            player.RestorePlayerSnapshot(cellObject, gridPosition.x, gridPosition.y, worldPosition, worldRotation);
            return;
        }

        block.RestoreBlockSnapshot(cellObject, gridPosition.x, gridPosition.y, worldPosition, worldRotation);
    }
}
