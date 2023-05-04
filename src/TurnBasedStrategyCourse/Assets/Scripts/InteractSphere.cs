using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    
    private bool isGreen;
    private GridPosition gridPosition;
    private Action onInteractComplete;
    private bool isActive;
    private float timer;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        SetColorGreen();
    }
    private void Update()
    {
        if (!isActive) return;


        timer -= Time.deltaTime;
        if (timer < 0)
        {
            isActive = false;
            onInteractComplete?.Invoke();
        }
    }

    private void SetColorGreen()
    {
        meshRenderer.sharedMaterial = greenMaterial;
        isGreen = true;
    }
    private void SetColorRed()
    {
        meshRenderer.sharedMaterial = redMaterial;
        isGreen = false;
    }

    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f;

        if (isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }

        onInteractComplete?.Invoke();
    }
}
