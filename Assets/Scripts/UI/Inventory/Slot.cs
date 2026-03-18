using UnityEngine;
using System;

public class Slot : MonoBehaviour
{
    private GameObject _currentItem;

    public event Action<Slot, GameObject, GameObject> OnItemChanged;
    public GameObject currentItem
    {
        get => _currentItem;
        set
        {
            if (_currentItem != value)
            {
                GameObject oldItem = _currentItem;
                GameObject newItem = value;

                _currentItem = value;

                OnItemChanged?.Invoke(this, oldItem, newItem);
            }
        }
    }
}