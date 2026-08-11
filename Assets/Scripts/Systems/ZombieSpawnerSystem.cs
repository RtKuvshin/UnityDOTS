using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

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
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        NativeList<DistanceHit> distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);

        foreach ( (RefRO<LocalTransform> localTransform, RefRW<ZombieSpawn> zombieSpawn) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ZombieSpawn>>())
        {
            zombieSpawn.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (zombieSpawn.ValueRO.timer > 0)
            {
              continue;  
            }
            zombieSpawn.ValueRW.timer = zombieSpawn.ValueRO.timerMax;

            distanceHitList.Clear();
            CollisionFilter collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << GameAssets.UNITS_LAYER,
                GroupIndex = 0
            };

            int nearbyZombieAmount = 0;
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, zombieSpawn.ValueRO.nearbyZombieDistance, ref distanceHitList, collisionFilter))
            {
                foreach (var distanceHit in distanceHitList)
                {
                    if (!SystemAPI.Exists(distanceHit.Entity))
                    {
                        continue;
                    }

                    if (SystemAPI.HasComponent<Unit>(distanceHit.Entity) && SystemAPI.HasComponent<Zombie>(distanceHit.Entity))
                    {
                        nearbyZombieAmount++;
                    }
                }
            }

            if (nearbyZombieAmount >= zombieSpawn.ValueRO.nearbyZombieAmountMax)
            {
                continue;
            }

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
