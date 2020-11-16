using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface iGun
{
    List<RaycastHit> Shoot();
    void Reload(int ammo);
    bool HasAmmo();

    int AmmoNeeded();
}

public class Pistol : iGun
{
    int magazineSize;
    int damage;
    int ammoType;
    float fireRate;
    int currentAmmo;

    public int MagazineSize { get => magazineSize; }
    public int Damage { get => damage; }
    public int AmmoType { get => ammoType; }
    public float FireRate { get => fireRate; }
    public int CurrentAmmo { get => currentAmmo; }

    public Pistol(int mag, int dmg, int ammo, float rof)
    {
        
    }

    public List<RaycastHit> Shoot()
    {
        List<RaycastHit> list = new List<RaycastHit>();
        return list;
    }

    public void Reload(int ammo)
    {
        currentAmmo += ammo;
    }

    public bool HasAmmo()
    {
        if (currentAmmo > 0)
            return true;
        else
            return false;
    }

    public int AmmoNeeded()
    {
        return magazineSize - currentAmmo;
    }

}