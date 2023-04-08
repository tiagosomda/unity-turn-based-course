using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void Show(Material materila)
    {
        meshRenderer.enabled = true;
        meshRenderer.material = materila;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
