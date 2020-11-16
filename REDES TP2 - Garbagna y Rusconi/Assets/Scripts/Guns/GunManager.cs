using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] iGun thisGun;
    [SerializeField] GunObject gunData;

    internal iGun ThisGun { get => thisGun; set => thisGun = value; }

    private void Awake()
    {
        switch (gunData.GunType)
        {   //Pistol
            case 0:
                thisGun = new Pistol(gunData.MagazineSize, gunData.Damage, gunData.AmmoType, gunData.FireRate);
                break;
        }
    }

    public List<RaycastHit> Shoot()
    {
        return thisGun.Shoot();
    }

    public bool CheckAmmo()
    {
        return thisGun.HasAmmo();
    }
    
    public void Reload()
    {
        //thisGun.Reload();
    }
}
