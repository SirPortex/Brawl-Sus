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
