using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    //Delegado que funca como evento
    public delegate void OnDeath();

    //Variables
    float _hp;
    float _maxHP;
    OnDeath _onEventDeath;

    //Constructor
    public Health(float initialhealth, OnDeath eventoMuerte)
    {
        _hp = initialhealth;
        _maxHP = initialhealth;
        _onEventDeath = eventoMuerte;
    }

    //Funcion para modificar vida, si le pasas un entero positivo, resta, si le pasas un entero negativo, suma
    public void ChangeLife(float modifier)
    {
        _hp -= modifier;
        _hp = Mathf.Clamp(_hp, 0, _maxHP);

        if (_hp == 0) _onEventDeath.Invoke();
    }

    public void MaxLife()
    {
        _hp = _maxHP;
    }

    //un getter de la var _hp
    public float HP
    {
        get { return _hp; }
    }
}
