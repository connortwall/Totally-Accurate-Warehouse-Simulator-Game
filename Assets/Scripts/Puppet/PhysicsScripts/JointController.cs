using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JointController : MonoBehaviour
{
    private List<Rigidbody> _rbs = new List<Rigidbody>();

    [SerializeField] private float mass = 10.0f;
    [SerializeField] private float drag = 6.0f;
    
    // Start is called before the first frame update
    void Awake()
    {
        _rbs = GetComponentsInChildren<Rigidbody>().ToList();
        _rbs.ForEach(rb =>
        {
            rb.mass = mass;
            rb.drag = drag;
            // TODO: SET VAR(positionForce, rb.gameObject, generalForce)
        });
    }
}
