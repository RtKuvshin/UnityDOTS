using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


[UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct ShootLightSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntitiesReference entitiesReference = SystemAPI.GetSingleton<EntitiesReference>();
            foreach (RefRW<ShootAttack> shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
            {
                if (shootAttack.ValueRO.onShoot.isTriggered)
                {
                    Entity shootLightEntity = state.EntityManager.Instantiate(entitiesReference.shootLightPrefabEntity);
                    SystemAPI.SetComponent(shootLightEntity, LocalTransform.FromPosition(shootAttack.ValueRO.onShoot.shootFromPosition));
                }
            }
        }
    }
