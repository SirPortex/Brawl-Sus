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

                obj.GetComponent<PoolObject>().readyToUse = false;

                obj.transform.position = new Vector3(0f, 2f, 0f);
                obj.GetComponent<Bullet>().SetDirection(transform.forward);
                obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool
            }
        }
    }
}
