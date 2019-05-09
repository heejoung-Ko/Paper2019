#define _WITH_SMOOTH_ROTATION
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform target = null;
    private Vector3 initialPosition;

    public float speed = 1.0f;
    public float distanceTo = 1.2f;

    void Start()
    {
        target = GameObject.Find("Target").transform;
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        print("ResetPosition");
    }

    void Update()
    {
#if _WITH_SMOOTH_ROTATION
        Vector3 look = transform.forward;
        Vector3 toTarget = (target.position - transform.position).normalized;
        float fDotProduct = Vector3.Dot(look, toTarget);
        Vector3 crossProduct = Vector3.Cross(look, toTarget);
        float fAngle = Mathf.Approximately(fDotProduct, 1.0f) ? 0.0f : ((fDotProduct > 0.0f) ? (Mathf.Acos(fDotProduct) * Mathf.Rad2Deg) : 90.0f);
        bool bAligned = Mathf.Abs(fAngle) < 0.5f;
        if (!bAligned)
            transform.Rotate(0.0f, fAngle * (Time.deltaTime * 0.5f) * ((crossProduct.y > 0.0f) ? 1.0f : -1.0f), 0.0f);
        else
            transform.LookAt(target);
        Debug.DrawLine(transform.position, target.position, (bAligned) ? new Color(1.0f, 0.0f, 1.0f) : Color.yellow);
#else
        transform.LookAt(target);
        Vector3 look = transform.forward;
#endif
        if (Vector3.Distance(target.position, transform.position) > distanceTo)
            transform.position += look * speed * Time.deltaTime;    // speed * Time.deltaTime: 한 프레임에 가야하는 거리
        //else
        //    target.gameObject.SendMessage("Arrived", gameObject);
    }

    void LateUpdate()
    {
        Vector3 right = transform.TransformDirection(Vector3.right) * 1.5f;
        Debug.DrawLine(transform.position, transform.position + right, Color.red);
        Vector3 up = transform.TransformDirection(Vector3.up) * 1.5f;
        Debug.DrawLine(transform.position, transform.position + right, Color.green);
        Vector3 look = transform.TransformDirection(Vector3.forward) * 1.5f;
        Debug.DrawLine(transform.position, transform.position + right, Color.blue);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 right = transform.TransformDirection(Vector3.right) * 1.5f;
    }
}
