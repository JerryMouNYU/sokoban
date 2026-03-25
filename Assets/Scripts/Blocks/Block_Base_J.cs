using UnityEngine;
/// <summary>
/// Classic Block where it can't be pushed if something is behind it
/// </summary>
public class Block_Base_J : Block
{
    public override bool CheckMove(int _deltaX, int _deltaY)
    {
        if (State == MoveStates.idle)
        {
            State = MoveStates.attemptingMove;
            if (canMove && InGrid(gridPos.x + _deltaX, gridPos.y + _deltaY))
            {
                Cell checkCell = gridManager.gridList[gridPos.x + _deltaX][gridPos.y + _deltaY].GetComponent<Cell>();
                if (!checkCell.CheckContainObj())
                {
                    StartMove(checkCell, _deltaX, _deltaY);
                    BroadcastMessage("BlockMoved", new Vector2Int(_deltaX, _deltaY), SendMessageOptions.DontRequireReceiver);
                    return true;
                }
            }
            State = MoveStates.idle;
        }
        return false;
    }


    protected bool InGrid(int _newX, int _newY)
    {
        if (_newX >= 0 && _newX < gridManager.gridList.Count &&
            _newY >= 0 && _newY < gridManager.gridList[0].Count) return true;
        return false;
    }
    /// <summary>
    /// Determines if a block that we run into can move
    /// </summary>
    protected bool CheckHit(Block _hitObj, int _deltaX, int _deltaY)
    {
        if (GetComponent<Slidey>() == null && _hitObj.CheckMove(_deltaX, _deltaY)) return true;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
