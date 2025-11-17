using UnityEngine;

//JML: World coordinate-based 2D Sprite grid slot
//     Represents a position where characters can be placed
public class GridSlot : MonoBehaviour
{
    [Header("Slot Info")]
    [SerializeField] private int slotIndex;             // Slot number (0-9)
    [SerializeField] private SpriteRenderer spriteRenderer; // Grid visualization

    [Header("Slot State")]
    private GameObject currentCharacter;  // Currently placed character GameObject
    private bool isOccupied = false;      // Whether slot is occupied

    private void Awake()
    {
        // Auto-setup SpriteRenderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    //JML: Initialize slot (called from CharacterPlacementManager)
    public void Initialize(int index)
    {
        slotIndex = index;
        HideGrid();
    }

    //JML: Show grid
    public void ShowGrid()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }
    }

    //JML: Hide grid
    public void HideGrid()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    //JML: Place character
    public void PlaceCharacter(GameObject character)
    {
        if (character == null) return;

        currentCharacter = character;
        isOccupied = true;

        // Move character to slot position
        character.transform.position = transform.position;

        Debug.Log($"[GridSlot {slotIndex}] Character placed");
    }

    //JML: Remove character
    public void RemoveCharacter()
    {
        currentCharacter = null;
        isOccupied = false;

        Debug.Log($"[GridSlot {slotIndex}] Character removed");
    }

    //JML: Check if slot is empty
    public bool IsEmpty()
    {
        return !isOccupied;
    }

    //JML: Get currently placed character
    public GameObject GetCurrentCharacter()
    {
        return currentCharacter;
    }

    //JML: Get slot index
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    //JML: Get slot's world position
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
