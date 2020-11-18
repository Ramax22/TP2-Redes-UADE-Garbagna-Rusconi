using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnsManager : MonoBehaviour
{
    public static SpawnsManager Instance;

    /*[SerializeField] List<GameObject> teamOneSpawns = new List<GameObject>();
    [SerializeField] List<GameObject> teamTwoSpawns = new List<GameObject>();
    [SerializeField] int counterOne = 0;
    [SerializeField] int counterTwo = 0;*/
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] int counter = 0;



    private void Awake()
    {
        if(Instance==null)
        Instance = this;
    }


    /*public List<GameObject> TeamOneSpawns { get => teamOneSpawns; set => teamOneSpawns = value; }
    public List<GameObject> TeamTwoSpawns { get => teamTwoSpawns; set => teamTwoSpawns = value; }
    public int CounterOne { get => counterOne; set => counterOne = value; }
    public int CounterTwo { get => counterTwo; set => counterTwo = value; }*/

    public List<GameObject> SpawnPoints { get => spawnPoints; set => spawnPoints = value; }
    public int Counter { get => counter; set => counter = value; }
}
