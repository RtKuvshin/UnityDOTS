using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct BuildingBarracksSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReference>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
        foreach ((RefRW<BuildingBarracks> buildingBarracks,
                     RefRO<LocalTransform> localTransform,
                     DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeDynamicBuffer) 
                 in SystemAPI.Query<RefRW<BuildingBarracks>,
                     RefRO<LocalTransform>,
                 DynamicBuffer<SpawnUnitTypeBuffer>>())
        {
            if (spawnUnitTypeDynamicBuffer.IsEmpty)
            {
                continue;
            }
            
            buildingBarracks.ValueRW.progress += SystemAPI.Time.DeltaTime;
            if (buildingBarracks.ValueRO.progress < buildingBarracks.ValueRO.progressMax)
            {
                continue;
            }
            buildingBarracks.ValueRW.progress = 0;

            UnitTypeSO.UnitType unitType = spawnUnitTypeDynamicBuffer[0].unitType;
            UnitTypeSO unitTypeSo = GameAssets.Instance.unitTypeListSO.GetUnitTypeListSO(unitType);
            spawnUnitTypeDynamicBuffer.RemoveAt(0);
            
            Entity spawnedUnitEntity = state.EntityManager.Instantiate(unitTypeSo.GetPrefabEntity(entitiesReference));

            SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
        }
    }
    
}
