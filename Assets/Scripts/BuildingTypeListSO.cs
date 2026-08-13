using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingTypeListSO : ScriptableObject
{
    public List<BuildingTypeSO> buildingTypeSOList;
    
    public BuildingTypeSO GetBuildingTypeSO(BuildingTypeSO.BuildingType _buildingType)
    {
        foreach (BuildingTypeSO buildingTypeSO in buildingTypeSOList)
        {
            if (buildingTypeSO.buildingType == _buildingType)
            {
                return buildingTypeSO;
            }
        }
        Debug.LogError("Couldn't find BuildingTypeSO for BuildingType " + _buildingType);
        return null;
    }
}
