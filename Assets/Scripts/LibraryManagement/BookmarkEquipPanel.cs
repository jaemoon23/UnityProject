using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookmarkEquipPanel : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotContainer;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject InfoPanel;
    private CharacterInfoPanel characterInfoPanel;
    
    [SerializeField] BookMarkManager bmManager;


    private void Awake()
    {
        characterInfoPanel = InfoPanel.GetComponent<CharacterInfoPanel>();
    }
    private void OnEnable()
    {
        List<BookMark> unequippedBookmarks = BookMarkManager.Instance.GetAllBookmarks();
        for (int i = 0; i < unequippedBookmarks.Count; i++)
        {
            if (!unequippedBookmarks[i].IsEquipped)
            {
                GameObject slot = Instantiate(slotPrefab, slotContainer);
                BookmarkEquipSlot slotComponent = slot.GetComponent<BookmarkEquipSlot>();
                slotComponent.Init(unequippedBookmarks[i], characterInfoPanel);
            }
        }
    }

    private void OnDisable()
    {
        // 패널이 비활성화될 때 슬롯 정리 (재활성화 시 다시 생성됨)
        if (slotContainer != null)
        {
            foreach (Transform child in slotContainer)
            {
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
        InfoPanel.SetActive(true);
    }
}
