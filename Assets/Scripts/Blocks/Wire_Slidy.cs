using UnityEngine;

public class Wire_Slidy : Wire
{
    protected override bool CanEnterOccupiedCell(Block hitObj, int deltaX, int deltaY)
    {
        return CheckHit(hitObj, deltaX, deltaY);
    }

    protected override void FinishMove()
    {
        base.FinishMove();
        Slide(moveChange.x, moveChange.y);
    }

    private void Slide(int deltaX, int deltaY)
    {
        CheckMove(deltaX, deltaY);
    }
}
