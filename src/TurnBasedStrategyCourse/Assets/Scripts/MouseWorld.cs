using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld singleton;

    [SerializeField]
    private LayerMask mousePlaneLayerMask;

    private Vector3 cachedMousePosition;

    private void Awake()
    {
        singleton = this;    
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, singleton.mousePlaneLayerMask);
        cachedMousePosition = raycastHit.point;

        this.transform.position = MouseWorld.GetPosition();
    }

    public static Vector3 GetPosition()
    {
        return singleton.cachedMousePosition;

    }
}
