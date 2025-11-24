using UnityEngine;
using UnityEngine.UI;
using NovelianMagicLibraryDefense.Core;
using Cysharp.Threading.Tasks;

namespace NovelianMagicLibraryDefense.UI
{
    /// <summary>
    /// Controls UI interactions in the Lobby scene
    /// Handles scene transitions from Lobby to Game
    /// </summary>
    public class LobbyUIController : MonoBehaviour
    {
        [Header("Button References")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button bookMarkCraftButton;


        public void OnBookMarkButton()
        {
            LoadBookMarkCraftSceneAsync().Forget();
        }

        public void OnGameStartButton()
        {
            LoadGameSceneAsync().Forget();
        }

        public void OnInventoryButton()
        {
            LoadInventorySceneAsync().Forget();
        }

        public void OnLibraryManagementButton()
        {
            LibraryManagementSceneAsync().Forget();
        }
        public async UniTaskVoid LoadGameSceneAsync()
        {
            await FadeController.Instance.LoadSceneWithFade("GameScene");
        }

        public async UniTaskVoid LoadInventorySceneAsync()
        {
            await FadeController.Instance.LoadSceneWithFade("Inventory");
        }

        public async UniTaskVoid LoadBookMarkCraftSceneAsync()
        {
            await FadeController.Instance.LoadSceneWithFade("BookMarkCraftScene");
        }

        public async UniTaskVoid LibraryManagementSceneAsync()
        {
            await FadeController.Instance.LoadSceneWithFade("LibraryManagementScene");
        }
    }
}