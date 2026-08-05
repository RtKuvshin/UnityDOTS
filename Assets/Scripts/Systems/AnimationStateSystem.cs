using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(ShootAttackSystem))]
partial struct AnimationStateSystem : ISystem
{
    private ComponentLookup<ActiveAnimation> _activeAnimationComponentLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _activeAnimationComponentLookup = state.GetComponentLookup<ActiveAnimation>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _activeAnimationComponentLookup.Update(ref state);

        IdleWalkingAnimationStateJob idleWalkingAnimationStateJob = new IdleWalkingAnimationStateJob
        {
            activeAnimationComponentLookup = _activeAnimationComponentLookup
        };
        idleWalkingAnimationStateJob.ScheduleParallel();

        AimShootAnimationStateJob aimShootAnimationStateJob = new AimShootAnimationStateJob
        {
            activeAnimationComponentLookup = _activeAnimationComponentLookup
        };
        aimShootAnimationStateJob.ScheduleParallel();

        MeleeAttackAnimationStateJob meleeAttackAnimationStateJob = new MeleeAttackAnimationStateJob
        {
            activeAnimationComponentLookup = _activeAnimationComponentLookup
        };
        meleeAttackAnimationStateJob.ScheduleParallel();

        /*foreach ((RefRO<AnimatedMesh> animatedMesh, 
                     RefRO<UnitMover> unitMover, 
                     RefRO<UnitAnimations> unitAnimations) in SystemAPI.Query<RefRO<AnimatedMesh>, 
                     RefRO<UnitMover>, 
                     RefRO<UnitAnimations>>())
        {
            RefRW<ActiveAnimation> activeAnimation =
                SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.meshEntity);
            activeAnimation.ValueRW.nextAnimationType = unitMover.ValueRO.isMoving ? unitAnimations.ValueRO.walkAnimationType : unitAnimations.ValueRO.idleAnimationType;
        }*/

        /*foreach ((RefRO<AnimatedMesh> animatedMesh, 
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
        }*/

        /*foreach ((RefRO<AnimatedMesh> animatedMesh,
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
        }*/
    }

}

[BurstCompile]
public partial struct IdleWalkingAnimationStateJob: IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;
    public void Execute(in AnimatedMesh animatedMesh, in UnitMover unitMover, in UnitAnimations unitAnimations)
    {
        RefRW<ActiveAnimation> activeAnimation = activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
        activeAnimation.ValueRW.nextAnimationType = unitMover.isMoving ? unitAnimations.walkAnimationType : unitAnimations.idleAnimationType;
    }
}

[BurstCompile]
public partial struct AimShootAnimationStateJob: IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;
    public void Execute(in AnimatedMesh animatedMesh, in UnitMover unitMover, in UnitAnimations unitAnimations, in ShootAttack shootAttack, in Target target)
    {
        if (!unitMover.isMoving && target.targetEntity != Entity.Null)
        {
            RefRW<ActiveAnimation> activeAnimation =
                activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType =  unitAnimations.aimAnimationType ;
        }
            
        if (shootAttack.onShoot.isTriggered)
        {
            RefRW<ActiveAnimation> activeAnimation =
                activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType =  unitAnimations.shootAnimationType ;
                
        }
    }
}

[BurstCompile]
public partial struct MeleeAttackAnimationStateJob: IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<ActiveAnimation> activeAnimationComponentLookup;
    public void Execute(in AnimatedMesh animatedMesh, in UnitAnimations unitAnimations, in MeleeAttack meleeAttack)
    {
        if (meleeAttack.onAttacked)
        {
            RefRW<ActiveAnimation> activeAnimation =
                activeAnimationComponentLookup.GetRefRW(animatedMesh.meshEntity);
            activeAnimation.ValueRW.nextAnimationType =  unitAnimations.meleeAnimationType ;
        }
    }
}
