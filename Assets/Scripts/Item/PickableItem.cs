using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [SerializeField] private int m_itemID;

    [SerializeField] private Transform m_iconTransform;
    [SerializeField] private GameObject m_arrowObject;
    [SerializeField] private Renderer m_quadRenderer;

    private MaterialPropertyBlock m_matPropBlock;

    [SerializeField] private float m_bobAnimHeight = 1.0f;
    [SerializeField] private float m_bobAnimDuration = 1.0f;

    public System.Action<PickableItem> OnItemPickup;

    public int itemID => m_itemID;
    private bool m_isInitialized = false;

    private void Awake() {
        m_matPropBlock = new MaterialPropertyBlock();
        m_quadRenderer.GetPropertyBlock(m_matPropBlock);
    }

    public void SpawnItem(int p_id, Texture p_tex, Vector3 p_targetPos, float p_maxHeight = 0.7f, float p_duration = 0.8f) {
        m_isInitialized = true;
        m_itemID = p_id;
        SetQuadTexture(p_tex);
        DoSpawnAnim(p_targetPos, p_maxHeight, p_duration);
    }

    public void SetQuadTexture(Texture p_tex) {
        m_matPropBlock.SetTexture("_MainTex", p_tex);
        m_quadRenderer.SetPropertyBlock(m_matPropBlock);
    }

    private void Start() {
        if(!m_isInitialized) {
            m_isInitialized = true;
            SetQuadTexture(GameManager.instance.GetItemData(m_itemID).itemTexture);
            StartBobAnim();
        }
    }

    private void DoSpawnAnim(Vector3 p_targetPos, float p_maxHeight = 0.7f, float p_duration = 0.8f) {
        Vector3 startPos = transform.position;
        LeanTween.value(gameObject, (val) => {
            transform.position = new Vector3(
                Mathf.Lerp(startPos.x, p_targetPos.x, val),
                Mathf.Lerp(startPos.y, p_maxHeight, Mathf.Asin(Mathf.Sin(val * Mathf.PI)) * (2 / Mathf.PI)),
                Mathf.Lerp(startPos.z, p_targetPos.z, val)
            );
        }, 0.0f, 1.0f, p_duration).setOnComplete(()=>StartBobAnim());
    }

    private void StartBobAnim() {
        LeanTween.moveLocalY(m_iconTransform.gameObject, m_bobAnimHeight, m_bobAnimDuration).setLoopPingPong();
    }

    private void OnTriggerEnter(Collider p_col) {
        if (p_col.CompareTag("PlayerTrigger")) {
            ToggleArrow(true);
        }
    }

    private void OnTriggerExit(Collider p_col) {
        if (p_col.CompareTag("PlayerTrigger")) {
            ToggleArrow(false);
        }
    }


    public void ToggleArrow(bool p_isActive) {
        m_arrowObject.SetActive(p_isActive);
    }

    public void PickupItem() {
        LeanTween.cancel(gameObject);
        OnItemPickup?.Invoke(this);
    }

}
