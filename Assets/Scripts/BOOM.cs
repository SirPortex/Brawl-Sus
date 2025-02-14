using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class BOOM : MonoBehaviourPun
{
    public float currentTime;
    public float maxTime;
    public float explosionDamage;

    public GameObject explosionOwner;

    Bullet bulletFather;

    public void Start()
    {
        //bulletFather = GetComponent<Bullet>();

        //bulletFather.GetComponent<Bullet>().owner = explosionOwner;

    
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime)
        {
            //_rb.useGravity = false;
            currentTime = 0;
            gameObject.SetActive(false); //Se "devuelve" a la pool
            GetComponent<PoolObject>().readyToUse = true;
        }

    }
}
