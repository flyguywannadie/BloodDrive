using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] float timetodie = 2;

    // Update is called once per frame
    void Update()
    {
        timetodie -= Time.deltaTime;
        if (timetodie < 0)
        {
            Destroy(gameObject);
        }
    }
}
