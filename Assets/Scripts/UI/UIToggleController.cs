using UnityEngine;
using UnityEngine.UI;

namespace NovelianMagicLibraryDefense.UI
{
    /// <summary>
    /// Controls UI object visibility via button click
    /// Toggles or sets active state of target GameObjects
    /// Can pause/resume game when toggling UI
    /// </summary>
    public class UIToggleController : MonoBehaviour
    {
        [Header("Target Objects")]
        [SerializeField] private GameObject[] targetObjects;

        [Header("Toggle Settings")]
        [SerializeField] private bool toggleMode = false;
        [SerializeField] private bool deactivateOthers = false;
        [SerializeField] private bool pauseGameOnActivate = false;
        [SerializeField] private bool resumeGameOnDeactivate = false;

        [Header("Optional: Other Objects to Deactivate")]
        [SerializeField] private GameObject[] objectsToDeactivate;

        /// <summary>
        /// Activate target objects (call this from Button's OnClick)
        /// </summary>
        public void ActivateTargets()
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                Debug.LogWarning("[UIToggleController] No target objects assigned!");
                return;
            }

            // Deactivate others first if enabled
            if (deactivateOthers && objectsToDeactivate != null)
            {
                foreach (var obj in objectsToDeactivate)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }

            // Activate or toggle targets
            foreach (var target in targetObjects)
            {
                if (target != null)
                {
                    if (toggleMode)
                    {
                        target.SetActive(!target.activeSelf);
                    }
                    else
                    {
                        target.SetActive(true);
                    }
                }
            }

            // Pause game if enabled
            if (pauseGameOnActivate)
            {
                Time.timeScale = 0f;
                Debug.Log("[UIToggleController] Game paused (Time.timeScale = 0)");
            }
        }

        /// <summary>
        /// Deactivate target objects (call this from Button's OnClick)
        /// </summary>
        public void DeactivateTargets()
        {
            if (targetObjects == null || targetObjects.Length == 0) return;

            foreach (var target in targetObjects)
            {
                if (target != null)
                {
                    target.SetActive(false);
                }
            }

            // Resume game if enabled
            if (resumeGameOnDeactivate)
            {
                Time.timeScale = 1f;
                Debug.Log("[UIToggleController] Game resumed (Time.timeScale = 1)");
            }
        }

        /// <summary>
        /// Toggle target objects on/off (call this from Button's OnClick)
        /// </summary>
        public void ToggleTargets()
        {
            if (targetObjects == null || targetObjects.Length == 0) return;

            bool willActivate = false;

            // Check if any target will be activated
            foreach (var target in targetObjects)
            {
                if (target != null && !target.activeSelf)
                {
                    willActivate = true;
                    break;
                }
            }

            // Toggle targets
            foreach (var target in targetObjects)
            {
                if (target != null)
                {
                    target.SetActive(!target.activeSelf);
                }
            }

            // Handle pause/resume
            if (willActivate && pauseGameOnActivate)
            {
                Time.timeScale = 0f;
            }
            else if (!willActivate && resumeGameOnDeactivate)
            {
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// Set active state for specific target by index
        /// </summary>
        public void SetTargetActive(int index, bool active)
        {
            if (targetObjects == null || index < 0 || index >= targetObjects.Length) return;

            if (targetObjects[index] != null)
            {
                targetObjects[index].SetActive(active);

                if (active && pauseGameOnActivate)
                {
                    Time.timeScale = 0f;
                }
                else if (!active && resumeGameOnDeactivate)
                {
                    Time.timeScale = 1f;
                }
            }
        }
    }
}
