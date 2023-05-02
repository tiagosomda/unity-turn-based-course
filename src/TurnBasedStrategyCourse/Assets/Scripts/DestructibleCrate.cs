using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition() { return gridPosition; }

    public void Damage()
    {
        OnAnyDestroyed.Invoke(this, EventArgs.Empty);
        Destroy(this.gameObject);
    }
}
