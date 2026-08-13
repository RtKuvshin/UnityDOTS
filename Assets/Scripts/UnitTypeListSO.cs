using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnitTypeListSO : ScriptableObject
{
    public List<UnitTypeSO> unitTypeListSO;
    
    public UnitTypeSO GetUnitTypeListSO(UnitTypeSO.UnitType _unitType)
    {
        foreach (UnitTypeSO unitTypeSO in unitTypeListSO)
        {
            if (unitTypeSO.unitType == _unitType)
            {
                return unitTypeSO;
            }
        }
        Debug.LogError("Couldn't find UnitTypeSO for UnitType " + _unitType);
        return null;
    }
}
