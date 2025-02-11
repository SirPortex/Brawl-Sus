using System;
using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using Photon.Pun;
using UnityEngine;

public class ShotBullet : MonoBehaviourPun
{
    GameObjectPool bulletPool;
    public float counter;
    public float maxCounter;
    public float transformX, transformY, transformZ;

    public float cooldown;

    public float deadTime;

    PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        bulletPool = GetComponentInChildren<GameObjectPool>();
        playerMovement = GetComponent<PlayerMovement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;

        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && counter >= maxCounter)
        {
            StartCoroutine(AttackDelay());
            

            counter = 0;
        }

        if(playerMovement.health <= 0)
        {
            maxCounter = 10000000;
        
        }

    }

    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(cooldown);

        GameObject obj = bulletPool.GimmeInactiveGameObject();

        if (obj)
        {
            Debug.Log("PIUM !");

            obj.GetComponent<PoolObject>().readyToUse = false;

            obj.transform.position = new Vector3(transform.position.x + transformX, transform.position.y + transformY, transform.position.z + transformZ);
            obj.transform.rotation = transform.rotation;
            obj.GetComponent<Bullet>().SetDirection(transform.forward);

            obj.GetComponent<Bullet>().owner = gameObject;

            obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool
        }

    }

   
}
