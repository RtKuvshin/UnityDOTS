using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    public ComponentLookup<LocalTransform> _localTransformComponentLookup;
    public ComponentLookup<Health> _healthComponentLookup;
    public ComponentLookup<PostTransformMatrix> _postTransformMatrixLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _localTransformComponentLookup = state.GetComponentLookup<LocalTransform>();
        _healthComponentLookup = state.GetComponentLookup<Health>(true);
        _postTransformMatrixLookup = state.GetComponentLookup<PostTransformMatrix>(false);
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if (Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }
        
        _localTransformComponentLookup.Update(ref state);
        _healthComponentLookup.Update(ref state);
        _postTransformMatrixLookup.Update(ref state);

        HealthBarJobSystem healthBarJobSystem = new HealthBarJobSystem
        {
            healthComponentLookup = _healthComponentLookup,
            cameraForward = cameraForward,
            localTransformComponentLookup = _localTransformComponentLookup,
            postTransformMatrixLookup = _postTransformMatrixLookup
        };
        healthBarJobSystem.ScheduleParallel();

        /*foreach ((RefRO<HealthBar> healthBar, RefRW<LocalTransform> localTransform) in SystemAPI.Query<RefRO<HealthBar>, RefRW<LocalTransform>>())
        {
            LocalTransform parentLocalTransform =
                SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);

            if (localTransform.ValueRW.Scale == 1f)
            {
                localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }
            
            Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);
            if (!health.onHealthChanged)
            {
                continue;
            }
            float healthNormalized = (float)health.healthAmount / health.healthAmountMax;
            localTransform.ValueRW.Scale = healthNormalized >= 1 ? 0 : 1;
            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barEntity);

            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }*/
    }
}

[BurstCompile]
public partial struct HealthBarJobSystem : IJobEntity
{
    [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> localTransformComponentLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<PostTransformMatrix> postTransformMatrixLookup;
    [ReadOnly] public ComponentLookup<Health> healthComponentLookup;
    
    public float3 cameraForward;
    public void Execute(in HealthBar healthBar,Entity entity)
    {
        RefRW<LocalTransform> localTransform = localTransformComponentLookup.GetRefRW(entity);
        LocalTransform parentLocalTransform =
            localTransformComponentLookup[healthBar.healthEntity];

        if (localTransform.ValueRO.Scale == 1f)
        {
            localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
        }
            
        Health health = healthComponentLookup[healthBar.healthEntity];
        if (!health.onHealthChanged)
        {
            return;
        }
        float healthNormalized = (float)health.healthAmount / health.healthAmountMax;
        localTransform.ValueRW.Scale = healthNormalized >= 1 ? 0 : 1;
        RefRW<PostTransformMatrix> barVisualPostTransformMatrix = postTransformMatrixLookup.GetRefRW(healthBar.barEntity);

        barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
    }
}
