using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    [SerializeField] private LayerMask obstacleLayerMask;
    private int maxShootDistance = 7;
    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    private void Update()
    {
        if (!isActive) return;


        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                float rotateSpeed = 10f;
                unit.transform.forward = Vector3.Lerp(unit.transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if(canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = .5f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float coolOffStateTime = .5f;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }
    public override string GetActionName()
    {
        return "Shoot";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if(testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // not unit present to shoot
                    continue;
                }

                // HasAnyUnitOnGridPosition already ensured we won't get a null from GetUnitAtGridPosition
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Both units are on the same 'team'
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if(Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDirection,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstacleLayerMask))
                {
                    // blocked by an obstacle
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs()
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        OnAnyShoot?.Invoke(this, new OnShootEventArgs()
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        targetUnit.Damage(40);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100)
        };
    }
}
