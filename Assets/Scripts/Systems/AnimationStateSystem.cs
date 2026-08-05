using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(ShootAttackSystem))]
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
        
        
        foreach ((RefRO<AnimatedMesh> animatedMesh, 
                     RefRO<UnitMover> unitMover, 
                     RefRO<UnitAnimations> unitAnimations,
                     RefRO<ShootAttack> shootAttack,
                     RefRO<Target> target) in SystemAPI.Query<RefRO<AnimatedMesh>, 
                     RefRO<UnitMover>, 
                     RefRO<UnitAnimations>, 
                     RefRO<ShootAttack>, RefRO<Target>>())
        {
            if (!unitMover.ValueRO.isMoving && target.ValueRO.targetEntity != Entity.Null)
            {
                RefRW<ActiveAnimation> activeAnimation =
                        SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
                    activeAnimation.ValueRW.nextAnimationType =  unitAnimations.ValueRO.aimAnimationType ;
            }
            
            if (shootAttack.ValueRO.onShoot.isTriggered)
            {
                RefRW<ActiveAnimation> activeAnimation =
                    SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
                activeAnimation.ValueRW.nextAnimationType =  unitAnimations.ValueRO.shootAnimationType ;
                
            }
        }
        
        foreach ((RefRO<AnimatedMesh> animatedMesh,
                     RefRO<UnitAnimations> unitAnimations,
                     RefRO<MeleeAttack> meleeAttack) in SystemAPI.Query<RefRO<AnimatedMesh>,
                     RefRO<UnitAnimations>, 
                     RefRO<MeleeAttack>
                     >())
        {

            if (meleeAttack.ValueRO.onAttacked)
            {
                RefRW<ActiveAnimation> activeAnimation =
                    SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
                activeAnimation.ValueRW.nextAnimationType =  unitAnimations.ValueRO.meleeAnimationType ;
                
            }
        }
    }

}
