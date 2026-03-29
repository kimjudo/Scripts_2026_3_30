using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName ="Scriptable Objects/Item")]
public abstract class Item : ScriptableObject
{

    [Header("Only Game Play")]
    public ItemType itemType;
    public ActionType actionType;

    public GameObject worldPrefab;
    public GameObject handPrefab;

    public float soundRange;
    public float soundLoudness;

    public int animatorID;

    [Header("Only UI")]
    public bool isStackable = true;
    public int maxStackCount;

    [Header("Both")]
    public Sprite image;
    public string itemName;


    public virtual bool Use(GameObject user)
    {
        return false;
    }

}
public enum ItemType
{
    Consumable,
    MainWeapon,
    SecondaryWeapon,
    Throwable,
    Ammo,
    Tool,
}

public enum ActionType
{
    Place,
    Eat,
    Attack,
    Throw,
    None,
    Use,
}



