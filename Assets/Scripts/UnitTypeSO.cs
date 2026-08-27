using System;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu]
public class UnitTypeSO : ScriptableObject
{
    public enum UnitType 
    {
        None,
        Soldier,
        Scout,
        Zombie
    }

    public UnitType unitType;
    public float progressMax;
    public Sprite sprite;

    public Entity GetPrefabEntity(EntitiesReference entitiesReference)
    {
        switch (unitType)
        {
            default:
            case UnitType.None: 
            case UnitType.Soldier: return entitiesReference.soldierPrefabEntity;
            case UnitType.Scout: return entitiesReference.scoutPrefabEntity;
            case UnitType.Zombie: return entitiesReference.zombiePrefabEntity;
                
        }
    }
}
