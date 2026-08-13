using Unity.Entities;
using UnityEngine;

public class BuildingBarracksAuthoring : MonoBehaviour
{
    public float progressMax;
    
    public class Baker: Baker<BuildingBarracksAuthoring>
    {
        public override void Bake(BuildingBarracksAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingBarracks()
            {
                progressMax = authoring.progressMax
            });

            DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffer = AddBuffer<SpawnUnitTypeBuffer>(entity);

            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = UnitTypeSO.UnitType.Soldier
            });
            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = UnitTypeSO.UnitType.Scout
            });
            spawnUnitTypeDynamicBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = UnitTypeSO.UnitType.Soldier
            });
        }
    }

}

public struct BuildingBarracks : IComponentData
{
    public float progress;
    public float progressMax;
}

[InternalBufferCapacity(10)]
public struct SpawnUnitTypeBuffer : IBufferElementData
{
    public UnitTypeSO.UnitType unitType;
    
}
