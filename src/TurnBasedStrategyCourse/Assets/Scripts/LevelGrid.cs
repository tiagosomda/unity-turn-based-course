using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnAnyUnitMovedGridPosition; 

    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    private GridSystem<GridObject> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"More than one {nameof(LevelGrid)}. {this.gameObject.name} and {Instance.gameObject.name}");
            Destroy(this.gameObject);
            return;
        }

        Instance = this;


        gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        Pathfinding.Instance.Setup(width, height, cellSize);
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMoveGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition) 
        => gridSystem.GetGridPosition(worldPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) 
        => gridSystem.IsValidGridPosition(gridPosition);

    public Vector3 GetWorldPosition(GridPosition gridPosition)
        => gridSystem.GetWorldPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }


    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable door)
    {
        var gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetInteractable(door);
    }
}
