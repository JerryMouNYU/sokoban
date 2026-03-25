using UnityEngine;

public class Door : Block, IElectronic
{
    [SerializeField]
    private Collider[] blockingColliders;

    [SerializeField]
    private GameObject closedVisual;

    [SerializeField]
    private GameObject openVisual;

    [SerializeField]
    private bool isOpen;

    public bool IsCharged => false;

    protected override void Start()
    {
        base.Start();
        UpdateDoorState();
    }

    protected override void GridChanged()
    {
    }

    private void ElectricalStateChanged()
    {
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        bool shouldOpen = HasChargedNeighbor();
        GridManager currentGridManager = GetComponentInParent<GridManager>();
        Cell currentCell = currentGridManager != null ? currentGridManager.GetCell(gridPos.x, gridPos.y) : null;
        bool occupiedByOtherBlock = currentCell != null && currentCell.ContainObj != null && currentCell.ContainObj != gameObject;

        isOpen = shouldOpen || occupiedByOtherBlock;

        if (isOpen)
        {
            if (currentCell != null && currentCell.ContainObj == gameObject)
            {
                currentCell.RemoveContainObj();
            }
        }
        else if (currentCell != null && currentCell.ContainObj == null)
        {
            currentCell.ContainObj = gameObject;
        }

        for (int i = 0; i < blockingColliders.Length; i++)
        {
            if (blockingColliders[i] != null)
            {
                blockingColliders[i].enabled = !isOpen;
            }
        }

        if (closedVisual != null)
        {
            closedVisual.SetActive(!isOpen);
        }

        if (openVisual != null)
        {
            openVisual.SetActive(isOpen);
        }
    }

    private bool HasChargedNeighbor()
    {
        for (int i = 0; i < BlockRuntimeExtensions.OrthogonalDirections.Length; i++)
        {
            if (!this.TryGetAdjacentBlock(BlockRuntimeExtensions.OrthogonalDirections[i], out Block adjacentBlock))
            {
                continue;
            }

            if (adjacentBlock.TryGetComponent(out IElectronic adjacentElectronic) && adjacentElectronic.IsCharged)
            {
                return true;
            }
        }

        return false;
    }
}
