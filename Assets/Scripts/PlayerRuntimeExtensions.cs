using UnityEngine;

public static class PlayerRuntimeExtensions
{
    public static bool TryMovePlayer(this Player player, Vector2Int direction)
    {
        if (!player.IsIdleState())
        {
            return false;
        }

        if (!player.CheckMove(direction.x, direction.y))
        {
            return false;
        }

        player.transform.rotation = GetFacing(direction);
        return true;
    }

    public static void RestorePlayerSnapshot(this Player player, GameObject parentCell, int gridX, int gridY, Vector3 worldPos, Quaternion worldRot)
    {
        Animator animator = player.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }

        player.RestoreBlockSnapshot(parentCell, gridX, gridY, worldPos, worldRot);
    }

    private static Quaternion GetFacing(Vector2Int direction)
    {
        if (direction.x < 0) return Quaternion.LookRotation(Vector3.left);
        if (direction.x > 0) return Quaternion.LookRotation(Vector3.right);
        if (direction.y < 0) return Quaternion.LookRotation(Vector3.back);
        return Quaternion.LookRotation(Vector3.forward);
    }
}
