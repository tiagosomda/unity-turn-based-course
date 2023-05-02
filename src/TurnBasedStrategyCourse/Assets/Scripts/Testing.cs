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
            //ScreenShake.Instance.Shake(2f);
            
            
            //GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            //GridPosition startGridPosition = new GridPosition(1, 7);

            //var path = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition, out int pathLength);
            //Debug.Log($"path count: {path.Count}, path length: {pathLength}");

            //for(int i = 0; i < path.Count-1; i++)
            //{
            //    Debug.DrawLine(
            //        LevelGrid.Instance.GetWorldPosition(path[i]),
            //        LevelGrid.Instance.GetWorldPosition(path[i+1]),
            //        Color.cyan,
            //        10f);
            //}
        }
    }
}
