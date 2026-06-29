using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRW<RandomWalking> randomWalking, 
                     RefRW<UnitMover> unitMover, 
                     RefRO<LocalTransform> localTransform)
                 in SystemAPI.Query<RefRW<RandomWalking>, 
                     RefRW<UnitMover>, 
                     RefRO<LocalTransform>>())
        {
            if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.targetPosition ) < UnitMoverSystem.REACH_TARGET_DISTANCE_SQUARE)
            {
                Random random = randomWalking.ValueRO.random;
                float3 randomDirection = new float3(random.NextFloat(-1, 1), 0, random.NextFloat(-1,1));
                randomDirection = math.normalize(randomDirection);

                randomWalking.ValueRW.targetPosition = randomWalking.ValueRO.originPosition + randomDirection *
                    random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);
                randomWalking.ValueRW.random = random;
                //UnityEngine.Debug.Log(randomWalking.ValueRO.targetPosition);
            }
            else
            {
                unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPosition;
            }
        }
    }
}
