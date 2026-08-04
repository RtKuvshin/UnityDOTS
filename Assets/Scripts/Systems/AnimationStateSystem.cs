using Unity.Burst;
using Unity.Entities;

partial struct AnimationStateSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((RefRO<AnimatedMesh> animatedMesh, 
                     RefRO<UnitMover> unitMover, 
                     RefRO<UnitAnimations> unitAnimations) in SystemAPI.Query<RefRO<AnimatedMesh>, 
        RefRO<UnitMover>, 
        RefRO<UnitAnimations>>())
        {
            RefRW<ActiveAnimation> activeAnimation =
                SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitMover.ValueRO.isMoving ? unitAnimations.ValueRO.walkAnimationType : unitAnimations.ValueRO.idleAnimationType;
        }
    }

}
