
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
        

        public bool potentialWinner = false;

        public float walkingSpeed, rotationSpeed, aceleration, sphereRadius;//,gravityScale;


        [Tooltip("The current Health of our player")]
        public float health = 1f;

        [Tooltip("The Player's UI GameObject Prefab")]

        public GameObject PlayerUiPrefab;

        public float maxHealth;

        public float counter;
        public float maxCounter;
        public float healthCounter;
        public float healthRegenTime;

        public float cooldown;

        //public float damage;

        public float deadTime;



        public string groundName;

        public Animator animator;

        private Vector3 movementVector;

        private Rigidbody rb;
        private float x, z, mouseX; //input

        private float currentSpeed;

        private Bullet bulletComp;

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

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
                return;

            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet && bullet.owner != gameObject)
            {
                health -= bullet.bulletDamage;
                healthCounter = 0;

            }

            BOOM boom = other.GetComponent<BOOM>();

            if (boom && boom.explosionOwner != gameObject)
            {
                health -= boom.explosionDamage;
                healthCounter = 0;
            }

            if (other.CompareTag("Detector"))
            {
                potentialWinner = true;
            }

            if(other.CompareTag("Win") && potentialWinner)
            {
                GameManager.instance.LeaveRoom();
                Destroy(this.gameObject);
                PhotonNetwork.LoadLevel("Victory");

            }
        }

        // Start is called before the first frame update
        void Start()
        {
            

            healthCounter = 0;

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
            bulletComp = GetComponent<Bullet>();

            //gravityScale = -Mathf.Abs(gravityScale); //Valor Absoluto

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif

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

            if (Input.GetMouseButtonDown(0) && counter >= maxCounter)
            {

                Attack();
                counter = 0;
            }

            InterpolationSpeed();

            RotatePlayer();

            if (photonView.IsMine) // Si la vida llega a 0 nos salimos de la sala
            {
                if(health <= 0)
                {
                    StartCoroutine(Dead());
                }
            }

            if(health < maxHealth)
            {
                healthCounter += Time.deltaTime;
            }

            if(healthCounter >= healthRegenTime)
            {
                healthCounter = 0;
                while (health < maxHealth)
                {
                    health += 0.1f * Time.deltaTime;
                }
            }
        }

   
        IEnumerator Dead()
        {
            walkingSpeed = 0;
            rotationSpeed = 0;
            maxCounter = 10000000;

            animator.SetBool("IsDead", true);
            yield return new WaitForSeconds(deadTime);
            GameManager.instance.LeaveRoom();
            //SceneManager.LoadScene(5);
            Destroy(this.gameObject);
            PhotonNetwork.LoadLevel("Defeat");
            


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
            if (stream.IsWriting)
            {
                stream.SendNext(health);
                stream.SendNext(counter);
            }
            else
            {
                health = (float)stream.ReceiveNext();
                counter = (float)stream.ReceiveNext();
            }
        }
#if !UNITY_5_4_OR_NEWER
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

    }
}


