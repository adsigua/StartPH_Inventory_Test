using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private Transform m_billboardTransform;
    [SerializeField] private Transform m_cameraTransform;

    [SerializeField] private bool m_flipForward = true;

    public void Awake() {
        if(m_cameraTransform == null) {
            m_cameraTransform = Camera.main.transform;
        }
    }

    public void LateUpdate() {
        if(m_cameraTransform!= null) {
            m_billboardTransform.LookAt(m_billboardTransform.position + m_cameraTransform.forward * (m_flipForward ? -1 : 1), m_cameraTransform.transform.up);
        }
    }

}
