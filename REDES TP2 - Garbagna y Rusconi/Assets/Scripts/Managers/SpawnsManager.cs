using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnsManager : MonoBehaviour
{
    public static SpawnsManager Instance;

    [SerializeField] List<GameObject> teamOneSpawns = new List<GameObject>();
    [SerializeField] List<GameObject> teamTwoSpawns = new List<GameObject>();
    [SerializeField] int counterOne = 0;
    [SerializeField] int counterTwo = 0;

    private void Awake()
    {
        if(Instance==null)
        Instance = this;
    }

    public List<GameObject> TeamOneSpawns { get => teamOneSpawns; set => teamOneSpawns = value; }
    public List<GameObject> TeamTwoSpawns { get => teamTwoSpawns; set => teamTwoSpawns = value; }
    public int CounterOne { get => counterOne; set => counterOne = value; }
    public int CounterTwo { get => counterTwo; set => counterTwo = value; }
}
