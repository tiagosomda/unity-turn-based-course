using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
    }

    private void DestructibleCrate_OnAnyDestroyed(object sender, System.EventArgs e)
    {
        DestructibleCrate destructableCrate = (DestructibleCrate) sender;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), isWalkable: true);
    }
}
