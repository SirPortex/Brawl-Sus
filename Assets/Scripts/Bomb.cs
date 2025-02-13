using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bomb : Bullet
{

    GameObjectPool explosionPool;

    

    protected override void Start()
    {
        base.Start();
        AttackParabolic();

        explosionPool = GetComponentInChildren<GameObjectPool>();


    }

    private void OnEnable()
    {
        if (_rb)
        {
            _rb.velocity = Vector3.zero;
            AttackParabolic();
        }
    }

    protected override void FixedUpdate()
    {
        
    }

    private void Update()
    {
        
    }

    public void AttackParabolic()
    {
        _rb.useGravity = true;
        //_rb.velocity = Vector3.zero;
        _rb.AddForce(((_rb.transform.forward) + Vector3.up) * parabolicSpeed);

        //_rb.AddRelativeForce(new Vector3(0, parabolicSpeed, 0));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (other.CompareTag("Ground"))
        {
            Debug.Log("BOOOM!");
            GameObject obj = explosionPool.GimmeInactiveGameObject();

            obj.GetComponent<PoolObject>().readyToUse = false;
            obj.SetActive(true);
            obj.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y * 1.5f, transform.position.z);
            obj.transform.rotation = transform.rotation;

            gameObject.GetComponent<PoolObject>().readyToUse = true;
            gameObject.SetActive(false);

            //gameObject.SetActive(false);
            //StartCoroutine(DestroyFireBall());
            //StartCoroutine(ExplosionBoom());


        }
    }
    public IEnumerator ExplosionBoom()
    {
        GameObject obj = explosionPool.GimmeInactiveGameObject();

        obj.GetComponent<PoolObject>().readyToUse = false;
        obj.SetActive(true);
        obj.transform.position = new Vector3(transform.position.x, transform.position.y + transform.localScale.y *1.5f, transform.position.z);
        obj.transform.rotation = transform.rotation;

        yield return new WaitForSeconds(3.3f);

        obj.GetComponent<PoolObject>().readyToUse = true;
        obj.SetActive(false);

    }

    public IEnumerator DestroyFireBall()
    {
        yield return new WaitForSeconds(3.4f);

        gameObject.SetActive(false);

        gameObject.GetComponent<PoolObject>().readyToUse = true;
    }




}
