using UnityEngine;

public class PowerSource : Block_Base_J, IElectronic
{
    public bool IsCharged => true;

    protected override void GridChanged()
    {
    }
}
