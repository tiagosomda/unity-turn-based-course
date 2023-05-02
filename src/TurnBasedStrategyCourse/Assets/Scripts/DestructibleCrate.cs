using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;

    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition() { return gridPosition; }

    public void Damage()
    {
        Transform createDestroyedTransform = Instantiate(crateDestroyedPrefab, this.transform.position, this.transform.rotation);

        ApplyExplosionToChildren(createDestroyedTransform, 150f, this.transform.position, 10f);

        OnAnyDestroyed.Invoke(this, EventArgs.Empty);
        Destroy(this.gameObject);
    }

    private void ApplyExplosionToChildren(Transform originalRoot, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in originalRoot)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
