using UnityEngine;
using UnityEngine.UI;

public class BookMarkTest : MonoBehaviour
{
    [SerializeField] private Button test;
    [SerializeField] private Button add;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        test.onClick.AddListener(OnClick);
        add.onClick.AddListener(OnAddClick);
    }

    private void OnAddClick()
    {
        // 모든 재료 추가
        IngredientManager.Instance.AddIngredient(1011, 100); // 희미한 마력의 종이
        IngredientManager.Instance.AddIngredient(1012, 100); // 응축된 마력의 종이
        IngredientManager.Instance.AddIngredient(1013, 100); // 비범한 마력의 종이
        IngredientManager.Instance.AddIngredient(1014, 100); // 신성한 마력의 종이
        IngredientManager.Instance.AddIngredient(1015, 100); // 고대 마력의 종이
        IngredientManager.Instance.AddIngredient(1016, 100); // 잉크
        IngredientManager.Instance.AddIngredient(10114, 100); // 룬석

        CurrencyManager.Instance.AddGold(500000);
        Debug.Log("모든 재료 100개씩 추가됨, 보유 골드: " + CurrencyManager.Instance.Gold);
    }
    private void OnClick()
    {
        Debug.Log("BookMarkTest Clicked");

        BookMarkCraftResult result = BookMarkCraft.CraftBookmark(121);
        
        if (result.IsSuccess)
        {
            Debug.Log($"제작 성공! {result.Message}");
            Debug.Log($"북마크: {result.CraftedBookmark.Name}");
        }
        else
        {
            Debug.LogError($"제작 실패! {result.Message}");
        }
    }
}
