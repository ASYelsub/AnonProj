using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool _isSolid = true;

    [SerializeField]
    private bool _reverseWindTriangles = false;

    [SerializeField]
    private bool _useGravity = false;

    [SerializeField]
    private bool _shareVertices = false;

    [SerializeField]
    private bool _smoothVertices = false;

    public bool IsSolid
    {
        get
        {
            return _isSolid;
        }
        set
        {
            _isSolid = value;
        }
    }

    public bool ReverseWireTriangles
    {
        get
        {
            return _reverseWindTriangles;
        }
        set
        {
            _reverseWindTriangles = value;
        }
    }

    public bool UseGravity 
    {
        get
        {
            return _useGravity;
        }
        set
        {
            _useGravity = value;
        }
    }

    public bool ShareVertices 
    {
        get
        {
            return _shareVertices;
        }
        set
        {
            _shareVertices = value;
        }
    }

    public bool SmoothVertices 
    {
        get
        {
            return _smoothVertices;
        }
        set
        {
            _smoothVertices = value;
        }
    }

    [HideInInspector] public bool slicedAlready = false;

    private void OnCollisionEnter(Collision other)
    {
        if (!slicedAlready)
            Slice();
    }

    public void Slice()
    {
        GameObject autoSlicerGO = new GameObject();
        AutoSlicer autoSlicer = autoSlicerGO.AddComponent<AutoSlicer>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (!mr)
            mr = GetComponentInChildren<MeshRenderer>();
        autoSlicer.Slice(mr);
        Destroy(autoSlicerGO, .2f);
    }

}