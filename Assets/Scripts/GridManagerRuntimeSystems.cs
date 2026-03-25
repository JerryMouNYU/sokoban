using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GridManager))]
public class GridManagerRuntimeSystems : MonoBehaviour
{
    private GridManager gridManager;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeOnLoad()
    {
        EnsurePresent();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static void EnsurePresent()
    {
        GridManager manager = Object.FindFirstObjectByType<GridManager>();
        if (manager == null || manager.GetComponent<GridManagerRuntimeSystems>() != null)
        {
            return;
        }

        manager.gameObject.AddComponent<GridManagerRuntimeSystems>();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsurePresent();
    }

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        gridManager.UpdateGrid();
    }

    private void GridChanged()
    {
        RebuildElectricalNetwork();
        BroadcastMessage("ElectricalStateChanged", SendMessageOptions.DontRequireReceiver);
    }

    private void RebuildElectricalNetwork()
    {
        Wire[] wires = Object.FindObjectsByType<Wire>(FindObjectsSortMode.None);
        for (int i = 0; i < wires.Length; i++)
        {
            wires[i].SetCharged(false);
        }

        PowerSource[] powerSources = Object.FindObjectsByType<PowerSource>(FindObjectsSortMode.None);
        Queue<Block> propagationQueue = new Queue<Block>();

        for (int i = 0; i < powerSources.Length; i++)
        {
            propagationQueue.Enqueue(powerSources[i]);
        }

        while (propagationQueue.Count > 0)
        {
            Block currentBlock = propagationQueue.Dequeue();

            for (int i = 0; i < BlockRuntimeExtensions.OrthogonalDirections.Length; i++)
            {
                Vector2Int direction = BlockRuntimeExtensions.OrthogonalDirections[i];
                Cell nextCell = gridManager.GetCell(currentBlock.gridPos.x + direction.x, currentBlock.gridPos.y + direction.y);
                if (nextCell == null || nextCell.ContainObj == null)
                {
                    continue;
                }

                if (!nextCell.ContainObj.TryGetComponent(out Wire adjacentWire) || adjacentWire.IsCharged)
                {
                    continue;
                }

                adjacentWire.SetCharged(true);
                propagationQueue.Enqueue(adjacentWire);
            }
        }
    }
}
