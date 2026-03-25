using UnityEngine;

public static class GridManagerExtensions
{
    public static bool IsInBounds(this GridManager gridManager, int gridX, int gridY)
    {
        return gridX >= 0 && gridX < gridManager.gridList.Count &&
               gridY >= 0 && gridY < gridManager.gridList[0].Count;
    }

    public static Cell GetCell(this GridManager gridManager, int gridX, int gridY)
    {
        if (!gridManager.IsInBounds(gridX, gridY))
        {
            return null;
        }

        return gridManager.gridList[gridX][gridY].GetComponent<Cell>();
    }

    public static void ClearGridOccupants(this GridManager gridManager)
    {
        for (int x = 0; x < gridManager.gridList.Count; x++)
        {
            for (int y = 0; y < gridManager.gridList[x].Count; y++)
            {
                gridManager.gridList[x][y].GetComponent<Cell>().RemoveContainObj();
            }
        }
    }
}
