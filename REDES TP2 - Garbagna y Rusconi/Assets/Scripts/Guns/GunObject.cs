using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun Information/New Gun Info")]
public class GunObject : ScriptableObject
{
    [SerializeField] int magazineSize;
    [SerializeField] int damage;
    [SerializeField] int ammoType;
    [SerializeField] float fireRate;
    [SerializeField] int gunType;

    public int MagazineSize { get => magazineSize; }
    public int Damage { get => damage; }
    public int AmmoType { get => ammoType; }
    public float FireRate { get => fireRate; }
    public int GunType { get => gunType; }

}
