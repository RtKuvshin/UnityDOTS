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
                     DynamicBuffer<SpawnUnitTypeBuffer> spawnUnitTypeBuffer,
                     RefRO<BuildingBarracksUnitEnqueue> buildingBarracksUnitEnqueue,
                     EnabledRefRW<BuildingBarracksUnitEnqueue> enabledBuildingBarracksUnitEnqueue) in SystemAPI.Query<RefRW<BuildingBarracks>, 
                     DynamicBuffer<SpawnUnitTypeBuffer>, 
                     RefRO<BuildingBarracksUnitEnqueue>, 
                     EnabledRefRW<BuildingBarracksUnitEnqueue>>())
        {
            spawnUnitTypeBuffer.Add(new SpawnUnitTypeBuffer
            {
                unitType = buildingBarracksUnitEnqueue.ValueRO.unitType
            });
            enabledBuildingBarracksUnitEnqueue.ValueRW = false;
            buildingBarracks.ValueRW.onUnitQueueChanged = true;

        }
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

            if (buildingBarracks.ValueRO.activeUnitType != spawnUnitTypeDynamicBuffer[0].unitType)
            {
                buildingBarracks.ValueRW.activeUnitType = spawnUnitTypeDynamicBuffer[0].unitType;

                UnitTypeSO activeUnitTypeSO =
                    GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(buildingBarracks.ValueRO.activeUnitType);
                buildingBarracks.ValueRW.progressMax = activeUnitTypeSO.progressMax;
            }
            
            
            buildingBarracks.ValueRW.progress += SystemAPI.Time.DeltaTime;
            if (buildingBarracks.ValueRO.progress < buildingBarracks.ValueRO.progressMax)
            {
                continue;
            }
            buildingBarracks.ValueRW.progress = 0;

            UnitTypeSO.UnitType unitType = spawnUnitTypeDynamicBuffer[0].unitType;
            UnitTypeSO unitTypeSo = GameAssets.Instance.unitTypeListSO.GetUnitTypeSO(unitType);
            spawnUnitTypeDynamicBuffer.RemoveAt(0);
            buildingBarracks.ValueRW.onUnitQueueChanged = true;
            
            Entity spawnedUnitEntity = state.EntityManager.Instantiate(unitTypeSo.GetPrefabEntity(entitiesReference));

            SystemAPI.SetComponent(spawnedUnitEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            SystemAPI.SetComponent(spawnedUnitEntity, new MoveOverride
            {
                targetPosition = localTransform.ValueRO.Position + buildingBarracks.ValueRO.rallyPositionOffset
            });
            SystemAPI.SetComponentEnabled<MoveOverride>(spawnedUnitEntity, true);
        }
    }
    
}
