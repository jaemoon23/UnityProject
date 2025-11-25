using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private GameObject bookmarkEquipPanel;

    [Header("Close Button")]
    [SerializeField] private Button closeButton;

    [Header("Character Info Tabs")]
    [SerializeField] private Button story1Button;
    [SerializeField] private Button story2Button;
    [SerializeField] private Button story3Button;

    [Header("Bookmark Equip Buttons")]
    [SerializeField] private Button bookmarkSlot1Button;
    [SerializeField] private Button bookmarkSlot2Button;
    [SerializeField] private Button bookmarkSlot3Button;
    [SerializeField] private Button bookmarkSlot4Button;
    [SerializeField] private Button bookmarkSlot5Button;

    [Header("Bookmark Equip Texts")]
    [SerializeField] private TextMeshProUGUI bookmarkSlot1Text;
    [SerializeField] private TextMeshProUGUI bookmarkSlot2Text;
    [SerializeField] private TextMeshProUGUI bookmarkSlot3Text;
    [SerializeField] private TextMeshProUGUI bookmarkSlot4Text;
    [SerializeField] private TextMeshProUGUI bookmarkSlot5Text;

    [Header("Upgrade Button")]
    [SerializeField] private Button upgradeButton;

    [Header("Character Name Text")]
    [SerializeField] private TextMeshProUGUI characterNameText;

    [Header("Character Level Text")]
    [SerializeField] private TextMeshProUGUI characterLevelText;

    [Header("Character EXP Text")]
    [SerializeField] private TextMeshProUGUI characterExpText;

    [Header("Character EXP Slider")]
    [SerializeField] private Slider characterExpSlider;

    [Header("Character Sprite")]
    [SerializeField] private Image characterSprite;
    public int CharacterID { get; private set; }
    private int selectedSlotIndex = 0;


    public void InitInfo(int characterID, int level)
    {
        CharacterID = characterID;
        var characterData = CSVLoader.Instance.GetData<CharacterData>(CharacterID);
        characterNameText.text = $"{characterData.Character_Name}";
        characterLevelText.text = $"Lv.{level}";

        RefreshBookmarkUI();
    }
    /// <summary>
    /// 책갈피 슬롯 버튼 클릭 (Inspector OnClick에서 호출)
    /// </summary>
    public void OnBookmarkSlotClicked(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        Debug.Log($"Slot {slotIndex} selected");
        ShowBookmarkEquipPanel();
    }

    /// <summary>
    /// 현재 선택된 슬롯 인덱스 가져오기
    /// </summary>
    public int GetSelectedSlotIndex()
    {
        return selectedSlotIndex;
    }

    /// <summary>
    /// BookMarkManager에서 장착된 책갈피 정보 가져와서 UI 갱신
    /// </summary>
    private void RefreshBookmarkUI()
    {
        for (int i = 0; i < 5; i++)
        {
            BookMark bookmark = BookMarkManager.Instance.GetCharacterBookmarkAtSlot(CharacterID, i);

            if (bookmark != null)
            {
                UpdateSlotText(i, bookmark.Name);
            }
            else
            {
                UpdateSlotText(i, $"책갈피 슬롯 {i + 1}"); // Display "슬롯 1", "슬롯 2", etc.
            }
        }
    }

    public void StoryButtonClicked()
    {

    }

    public void UpdateSlotText(int slotIndex, string bookmarkName)
    {
        switch (slotIndex)
        {
            case 0:
                bookmarkSlot1Text.text = bookmarkName;
                break;
            case 1:
                bookmarkSlot2Text.text = bookmarkName;
                break;
            case 2:
                bookmarkSlot3Text.text = bookmarkName;
                break;
            case 3:
                bookmarkSlot4Text.text = bookmarkName;
                break;
            case 4:
                bookmarkSlot5Text.text = bookmarkName;
                break;
            default:
                Debug.LogError("Invalid slot index");
                break;
        }
    }

    public void ShowBookmarkEquipPanel()
    {
        bookmarkEquipPanel.SetActive(true);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
