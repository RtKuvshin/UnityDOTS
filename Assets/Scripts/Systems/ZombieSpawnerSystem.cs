using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
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
        }   
    }
}
