using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Events : MonoBehaviour
{
    public UnityEvent<int, float> events;

    // Start is called before the first frame update
    void Start()
    {
        events.AddListener(exploision);
        events.Invoke(12, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exploision(int ent, float fl)
    {
        Debug.Log("explosion" + ent + fl); ;
    }

}
