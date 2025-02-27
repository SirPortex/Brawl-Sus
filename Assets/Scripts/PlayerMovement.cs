
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pavel;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.Asteroids;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;

namespace Com.MyCompany.MyGame
{
    public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
    {
        private IEnumerator coroutine;

        public FixedJoystick joystick;

        [SerializeField] private Volume damageEffect;

        public bool potentialWinner = false;
        public bool ultimateReady = false;

        public float walkingSpeed, rotationSpeed, aceleration, sphereRadius;//,gravityScale;


        [Tooltip("The current Health of our player")]
        public float health = 1f;

        [Tooltip("The Player's UI GameObject Prefab")]

        public GameObject PlayerUiPrefab;
        public GameObject HealingEffect;
        public GameObject UltiUiPrefab;

        public float maxHealth, regenTime, regenMaxTime, healingValue, ulti, maxUlti, ultiDuration, ultiIncrement;

        private Image ultiImage;

        public float counter;
        public float maxCounter;

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
            damageEffect.weight = 0;
            
            ultimateReady = false;
            ulti = 0;
            //joystick.gameObject.SetActive(false);

#if UNITY_ANDROID //PARA QUE FUNCIONE EN ANDROID
            joystick = FindObjectOfType<FixedJoystick>();
#endif


            if (photonView.IsMine)
            {
                PlayerMovement.LocalPlayerInstance = this.gameObject;
                UltiUiPrefab.SetActive(true);
            }

            else
            {
                UltiUiPrefab.SetActive(false);
            }

            DontDestroyOnLoad(this.gameObject);
            SceneManager.activeSceneChanged += OnSceneLoaded;
        }

        public void PlusUlti()
        {
            ulti += ultiIncrement;
        }

        void OnSceneLoaded(Scene scene, Scene mode)
        {
            if(this != null)
            {
                GetComponentInChildren<GameObjectPool>().DelayInstanitateObjects();
            }
        }
        [PunRPC]
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
                return;

            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet && bullet.owner != gameObject)
            {
                //ulti += 0.1f;

                //bullet.owner.GetComponent<PlayerMovement>().ulti += 0.1f;

                //ulti = bullet.owner.GetComponent<PlayerMovement>().ulti += 0.1f;
              
                health = Mathf.Clamp(health, 0, maxHealth);

                health -= bullet.bulletDamage;

                regenTime = 0;

                if(coroutine != null)
                {
                    HealingEffect.SetActive(false);
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
                Debug.Log("Corrutina Parada");
            }

            BOOM boom = other.GetComponent<BOOM>();

            if (boom && boom.explosionOwner != gameObject)
            {
                health = Mathf.Clamp(health, 0, maxHealth);

                health -= boom.explosionDamage;

                regenTime = 0;
                
                if(coroutine != null)
                {
                    HealingEffect.SetActive(false);
                    StopCoroutine(coroutine);
                    coroutine = null;
                }
                Debug.Log("Corrutina Parada");

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
            //coroutine = Healing();

            health = Mathf.Clamp(health, 0, maxHealth);

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

            if(UltiUiPrefab != null)
            {
                GameObject _ultiGo = Instantiate(UltiUiPrefab);
                _ultiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> UltiUiPrefab reference on player Prefab.", this);
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
                //ulti += 0.1f;
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
                regenTime += Time.deltaTime;

                if(regenTime >= regenMaxTime)
                {
                    coroutine = Healing();
                    regenTime = 0;

                    StartCoroutine(coroutine);

                    HealingEffect.SetActive(true);

                    Debug.Log("Corrutina Empezada");
                }
            }
            else if(health >= maxHealth)
            {
                regenTime = 0;
                if(coroutine != null)
                {
                    StopCoroutine(coroutine);

                    HealingEffect.SetActive(false);
                    coroutine = null;
                    Debug.Log("Corrutina Parada");
                }
            }

            if(ulti >= 1)
            {
                Debug.Log("Ulti Ready");
            }

            if(health <= maxHealth / 2)
            {
                //damageEffect.weight = 1;
                for(int i = 0; i < 10; i++)
                {
                    damageEffect.weight++;
                    
                }
            }
            else
            {
                damageEffect.weight = 0;
            }
        }

        [PunRPC]
        IEnumerator Healing()
        {
            regenTime = 0;

            health = Mathf.Clamp(health, 0, maxHealth);

            for(float i = 0; i < maxHealth; i += healingValue)
            {
                i = health;
                health += healingValue;
                yield return new WaitForSeconds(0.5f);
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
#if UNITY_EDITOR_WIN || UNITY_STANDALONE //PARA QUE FUNCIONE EN PC

            ApplySpeed();


#endif

#if UNITY_ANDROID //PARA QUE FUNCIONE EN ANDROID
            joystick.gameObject.SetActive(true);

            if (photonView.IsMine)
            {

                rb.velocity = new Vector3(joystick.Horizontal * walkingSpeed, rb.velocity.y, joystick.Vertical * walkingSpeed);

                if (joystick.Horizontal != 0 || joystick.Vertical != 0)
                {
                    //Debug.Log("Estoy moviendome");
                    transform.rotation = Quaternion.LookRotation(rb.velocity);
                }
            }
#endif



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

            GameObject _ultiGo = Instantiate(this.UltiUiPrefab);
            _ultiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

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


