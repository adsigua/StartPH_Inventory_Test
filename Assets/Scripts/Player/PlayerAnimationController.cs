using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator m_animator;

    public System.Action OnPickupTrigger;

    public void SetMoveAnim(float p_speed) {
        m_animator.SetFloat("speed", p_speed);
    }

    public void SetPickupLayer(float p_moveLayerWeight) {
        m_animator.SetLayerWeight(1, p_moveLayerWeight);
        m_animator.SetLayerWeight(2, 1 - p_moveLayerWeight);
    }

    public void DoPickupAnim() {
        m_animator.SetTrigger("pickup");
    }

    public void OnPickupAnimTrigger() {
        OnPickupTrigger?.Invoke();
    }
}
