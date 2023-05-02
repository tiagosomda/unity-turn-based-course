using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotateSpeed = 10f;
        this.transform.forward = Vector3.Lerp(this.transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stopingDistance = 0.1f;
        float distanceToTarget = Vector3.Distance(targetPosition, this.transform.position);
        if (distanceToTarget > stopingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), targetPosition, out int _);
        currentPositionIndex = 0;
        positionList = new List<Vector3>(pathGridPositionList.Count);

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if(testGridPosition == unitGridPosition)
                {
                    continue;
                }

                if(LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if(!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    // we could use Pathfinding.Instance.GetPathLength
                    // and check if the path length is 0
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;
                if(Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    // path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        //int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        //float distanceFromCurrentLocation = Vector3.Distance(unit.GetWorldPosition(), LevelGrid.Instance.GetWorldPosition(gridPosition));

        Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = pathLength ////targetCountAtGridPosition * 10
        };
    }
}
