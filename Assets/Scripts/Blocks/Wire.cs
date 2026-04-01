using UnityEngine;

public class Wire : Block_Base_J_NoPush, IElectronic
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip chargeUpClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float chargeUpVolume = 0.8f;

    [SerializeField]
    [Range(0.25f, 3f)]
    private float chargeUpPitch = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float chargeUpSpatialBlend = 0f;

    [Header("Visuals")]
    [SerializeField]
    private Visual_Wire visualWire;

    // [SerializeField]
    // private Renderer wireRenderer;

    // [SerializeField]
    // private Material wireMaterial;

    // [SerializeField]
    // private Material chargedWireMaterial;

    [SerializeField]
    private bool isCharged;

    private static AudioSource sharedChargeAudioSource;
    private static int lastChargeSoundFrame = -1;

    public bool IsCharged => isCharged;

    private void Awake()
    {
        if (visualWire == null)
        {
            visualWire = GetComponentInChildren<Visual_Wire>();
        }

        // if (wireRenderer == null)
        // {
        //     wireRenderer = GetComponentInChildren<Renderer>();
        // }
    }

    protected override void Start()
    {
        base.Start();
        SetCharged(false);
    }

    protected override void GridChanged()
    {
        // Charge is resolved centrally by GridManagerRuntimeSystems after the grid updates.
    }

    public void SetCharged(bool charged)
    {
        SetCharged(charged, true);
    }

    public void SetCharged(bool charged, bool playSound)
    {
        bool becameCharged = !isCharged && charged;
        isCharged = charged;
        UpdateVisualState();

        if (playSound && becameCharged)
        {
            PlaySharedChargeSound();
        }
    }

    private void UpdateVisualState()
    {
        bool upConnected = HasVisualConnection(new Vector2Int(0, -1));
        bool downConnected = HasVisualConnection(new Vector2Int(0, 1));
        bool leftConnected = HasVisualConnection(Vector2Int.left);
        bool rightConnected = HasVisualConnection(Vector2Int.right);

        if (visualWire != null)
        {
            visualWire.ApplyState(isCharged, upConnected, downConnected, leftConnected, rightConnected);
        }
        visualWire.ChangeLight(isCharged);
        // if (wireRenderer == null)
        // {
        //     return;
    }


    // Material targetMaterial = isCharged ? chargedWireMaterial : wireMaterial;
    // if (targetMaterial != null)
    // {
    //     wireRenderer.sharedMaterial = targetMaterial;
    // }


    private bool HasVisualConnection(Vector2Int direction)
    {
        if (!this.TryGetAdjacentBlock(direction, out Block adjacentBlock))
        {
            return false;
        }

        return adjacentBlock.TryGetComponent(out IElectronic electronic) || adjacentBlock.TryGetComponent(out WireBox wireBox);
    }

    private void PlaySharedChargeSound()
    {
        if (chargeUpClip == null || lastChargeSoundFrame == Time.frameCount)
        {
            return;
        }

        if (sharedChargeAudioSource == null)
        {
            GameObject audioObject = new GameObject("WireChargeAudioPlayer");
            sharedChargeAudioSource = audioObject.AddComponent<AudioSource>();
            sharedChargeAudioSource.playOnAwake = false;
            sharedChargeAudioSource.loop = false;
            Object.DontDestroyOnLoad(audioObject);
        }

        sharedChargeAudioSource.Stop();
        sharedChargeAudioSource.clip = chargeUpClip;
        sharedChargeAudioSource.volume = chargeUpVolume;
        sharedChargeAudioSource.pitch = chargeUpPitch;
        sharedChargeAudioSource.spatialBlend = chargeUpSpatialBlend;
        sharedChargeAudioSource.Play();
        lastChargeSoundFrame = Time.frameCount;
    }

}
