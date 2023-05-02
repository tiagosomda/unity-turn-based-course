using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathfindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;
    [SerializeField] private SpriteRenderer isWalkableSprite;
    [SerializeField] private Color walkableColor;
    [SerializeField] private Color notWalkableColor;

    private PathNode pathNode;
    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = gridObject as PathNode;
    }

    protected override void Update()
    {
        base.Update();
        gCostText.text = $"g: {pathNode.GetGCost()}";
        hCostText.text = $"h: {pathNode.GetHCost()}";
        fCostText.text = $"f: {pathNode.GetFCost()}";
        isWalkableSprite.color = pathNode.IsWalkable() ? walkableColor : notWalkableColor;
    }
}
