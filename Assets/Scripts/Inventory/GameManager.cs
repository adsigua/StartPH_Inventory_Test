using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private ItemsMasterList m_itemsMasterList;
    public ItemData GetItemData(int p_itemId) => m_itemsMasterList.GetItemData(p_itemId);

    public Color GetRarityColor(ItemData.RarityType p_rarityType) => m_itemsMasterList.rarityColors[(int)p_rarityType];
    public ItemData GetRandomItemWeigthed() => m_itemsMasterList.GetRandomItemWeighted();

    public void GetRandomItemIndex() {

    }

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
    }


    public void QuitGame() {
        Application.Quit();
    }


}
