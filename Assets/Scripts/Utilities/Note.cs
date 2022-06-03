using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("A component for taking notes :)")]
    [TextArea(10, 100)]
    [SerializeField] private string note;
    
    void Awake()
    {
        // Prevents it from taking up resources
        enabled = false;
    }
}