using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerSlotItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameLabel;

    public void Init(int index)
    {
        if (nameLabel) nameLabel.text = $"P{index + 1}";
        
    }
   
    }
public class SkillSlotItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI keyLabel;

    public void Init(int index)
    {
        if (keyLabel) keyLabel.text = $"S{index + 1}";
       
    }
}