using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;

    private Vector3 targetPosition;
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - this.transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(this.transform.position, targetPosition);
        float moveSpeed = 200f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(this.transform.position, targetPosition);
        if (distanceBeforeMoving < distanceAfterMoving)
        {
            trailRenderer.transform.position = targetPosition;
            trailRenderer.transform.parent = null; // the trail is set to auto destruct
            Destroy(gameObject);

            Instantiate(bulletHitVfxPrefab, targetPosition, Quaternion.identity); // vfx will destroy itself
        }
    }
}
