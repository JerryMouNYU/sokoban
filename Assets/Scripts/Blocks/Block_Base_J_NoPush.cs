using UnityEngine;

/// <summary>
/// Block variant that can be moved itself, but never pushes another occupied block.
/// </summary>
public class Block_Base_J_NoPush : Block_Base_J
{
    protected override bool CanEnterOccupiedCell(Block hitObj, int deltaX, int deltaY)
    {
        return false;
    }
}
