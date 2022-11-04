using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    protected Rigidbody _rb;

    protected void Awake()
    {
        gameObject.tag = "Item";
        _rb = GetComponent<Rigidbody>();
    }
}
