using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface iGun
{
    List<RaycastHit> Shoot(Animator gunAnimator);
    void Reload(int ammo, Animator gunAnimator);
    bool HasAmmo();

    int AmmoNeeded();

    bool CanShoot(Animator gunAnimator);
    bool CanReload(Animator gunAnimator);
    int AmmoAmount();
}

public class Pistol : iGun
{
    int magazineSize;
    int damage;
    int ammoType;
    float fireRate;
    int currentAmmo;
    float count =0;

    public int MagazineSize { get => magazineSize; }
    public int Damage { get => damage; }
    public int AmmoType { get => ammoType; }
    public float FireRate { get => fireRate; }
    public int CurrentAmmo { get => currentAmmo; }

    void Update()
    {
        if(count != 0)
        {
            count -= Time.deltaTime;
        }
    }

    public Pistol(int mag, int dmg, int ammo, float rof)
    {
        magazineSize = mag;
        damage = dmg;
        ammoType = ammo;
        fireRate = rof;
    }

    public List<RaycastHit> Shoot(Animator gunAnimator) //FALTA LOGICA DEL TIRO
    {
        gunAnimator.SetTrigger("Shoot");

        List<RaycastHit> list = new List<RaycastHit>();
        //HAGA ACA LA LOGICA DEL DISPARO Y QUE LA GUARDE EN LA LISTA, HICE LA LISTA POR SI HACEMOS LA SHOTGUN MAS ADELANTE PORQUE TIENE QUE DEVOLVER VARIOS PUNTOS DE GOLPE
        currentAmmo--;
        count = fireRate;
        return list;
    }

    public void Reload(int ammo, Animator gunAnimator)
    {
        currentAmmo += ammo;
        gunAnimator.SetTrigger("Reload");
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

    public bool CanShoot(Animator gunAnimator)
    {
        if ((count <= 0) && (currentAmmo > 0) && (gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Reload") == false))
            return true;
        else
            return false;
    }

    public bool CanReload(Animator gunAnimator)
    {
        if ((currentAmmo < magazineSize) && (gunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") == true))
            return true;
        else
            return false;
    }

    public int AmmoAmount()
    {
        return currentAmmo;
    }
}