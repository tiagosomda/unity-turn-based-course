using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExplode;
    [SerializeField] private Transform grenadeExplosionVfxPrefab;
    [SerializeField] private Transform trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private Action OnGrenadeBehaviorComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;

        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;

        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;

        this.transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = .2f;
        if(Vector3.Distance(positionXZ, targetPosition)  < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(60);
                }

                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructableCrate))
                {
                    destructableCrate.Damage();
                }
            }

            OnAnyGrenadeExplode?.Invoke(this, EventArgs.Empty);

            trailRenderer.parent.parent = null; // enabled auto destroy on particle effect

            Instantiate(grenadeExplosionVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);


            Destroy(this.gameObject);

            OnGrenadeBehaviorComplete?.Invoke();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviorComplete)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        OnGrenadeBehaviorComplete = onGrenadeBehaviorComplete;

        positionXZ = this.transform.position;
        positionXZ.y = 0f;

        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
