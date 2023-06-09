using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLaywerMask;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one {nameof(Pathfinding)}. {this.gameObject.name} and {Instance.gameObject.name}");
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    
    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(
            width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if(Physics.Raycast(
                    worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up,
                    raycastOffsetDistance * 2,
                    obstacleLaywerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }


    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);

        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                // Reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                if(closedList.Contains(neighborNode))
                {
                    continue;
                }

                if(!neighborNode.IsWalkable())
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = 
                    currentNode.GetGCost() 
                    + CalculateDistance(
                        currentNode.GetGridPosition(),
                        neighborNode.GetGridPosition());

                if(tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if(!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        // No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition a, GridPosition b)
    {

        //Vector3 aWorldPosition = LevelGrid.Instance.GetWorldPosition(a);
        //Vector3 bWorldPosition = LevelGrid.Instance.GetWorldPosition(b);

        //Vector3 shootDirection = (aWorldPosition - bWorldPosition).normalized;
        //float unitShoulderHeight = 1.7f;
        //if (Physics.Raycast(
        //    aWorldPosition + Vector3.up * unitShoulderHeight,
        //    shootDirection,
        //    Vector3.Distance(aWorldPosition, bWorldPosition),
        //    obstacleLaywerMask))
        //{
        //    // blocked by an obstacle
        //    return int.MaxValue;
        //}


        GridPosition gridPositionDistance = a - b;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance)
             + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }
    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if(gridPosition.x - 1 >= 0)
        {
            // Left Node
            neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));

            if (gridPosition.z - 1 >= 0)
            {
                // Left Down Node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
            }

            if(gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Left Up Node
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
            }
        }

        if (gridPosition.x + 1 < gridSystem.GetWidth())
        {
            // Right Node
            neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                // Right Down Node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
            }

            if(gridPosition.z + 1 < gridSystem.GetHeight())
            {
                // Right Up Node
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
            }
        }


        if (gridPosition.z - 1 >= 0)
        {
            // Down Node
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        }

        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            // Up Node
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        }

        return neighborList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach(PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int _) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
