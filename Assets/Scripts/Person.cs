using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [SerializeField] private float bodyFat;
    [SerializeField] private float life;
    [SerializeField] private Transform shop;
    private Rigidbody2D _CompRigidbody2D;

    public float BodyFat
    {
        get
        {
            return BodyFat;
        }
        set
        {
            BodyFat = value;
        }
    }
    public float Life
    {
        get
        {
            return life;
        }
        set
        {
            life = value;
        }
    }

    private void Start()
    {
        
    }
    private void Awake()
    {
        _CompRigidbody2D = GetComponent<Rigidbody2D>();
    }
}
