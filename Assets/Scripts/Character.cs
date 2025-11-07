using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Targeting")]
    [SerializeField] private Transform target;

    [Header("Character Attributes")]
    [SerializeField] private float attackSpeed = 1.0f;
    [SerializeField] private float damage = 10.0f;

    [SerializeField] private float timer = 0.0f;
    [SerializeField] private float attackInterval = 1.0f;

    private void Update()
    {
        
    }
}
