
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pavel;
using UnityEngine.SceneManagement;

namespace Com.MyCompany.MyGame
{
    public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float walkingSpeed, rotationSpeed, aceleration, sphereRadius;//,gravityScale;


        [Tooltip("The current Health of our player")]
        public float Health = 0.5f;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        public float counter = 1;

        public float cooldown;


        public string groundName;

        public Animator animator;

        private Vector3 movementVector;

        private Rigidbody rb;
        private float x, z, mouseX; //input

        private float currentSpeed;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerMovement.LocalPlayerInstance = this.gameObject;
            }

            DontDestroyOnLoad(this.gameObject);
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, Scene mode)
        {
            if(this != null)
            {

            GetComponentInChildren<GameObjectPool>().DelayInstanitateObjects();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }


            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }



            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();

            //gravityScale = -Mathf.Abs(gravityScale); //Valor Absoluto

        }

        // Update is called once per frame
        void Update()
        {
            counter += Time.deltaTime;

            if (!photonView.IsMine && PhotonNetwork.IsConnected)
            {
                return;
            }

            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
            mouseX = Input.GetAxisRaw("Mouse X");

            if (Input.GetMouseButtonDown(0) && counter >= 0.5f)
            {

                Attack();
                counter = 0;
            }

            InterpolationSpeed();

            RotatePlayer();
        }

        public void Attack()
        {
            StartCoroutine(AttackAnim());
        }

        IEnumerator AttackAnim()
        {
            Debug.Log("Estoy atacando");
            animator.SetBool("IsAttacking", true);
            //animator.Play("Attack");
            yield return new WaitForSeconds(cooldown);
            animator.SetBool("IsAttacking", false);

        }
        public void InterpolationSpeed()
        {

            if (x != 0 || z != 0)
            {
                //animator.SetBool("IsAttacking", false);
                currentSpeed = Mathf.Lerp(currentSpeed, walkingSpeed, aceleration * Time.deltaTime); // velocidad de caminar
            }
            else
            {
                //animator.SetBool("IsAttacking", false);
                currentSpeed = Mathf.Lerp(currentSpeed, 0, aceleration * Time.deltaTime); // velocidad de idle
            }
        }

        void RotatePlayer()
        {
            Vector3 rotation = new Vector3(0, mouseX, 0) * rotationSpeed * Time.deltaTime;
            transform.Rotate(rotation); // Se aplica la rotacion, tiene numero imaginarios
        }
        private void FixedUpdate()
        {
            ApplySpeed();


        }

        void ApplySpeed()
        {
            rb.velocity = (transform.forward * currentSpeed * z) + (transform.right * currentSpeed * x) +
                new Vector3(0, rb.velocity.y, 0); //Gravedad base de Unity.
                                                  //+ (transform.up * gravityScale); //Gravedad constante no realista
                                                  //rb.AddForce(transform.up * gravityScale);
        }

        private bool IsGrounded()
        {
            Collider[] colliders = Physics.OverlapSphere(new Vector3(transform.position.x,
                transform.position.y - transform.localScale.y / 20, transform.position.z), sphereRadius);

            for (int i = 0; i < colliders.Length; i++) //recorremos elemento a elemento.
            {
                // y comprobamos si el elemento es suelo o no.
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer(groundName)) //Recorre cada elemento del array para ver si tocamos suelo
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos() //Raycast de la esfera.
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x,
                transform.position.y - transform.localScale.y / 20, transform.position.z), sphereRadius);
        }

        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //if (stream.IsWriting)
            //{
            //    stream.SendNext(gameObject.activeSelf);
            //}
            //else
            //{
            //    gameObject.SetActive((bool)stream.ReceiveNext());
            //}
        }
    }
}


