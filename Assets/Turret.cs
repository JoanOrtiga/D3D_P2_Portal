using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private RaycastHit ray;
    public float rayLength;
    private bool isGrabbed;
    private Rigidbody grabbedObject;
    public GameObject shootingPoint;
    private LineRenderer laser;
    private void Start()
    {
        isGrabbed = false;
        laser = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        Vector3 fwd = shootingPoint.transform.forward;
        Debug.DrawRay(shootingPoint.transform.position, fwd * rayLength, Color.green);
        laser.SetPosition(0, shootingPoint.transform.position);
        laser.SetPosition(1, fwd * rayLength + shootingPoint.transform.position);
    }
}
