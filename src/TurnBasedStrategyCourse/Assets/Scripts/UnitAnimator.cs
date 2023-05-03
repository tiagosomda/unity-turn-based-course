using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private const string IsWalkingAnim = "IsWalking";
    private const string ShootAnim = "Shoot";
    private const string SwordSlashAnim = "SwordSlash";

    [SerializeField] private Animator unitAnimator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        if(TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if(TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if(TryGetComponent(out SaberAction saberAction))
        {
            saberAction.OnSwordActionStarted += SaberAction_OnSwordActionStarted;
            saberAction.OnSwordActionCompleted += SaberAction_OnSwordActionCompleted;
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SaberAction_OnSwordActionCompleted(object sender, System.EventArgs e)
    {
        EquipRifle();
    }

    private void SaberAction_OnSwordActionStarted(object sender, System.EventArgs e)
    {
        EquipSword();
        unitAnimator.SetTrigger(SwordSlashAnim);
    }

    private void MoveAction_OnStartMoving(object sender, System.EventArgs e)
    {
        unitAnimator.SetBool(IsWalkingAnim, true);
    }

    private void MoveAction_OnStopMoving(object sender, System.EventArgs e)
    {
        unitAnimator.SetBool(IsWalkingAnim, false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        unitAnimator.SetTrigger(ShootAnim);
        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();

        targetUnitShootAtPosition.y = shootPointTransform.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
}
