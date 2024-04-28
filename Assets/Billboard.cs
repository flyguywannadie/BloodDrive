using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform upcompare;
    [SerializeField] Camera camtolookat;

    // Start is called before the first frame update
    void Start()
    {
        if (!camtolookat)
        {
            camtolookat = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (upcompare)
        {
			transform.LookAt(camtolookat.transform, upcompare.up);
            //transform.rotation = Quaternion.LookRotation(-upcompare.forward);
		} else
        {
			transform.rotation = Quaternion.LookRotation(-camtolookat.transform.forward);
		}
    }
}
