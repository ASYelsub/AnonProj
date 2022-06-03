using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectGrabber : CalledOnScenesLoadedBase
{

    [SerializeField] private GameObject testObjectRetrievedByName;
    [SerializeField] private Rigidbody testObjectRetrievedByComponent;

    protected override void OnScenesLoaded(NEvent e)
    {
        Debug.Log("Called after all scenes loaded :)");
        testObjectRetrievedByName = GameObject.Find("ObjectToRetrieve");
        testObjectRetrievedByComponent = FindObjectOfType<Rigidbody>();
    }
}
