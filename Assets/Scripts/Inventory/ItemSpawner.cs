using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    [SerializeField] private Transform m_itemContainer;
    [SerializeField] private float m_spawnRadius = 20;
    [SerializeField] private int m_maxSpawnCount = 30;
    [SerializeField] private float m_spawnRate;
    private int m_currentSpawnCount = 0;
    //[SerializeField] private List<PickableItem> m_items;

    private float m_nextSpawnTime = 0;
    [SerializeField] private PickableItem m_itemPrefab;
    [SerializeField] private float m_spawnAnimHeight = 0.8f;
    [SerializeField] private float m_spawnAnimDuration = 0.8f;
    [SerializeField] private float m_dropAnimRadius = 1.0f;

    private void Start() {
        
    }

    private void Update() {
        if(m_currentSpawnCount < m_maxSpawnCount && Time.time > m_nextSpawnTime) {
            m_nextSpawnTime = Time.time + (1 / m_spawnRate);
            Vector2 randPos = Random.insideUnitCircle;
            Vector3 spawnPos = new Vector3(randPos.x, 0, randPos.y) * m_spawnRadius;
            ItemData randomItem = GameManager.instance.GetRandomItemWeigthed();
            SpawnItem(spawnPos, randomItem, spawnPos);
        }
    }

    private void OnEnable() {
        PlayerManager.OnItemDrop += OnItemDrop;
    }

    private void OnDisable() {
        PlayerManager.OnItemDrop -= OnItemDrop;
    }

    private void OnItemDrop(int p_itemId, int p_itemCount, Vector3 p_playerPos) {
        ItemData item = GameManager.instance.GetItemData(p_itemId);
        for (int i=0; i< p_itemCount; i++) {
            Vector2 randPos = Random.insideUnitCircle;
            Vector3 spawnOffset = new Vector3(randPos.x, 0, randPos.y) * m_dropAnimRadius;
            SpawnItem(p_playerPos, item, p_playerPos + spawnOffset);
        }
    }

    public void SpawnItem(Vector3 p_pos, ItemData p_itemData, Vector3 p_targetPos) {
        PickableItem newItem = Instantiate(m_itemPrefab, m_itemContainer);
        newItem.transform.position = p_pos;
        newItem.SpawnItem(p_itemData.itemID, p_itemData.itemTexture, p_targetPos, m_spawnAnimHeight, m_spawnAnimDuration);
        newItem.OnItemPickup += OnItemPickup;
        m_currentSpawnCount++;
        //m_items.Add(newItem);
    }

    private void OnItemPickup(PickableItem item) {
        item.OnItemPickup -= OnItemPickup;
        //if(m_items.Contains(item)) {
        //    m_items.Remove(item);
        //}
        m_currentSpawnCount--;
        Destroy(item.gameObject);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, m_spawnRadius);
    }
#endif


}
