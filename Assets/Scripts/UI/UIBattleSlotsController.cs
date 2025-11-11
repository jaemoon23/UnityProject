using System.Collections.Generic;
using UnityEngine;

public class UIBattleSlotsController : MonoBehaviour
{
    //CBL
    [Header("Parents (Existing objects)")]
    [SerializeField] private Transform playerSlotParent; // PlayerSlot
    [SerializeField] private Transform skillSlotParent;

    [Header("Hidden Pool Root (Disabled Instance)")]
    [SerializeField] private Transform hiddenPoolRoot;

    [Header("Prefabs")]
    [SerializeField] private PlayerSlotItem playerSlotPrefab;
    [SerializeField] private SkillSlotItem skillSlotPrefab;

    [Header("Counts")]
    [SerializeField] private int playerCount = 10; // 5x2
    [SerializeField] private int skillCount = 4;

    private UIPool<PlayerSlotItem> playerPool;
    private UIPool<SkillSlotItem> skillPool;

    private readonly List<PlayerSlotItem> activePlayers = new();
    private readonly List<SkillSlotItem> activeSkills = new();

    private void Awake()
    {
        // Grid Safety Device
        ValidateGrids();

        playerPool = new UIPool<PlayerSlotItem>(playerSlotPrefab, playerCount, hiddenPoolRoot);
        skillPool = new UIPool<SkillSlotItem>(skillSlotPrefab, skillCount, hiddenPoolRoot);

        // All old slots placed in the editor are hidden
        ClearChildren(playerSlotParent);
        ClearChildren(skillSlotParent);
    }

    // ===== Called from the External/Inspector button =====
    [ContextMenu("Show Slots")]
    public void ShowSlots()
    {
        // playerslot 5x2
        for (int i = 0; i < playerCount; i++)
        {
            var slot = playerPool.Get(playerSlotParent);
            slot.Init(i); //icon/text setting
            activePlayers.Add(slot);
        }

        // 4 skills 
        for (int i = 0; i < skillCount; i++)
        {
            var slot = skillPool.Get(skillSlotParent);
            slot.Init(i);
            activeSkills.Add(slot);
        }

        // Update<-Prevent warping
        Canvas.ForceUpdateCanvases();
    }

    [ContextMenu("Hide Slots")]
    public void HideSlots()
    {
        playerPool.ReleaseAll(activePlayers);
        skillPool.ReleaseAll(activeSkills);
    }

    // ===== Helpers =====
    private void ValidateGrids()
    {
        // PlayerSlotParent is Fixed to GridLayoutGroup 5

        var playerGrid = playerSlotParent.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        if (playerGrid != null)
        {
            playerGrid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
            playerGrid.constraintCount = 5; // 5x2
        }

    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var t = parent.GetChild(i);
            t.gameObject.SetActive(false);
            t.SetParent(hiddenPoolRoot, false);
        }
    }
}

