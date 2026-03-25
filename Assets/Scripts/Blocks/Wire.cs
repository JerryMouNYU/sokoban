using UnityEngine;

public class Wire : Block, IElectronic
{
    [Header("Visuals")]
    [SerializeField]
    private Renderer wireRenderer;

    [SerializeField]
    private Material wireMaterial;

    [SerializeField]
    private Material chargedWireMaterial;

    [SerializeField]
    private bool isCharged;

    public bool IsCharged => isCharged;

    private void Awake()
    {
        if (wireRenderer == null)
        {
            wireRenderer = GetComponentInChildren<Renderer>();
        }
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
        isCharged = charged;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (wireRenderer == null)
        {
            return;
        }

        Material targetMaterial = isCharged ? chargedWireMaterial : wireMaterial;
        if (targetMaterial != null)
        {
            wireRenderer.sharedMaterial = targetMaterial;
        }
    }

}
