using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    private const string IsWalkingAnim = "IsWalking";
    
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;
    protected override void Awake()
    {
        base.Awake();
        this.targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isActive) return;

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        
        float stopingDistance = 0.1f;
        float distanceToTarget = Vector3.Distance(targetPosition, this.transform.position);
        if (distanceToTarget > stopingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            unitAnimator.SetBool(IsWalkingAnim, true);
        }
        else
        {
            unitAnimator.SetBool(IsWalkingAnim, false);
            isActive = false;
            onActionCompleted?.Invoke();
        }

        float rotateSpeed = 10f;
        this.transform.forward = Vector3.Lerp(this.transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        this.onActionCompleted = onActionComplete;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(targetPosition);
        isActive = true;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
