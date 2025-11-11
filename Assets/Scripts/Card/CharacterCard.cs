using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 카드 클릭 처리
/// </summary>
[RequireComponent(typeof(Button))]
public class CharacterCard : MonoBehaviour
{
    public Image characterImage; // 캐릭터 이미지
    public TextMeshProUGUI characterNameText; // 캐릭터 이름 텍스트
    public GenreType genreType;  // 현재 장르

    private Button button;
    private CardSelectionManager manager;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Start()
    {
        manager = FindFirstObjectByType<CardSelectionManager>();

        if (characterImage == null)
        {
            characterImage = GetComponentInChildren<Image>();
        }
    }

    /// <summary>
    /// 캐릭터 정보 업데이트
    /// </summary>
    public void UpdateCharacter(Sprite sprite, string characterName, GenreType genre)
    {
        if (characterImage != null)
        {
            characterImage.sprite = sprite;
        }

        if (characterNameText != null)
        {
            characterNameText.text = characterName;
        }

        genreType = genre;
    }

    void OnClick()
    {
        if (manager != null)
        {
            manager.OnCardSelected();
        }
    }
}
