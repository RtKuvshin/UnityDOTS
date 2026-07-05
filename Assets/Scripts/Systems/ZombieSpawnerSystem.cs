using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReference>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach ( (RefRO<LocalTransform> localTransform, RefRW<ZombieSpawn> zombieSpawn) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ZombieSpawn>>())
        {
            zombieSpawn.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (zombieSpawn.ValueRO.timer > 0)
            {
              continue;  
            }
            zombieSpawn.ValueRW.timer = zombieSpawn.ValueRO.timerMax;

            Entity zombieEntity = state.EntityManager.Instantiate(entitiesReference.zombiePrefabEntity);
            SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));
            
            entityCommandBuffer.AddComponent(zombieEntity, new RandomWalking
            {
                originPosition = localTransform.ValueRO.Position,
                targetPosition = localTransform.ValueRO.Position,
                distanceMin = zombieSpawn.ValueRO.randomWalkingDistanceMin,
                distanceMax = zombieSpawn.ValueRO.randomWalkingDistanceMax,
                random = new Random((uint)zombieEntity.Index)
            });
        }   
    }
}
