using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct HealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Vector3 cameraForward = Vector3.zero;
        if (Camera.main != null)
        {
            cameraForward = Camera.main.transform.forward;
        }
        foreach ((RefRO<HealthBar> healthBar, RefRW<LocalTransform> localTransform) in SystemAPI.Query<RefRO<HealthBar>, RefRW<LocalTransform>>())
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
        }
    }
}
