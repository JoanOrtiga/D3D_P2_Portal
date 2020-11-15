using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartableObject : MonoBehaviour
{
    protected Vector3 m_InitialPosition;
    protected Quaternion m_InitialRotation;

    protected virtual void Start()
    {
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
    }

    public virtual void RestartObject()
    {
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
    }

    protected virtual void UpdateCheckPoint()
    {
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
    }
}
