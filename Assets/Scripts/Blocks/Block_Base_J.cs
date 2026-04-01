using UnityEngine;
/// <summary>
/// Classic Block where it can't be pushed if something is behind it
/// </summary>
public class Block_Base_J : Block
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip moveClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float moveVolume = 0.8f;

    [SerializeField]
    [Range(0.25f, 3f)]
    private float movePitch = 1.25f;

    [SerializeField]
    [Range(0f, 1f)]
    private float moveSpatialBlend = 0f;

    private AudioSource moveAudioSource;

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
                else if (checkCell.ContainObj.TryGetComponent(out Block blockInCell) &&
                         CanEnterOccupiedCell(blockInCell, _deltaX, _deltaY))
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

    protected override void StartMove(Cell _newParent, int _deltaX, int _deltaY)
    {
        base.StartMove(_newParent, _deltaX, _deltaY);
        PlayMoveSound();
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

    protected virtual bool CanEnterOccupiedCell(Block hitObj, int deltaX, int deltaY)
    {
        return CheckHit(hitObj, deltaX, deltaY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayMoveSound()
    {
        if (moveClip == null)
        {
            return;
        }

        if (moveAudioSource == null)
        {
            moveAudioSource = GetComponent<AudioSource>();
            if (moveAudioSource == null)
            {
                moveAudioSource = gameObject.AddComponent<AudioSource>();
            }

            moveAudioSource.playOnAwake = false;
            moveAudioSource.loop = false;
            moveAudioSource.volume = 1f;
            moveAudioSource.minDistance = 1f;
            moveAudioSource.maxDistance = 25f;
            moveAudioSource.rolloffMode = AudioRolloffMode.Linear;
        }

        if (moveClip.loadState == AudioDataLoadState.Unloaded)
        {
            moveClip.LoadAudioData();
        }

        moveAudioSource.spatialBlend = moveSpatialBlend;
        moveAudioSource.pitch = movePitch;
        moveAudioSource.PlayOneShot(moveClip, moveVolume);
    }
}
