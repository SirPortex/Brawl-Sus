using System.Collections;
using System.Collections.Generic;
using Com.MyCompany.MyGame;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviourPun
{
    public float parabolicSpeed;

    public float speed;
    public float maxTime;

    public float bulletDamage;

    public GameObject owner;
    

    private float currentTime;
    private Vector3 _dir;
    protected Rigidbody _rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody>();

        owner.GetComponent<PlayerMovement>();

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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != owner)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                owner.GetComponent<PlayerMovement>().ulti += 0.1f;
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        AttackBullet();
    }

    public void AttackBullet()
    {
        _rb.velocity = speed * _dir;
    }

    public void SetDirection(Vector3 value)
    {
        _dir = value;
    }
}
