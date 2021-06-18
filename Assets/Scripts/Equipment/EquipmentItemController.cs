using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EquipmentItemType
{
    weapon,
    throwableItem
}

public class EquipmentItemController : MonoBehaviour
{
    public EquipmentItemType itemType;
}
