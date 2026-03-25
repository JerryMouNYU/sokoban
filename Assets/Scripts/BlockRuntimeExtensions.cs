using UnityEngine;

public static class BlockRuntimeExtensions
{
    public static readonly Vector2Int[] OrthogonalDirections =
    {
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
    };

    public static bool IsIdleState(this Block block)
    {
        return block.State == Block.MoveStates.idle;
    }

    public static bool TryGetAdjacentBlock(this Block block, Vector2Int offset, out Block adjacentBlock)
    {
        adjacentBlock = null;
        GridManager gridManager = block.GetComponentInParent<GridManager>();
        if (gridManager == null)
        {
            return false;
        }

        int checkX = block.gridPos.x + offset.x;
        int checkY = block.gridPos.y + offset.y;
        Cell adjacentCell = gridManager.GetCell(checkX, checkY);
        if (adjacentCell == null || adjacentCell.ContainObj == null)
        {
            return false;
        }

        return adjacentCell.ContainObj.TryGetComponent(out adjacentBlock);
    }

    public static void RestoreBlockSnapshot(this Block block, GameObject parentCell, int gridX, int gridY, Vector3 worldPos, Quaternion worldRot)
    {
        block.State = Block.MoveStates.idle;
        block.moveChange = Vector2Int.zero;
        block.targetPos = worldPos;
        block.transform.SetPositionAndRotation(worldPos, worldRot);
        block.SetNewGridPos(parentCell, gridX, gridY);
    }
}
