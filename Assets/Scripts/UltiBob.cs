using Com.MyCompany.MyGame;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UltiBob : MonoBehaviourPun
{
    public PlayerMovement Bob;

    public GameObjectPool ultiPool;

    // Start is called before the first frame update
    void Start()
    {
        
        //ultiPool = GameObject.Find("UltiPool").GetComponentInChildren<GameObjectPool>();
        //ultiPool = GameObjectPool.FindObjectOfType<GameObjectPool>();

        Bob = FindObjectOfType<PlayerMovement>();
     
        //Bob = FindAnyObjectByType<PlayerMovement>();
        Bob.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Bob.ultimateReady && Input.GetKeyDown(KeyCode.Q))
        {
            Bob.animator.SetBool("IsUlting", true);
            Bob.ulti = 0;
            Bob.ultimateReady = false;
            StartCoroutine(UltiBobRoutine());
        }
    }
   
    IEnumerator UltiBobRoutine()
    {
        Bob.animator.SetBool("IsUlting", true);
        Bob.ulti = 0;
        Bob.ultimateReady = false;

        yield return new WaitForSeconds(0.1f);

        for(int i = 0; i < 5; i++)
        {
            GameObject obj = ultiPool.GimmeInactiveGameObject();
            if (obj)
            {
                obj.GetComponent<PoolObject>().readyToUse = false;

                obj.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                obj.transform.rotation = transform.rotation;
                obj.GetComponent<Bullet>().SetDirection(transform.forward);

                obj.GetComponent<Bullet>().owner = gameObject;

                obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool
            }
            yield return new WaitForSeconds(0.1f);
        }
        //GameObject obj = ultiPool.GimmeInactiveGameObject();
        //if (obj)
        //{
        //    obj.GetComponent<PoolObject>().readyToUse = false;

        //    obj.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        //    obj.transform.rotation = transform.rotation;
        //    obj.GetComponent<Bullet>().SetDirection(transform.forward);

        //    obj.GetComponent<Bullet>().owner = gameObject;

        //    obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool
        //}
        //yield return new WaitForSeconds(0.1f);





        yield return new WaitForSeconds(Bob.ultiDuration);
        Bob.animator.SetBool("IsUlting", false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
