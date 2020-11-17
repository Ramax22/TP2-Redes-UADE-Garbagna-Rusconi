using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] iGun thisGun;
    [SerializeField] GunObject gunData;
    [SerializeField] Animator gunAnimator;

    internal iGun ThisGun { get => thisGun; set => thisGun = value; }

    private void Awake()
    {
        switch (gunData.GunType)
        {   //Pistol
            case 0:
                thisGun = new Pistol(gunData.MagazineSize, gunData.Damage, gunData.AmmoType, gunData.FireRate);
                break;
        }

        gunAnimator = gameObject.GetComponent<Animator>();
    }

    public List<RaycastHit> Shoot()
    {
        return thisGun.Shoot(gunAnimator);
    }

    public bool CheckAmmo()
    {
        return thisGun.HasAmmo();
    }
    
    public void Reload(int ammo)
    {
        thisGun.Reload(ammo, gunAnimator);
    }

    public bool CanShoot()
    {
        return thisGun.CanShoot(gunAnimator);
    }

    public int AmmoNeeded()
    {
        return thisGun.AmmoNeeded();
    }

    public bool CanReload()
    {
        return thisGun.CanReload(gunAnimator);
    }
}
