using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameObjectPool : MonoBehaviour
{
    [Tooltip("Object that will go to the pool")]
    public GameObject objectToPool; //Objecto (en este caso tuberias) que añadimos a la pool
    [Tooltip("Initial pool size")]
    public uint poolSize; //unit = "unisigned int"
    [Tooltip("If true, size increments")] 
    public bool shouldExpand = false; //Opcion de expandir la lista, por defecto viene falso, es lo mejor

    private List<GameObject> _pool; //lista de GameObjects

    private void Awake()
    {
        _pool = new List<GameObject>(); //instanciamos la lista

        for (int i = 0; i < poolSize; i++) //Instancia X objectos al inicio
        {
            AddGameObjectToPool();
        }
    }

    public GameObject GimmeInactiveGameObject()
    {
        foreach (GameObject obj in _pool)
        {
            if (!obj.activeSelf) //Si el objecto NO esta activado (desatcivado)
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
        GameObject clone = Instantiate(objectToPool);
        clone.SetActive(false); // desactivamos el objecto para que no se utilice de primeras, consume menos recuros asi
        _pool.Add(clone); // lo guardamos en la lista

        return clone;
    }
}
