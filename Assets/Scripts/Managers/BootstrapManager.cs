using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NovelianMagicLibraryDefense.Managers
{
    /// <summary>
    /// LMJ: Bootstrap manager for initial scene loading
    /// Loads ManagerScene, unloads itself, then loads initial gameplay scene
    /// Uses UniTask for async scene operations (no coroutines)
    /// </summary>
    public class BootstrapManager : MonoBehaviour
    {
        [Header("Initial Scene Settings")]
        [SerializeField] private string managerSceneName = "ManagerScene";
        [SerializeField] private string initialGameplaySceneName = "SampleScene";

        async void Start()
        {
            Debug.Log("[Bootstrap] Starting game initialization...");
            await InitializeGame();
        }

        /// <summary>
        /// LMJ: Initialize game by loading necessary scenes
        /// 1. Load ManagerScene (Additive)
        /// 2. Set ManagerScene as active
        /// 3. Unload BootScene (self)
        /// 4. Load initial gameplay scene
        /// </summary>
        private async UniTask InitializeGame()
        {
            try
            {
                // 1. Load ManagerScene (Additive)
                Debug.Log($"[Bootstrap] Loading {managerSceneName}...");
                await SceneManager.LoadSceneAsync(managerSceneName, LoadSceneMode.Additive).ToUniTask();

                // 2. Set ManagerScene as active scene
                Scene managerScene = SceneManager.GetSceneByName(managerSceneName);
                if (managerScene.IsValid())
                {
                    SceneManager.SetActiveScene(managerScene);
                    Debug.Log($"[Bootstrap] {managerSceneName} set as active scene");
                }
                else
                {
                    Debug.LogError($"[Bootstrap] Failed to find {managerSceneName}!");
                    return;
                }

                // 3. Unload BootScene (self)
                Scene bootScene = SceneManager.GetSceneByName("BootScene");
                if (bootScene.IsValid())
                {
                    Debug.Log("[Bootstrap] Unloading BootScene...");
                    await SceneManager.UnloadSceneAsync(bootScene).ToUniTask();
                }

                // 4. Load initial gameplay scene
                Debug.Log($"[Bootstrap] Loading {initialGameplaySceneName}...");
                await SceneManager.LoadSceneAsync(initialGameplaySceneName, LoadSceneMode.Additive).ToUniTask();

                Debug.Log("[Bootstrap] Game initialization complete!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Bootstrap] Initialization failed: {e.Message}");
            }
        }
    }
}
