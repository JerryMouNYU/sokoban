using UnityEngine;

public class Door : Block_Base_J, IElectronic
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip openClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float openVolume = 0.85f;

    [SerializeField]
    private float openPitch = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float openSpatialBlend = 0f;

    [SerializeField]
    private Collider[] blockingColliders;

    [SerializeField]
    private GameObject closedVisual;

    [SerializeField]
    private GameObject openVisual;

    [SerializeField]
    private bool isOpen;

    public bool IsCharged => false;

    [SerializeField]
    private bool blockVerticalWhenOpen = false;

    [SerializeField]
    private bool blockHorizontalWhenOpen = false;

    private AudioSource openAudioSource;

    protected override void Start()
    {
        base.Start();
        UpdateDoorState();
    }

    protected override void GridChanged()
    {
    }

    public override bool CheckMove(int _deltaX, int _deltaY)
    {
        if (AllowsEntry(_deltaX, _deltaY))
        {
            return true;
        }

        return base.CheckMove(_deltaX, _deltaY);
    }

    private void ElectricalStateChanged()
    {
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        bool wasOpen = isOpen;
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

        if (!wasOpen && isOpen)
        {
            PlayOpenSound();
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

    protected override bool CanEnterOccupiedCell(Block hitObj, int deltaX, int deltaY)
    {
        if (hitObj != this)
        {
            return base.CanEnterOccupiedCell(hitObj, deltaX, deltaY);
        }

        return AllowsEntry(deltaX, deltaY);
    }

    private bool AllowsEntry(int deltaX, int deltaY)
    {
        if (!isOpen)
        {
            return false;
        }

        bool movingHorizontally = deltaX != 0;
        bool movingVertically = deltaY != 0;

        if (movingHorizontally && !blockHorizontalWhenOpen)
        {
            return true;
        }

        if (movingVertically && !blockVerticalWhenOpen)
        {
            return true;
        }

        return false;
    }

    private void PlayOpenSound()
    {
        if (openClip == null)
        {
            return;
        }

        if (openAudioSource == null)
        {
            openAudioSource = GetComponent<AudioSource>();
            if (openAudioSource == null)
            {
                openAudioSource = gameObject.AddComponent<AudioSource>();
            }

            openAudioSource.playOnAwake = false;
            openAudioSource.loop = false;
        }

        if (openAudioSource.isPlaying)
        {
            openAudioSource.Stop();
        }

        openAudioSource.clip = openClip;
        openAudioSource.volume = openVolume;
        openAudioSource.spatialBlend = openSpatialBlend;
        openAudioSource.pitch = openPitch;
        openAudioSource.Play();
    }
}
