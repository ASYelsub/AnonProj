using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AutoSlicer : MonoBehaviour
{
    [SerializeField] private MeshRenderer objectToSlice;
    private float _forceAppliedToCut = 3f;

    [SerializeField] private int numSlices = 3;

    private List<GameObject> _objectsToSliceCurrentFrame = new List<GameObject>();
    private List<GameObject> _objectsToSliceNextFrame = new List<GameObject>();

    private float _boundsMagnitude;

    public void Slice(MeshRenderer obj)
    {
        objectToSlice = obj;
        
        _objectsToSliceCurrentFrame.Add(objectToSlice.gameObject);
        _boundsMagnitude = objectToSlice.bounds.max.magnitude;
        StartCoroutine(SliceCO());
    }

    private IEnumerator SliceCO()
    {
        for (int i = 0; i < numSlices; i++)
        {
            _objectsToSliceNextFrame.Clear();
            foreach (var obj in _objectsToSliceCurrentFrame)
            {
                CalculateObjectSlicePositions(obj);
            }
            yield return new WaitForFixedUpdate();
            _objectsToSliceCurrentFrame.Clear();
            _objectsToSliceCurrentFrame.AddRange(_objectsToSliceNextFrame);
        }

        foreach (var obj in _objectsToSliceNextFrame)
        {
            obj.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere.normalized * _forceAppliedToCut, ForceMode.Impulse);
        }
    }

    private void CalculateObjectSlicePositions(GameObject obj)
    {
        Vector3 pos1 = Random.insideUnitSphere.normalized * _boundsMagnitude;
        Vector3 pos2 = Random.insideUnitSphere.normalized * _boundsMagnitude;
        Vector3 pos3 = -pos1;
        //Vector3 pos3 = Random.insideUnitSphere.normalized * _boundsMagnitude;
        
        SliceObject(obj, pos1, pos2, pos3);
    }

    private void SliceObject(GameObject obj, Vector3 pos1, Vector3 pos2, Vector3 pos3)
    {
        //Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = pos3 - pos1;
        Vector3 side2 = pos3 - pos2;

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(obj.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = obj.transform.InverseTransformPoint(pos1);

        Plane plane = new Plane();

        plane.SetNormalAndPosition(
            transformedNormal,
            transformedStartingPoint);

        var direction = Vector3.Dot(Vector3.up, transformedNormal);

        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
        {
            plane = plane.flipped;
        }

        GameObject[] slices = Slicer.Slice(plane, obj);
        //_objectsToSliceCurrentFrame.Remove(obj);
        _objectsToSliceNextFrame.AddRange(slices);
        Destroy(obj);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * _forceAppliedToCut;
        //rigidbody.AddForce(newNormal, ForceMode.Impulse);
    }
}
