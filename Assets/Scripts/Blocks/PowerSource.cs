using UnityEngine;

public class PowerSource : Block, IElectronic
{
    public bool IsCharged => true;

    protected override void GridChanged()
    {
    }
}
