using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShotBullet : MonoBehaviourPun
{
    GameObjectPool bulletPool;
    float counter = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        bulletPool = GetComponentInChildren<GameObjectPool>();
        
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;

        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && counter >= 0.5f)
        {
            GameObject obj = bulletPool.GimmeInactiveGameObject();

            if (obj)
            {
                Debug.Log("PIUM !");

                obj.GetComponent<PoolObject>().readyToUse = false;

                obj.transform.position = new Vector3(transform.position.x , transform.position.y + 1f, transform.position.z);
                obj.transform.rotation = transform.rotation;
                obj.GetComponent<Bullet>().SetDirection(transform.forward);
                obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool
            }

            counter = 0;
        }

    }

   
}
