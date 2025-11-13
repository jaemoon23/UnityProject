using UnityEngine;
using NovelianMagicLibraryDefense.Managers;
using UnityEngine.InputSystem;

public class TestObj : MonoBehaviour
{
    public GameManager gameManager;

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                gameManager.StageState.SetStageState(StageState.Cleared);
            }
            else if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                gameManager.StageState.SetStageState(StageState.Failed);
            }
        }
    }
#endif
}