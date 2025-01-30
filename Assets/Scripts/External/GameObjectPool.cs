using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameObjectPool : MonoBehaviourPunCallbacks
{
    [Tooltip("Object that will go to the pool")]
    public GameObject objectToPool; //Objecto (en este caso tuberias) que añadimos a la pool
    [Tooltip("Initial pool size")]
    public uint poolSize; //unit = "unisigned int"
    [Tooltip("If true, size increments")] 
    public bool shouldExpand = false; //Opcion de expandir la lista, por defecto viene falso, es lo mejor

    string currentScene;

    private List<GameObject> _pool; //lista de GameObjects

    private void Awake()
    {
        _pool = new List<GameObject>(); //instanciamos la lista

        //SceneManager.activeSceneChanged += OnSceneLoaded;

        currentScene = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        //if(currentScene != SceneManager.GetActiveScene().name)
        //{
        //    StartCoroutine(DelayInstanitateObjects());
        //}
    }


    public GameObject GimmeInactiveGameObject()
    {
        foreach (GameObject obj in _pool)
        {
            if (obj.GetComponent<Bullet>().readyToUse) //Si el objecto NO esta activado (desatcivado) SI FUNCIONA GENERELAIZAR (DIEGO)
            {
                return obj; //Si el objetco no es activo lo damos
            }
        }

        if (shouldExpand) //Si deberia de expandirse la pool se instancia un nuevo objecto
        {
            return AddGameObjectToPool();
        }

        return null; //Si no encontramos nada devolvemos NULL, osea nada.
    }

    private GameObject AddGameObjectToPool() //añadir GameObject a la pool
    {
        GameObject clone = PhotonNetwork.Instantiate(this.objectToPool.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        if (clone)
        {
            clone.SetActive(false); // desactivamos el objecto para que no se utilice de primeras, consume menos recuros asi
            _pool.Add(clone); // lo guardamos en la lista
        }

        return clone;
    }


    //void OnSceneLoaded(Scene scene, Scene mode)
    //{
    //    StartCoroutine(DelayInstanitateObjects());
    //}

    public void DelayInstanitateObjects()
    {
        Debug.Log("JJJJJJJJJJJJJJJJ");
        //yield return new WaitForSeconds(0.6f);

        _pool.Clear();
        for (int i = 0; i < poolSize; i++) //Instancia X objectos al inicio
        {
            AddGameObjectToPool();
        }
    }
    
}
