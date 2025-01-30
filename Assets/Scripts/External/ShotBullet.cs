using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShotBullet : MonoBehaviourPun
{
    GameObjectPool bulletPool;


    // Start is called before the first frame update
    void Start()
    {
        bulletPool = GetComponentInChildren<GameObjectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = bulletPool.GimmeInactiveGameObject();

            if (obj)
            {
                Debug.Log("PIUM !");
                obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool

                obj.GetComponent<Bullet>().readyToUse = false;

                obj.transform.position = transform.position;
                obj.GetComponent<Bullet>().SetDirection(transform.forward);
            }
        }
    }
}
