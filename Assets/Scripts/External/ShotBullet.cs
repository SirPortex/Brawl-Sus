using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBullet : MonoBehaviour
{
    GameObjectPool bulletPool;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = bulletPool.GimmeInactiveGameObject();

            if (obj)
            {
                obj.SetActive(true); //quitar el boli del estuche, ya no esta disponible en la poool
                obj.transform.position = transform.position;
                obj.GetComponent<Bullet>().SetDirection(transform.forward);
            }
        }
    }
}
