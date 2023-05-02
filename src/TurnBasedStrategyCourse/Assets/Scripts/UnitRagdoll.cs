using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRagdollRootBone)
    {
        MatchAllChildTransforms(originalRagdollRootBone, ragdollRootBone);

        Vector3 randomDir = new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f));
        ApplyExplosionToRagdoll(ragdollRootBone, 300f, this.transform.position + randomDir, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach(Transform child in root)
        {
            Transform childClone = clone.Find(child.name);
            if(childClone != null)
            {
                childClone.position = child.position;
                childClone.rotation = child.rotation;

                MatchAllChildTransforms(child, childClone);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform originalRoot, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in originalRoot)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
