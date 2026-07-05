using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<LocalTransform> localTransform,
                     RefRO<MoveOverride> moveOverride, 
                     EnabledRefRW<MoveOverride> enabledMoveOverride, 
                     RefRW<UnitMover> unitMover) in SystemAPI.Query<RefRO<LocalTransform>, 
                     RefRO<MoveOverride>,
                     EnabledRefRW<MoveOverride>, 
                     RefRW<UnitMover>>())
        {
            if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) > UnitMoverSystem.REACH_TARGET_DISTANCE_SQUARE)
            {
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                enabledMoveOverride.ValueRW = false;
            }
        }    
    }

}
