using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;
    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.T))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition startGridPosition = new GridPosition(0, 0);

            var path = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition);
            Debug.Log(path);

            for(int i = 0; i < path.Count-1; i++)
            {
                Debug.DrawLine(
                    LevelGrid.Instance.GetWorldPosition(path[i]),
                    LevelGrid.Instance.GetWorldPosition(path[i+1]),
                    Color.cyan,
                    10f);
            }
        }
    }
}
