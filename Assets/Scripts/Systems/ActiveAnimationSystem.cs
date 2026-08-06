using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

partial struct ActiveAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AnimationDataHolder>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataHolder animationDataHolder= SystemAPI.GetSingleton<AnimationDataHolder>();
        ActiveAnimationJob activeAnimationJob = new ActiveAnimationJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            animationDataBlobArrayBlobAssetReference = animationDataHolder.animationDataBlobArrayBlobAssetReference
        };
        activeAnimationJob.ScheduleParallel();

        /*foreach ((RefRW<ActiveAnimation> activeAnimation, RefRW<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
        {
            ref AnimationData animationData = ref animationDataHolder.animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.ValueRO.activeAnimationType];
            
            activeAnimation.ValueRW.frameTimer += SystemAPI.Time.DeltaTime;
            if (activeAnimation.ValueRW.frameTimer > animationData.frameTimerMax)
            {
                activeAnimation.ValueRW.frameTimer -= animationData.frameTimerMax;
                activeAnimation.ValueRW.frame = (activeAnimation.ValueRO.frame + 1) % animationData.frameMax;
                materialMeshInfo.ValueRW.MeshID = animationData.batchMeshIDBlobArray[activeAnimation.ValueRO.frame];

                if (activeAnimation.ValueRO.frame == 0 && activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
                {
                    activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                }
                if (activeAnimation.ValueRO.frame == 0 && activeAnimation.ValueRO.activeAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
                {
                    activeAnimation.ValueRW.activeAnimationType = AnimationDataSO.AnimationType.None;
                }
            }
        }*/
    }
}

[BurstCompile]
public partial struct ActiveAnimationJob : IJobEntity
{
    public BlobAssetReference<BlobArray<AnimationData>> animationDataBlobArrayBlobAssetReference;
    public float deltaTime;
    public void Execute(ref ActiveAnimation activeAnimation, ref MaterialMeshInfo materialMeshInfo)
    {
        ref AnimationData animationData = ref animationDataBlobArrayBlobAssetReference.Value[(int)activeAnimation.activeAnimationType];
            
        activeAnimation.frameTimer += deltaTime;
        if (activeAnimation.frameTimer > animationData.frameTimerMax)
        {
            activeAnimation.frameTimer -= animationData.frameTimerMax;
            activeAnimation.frame = (activeAnimation.frame + 1) % animationData.frameMax;
            materialMeshInfo.Mesh = animationData.intMeshIDBlobArray[activeAnimation.frame];

            if (activeAnimation.frame == 0 && activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
            {
                activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.None;
            }
            if (activeAnimation.frame == 0 && activeAnimation.activeAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
            {
                activeAnimation.activeAnimationType = AnimationDataSO.AnimationType.None;
            }
        }
    }
}
