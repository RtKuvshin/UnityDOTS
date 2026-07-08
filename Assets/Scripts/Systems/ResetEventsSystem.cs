using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new ResetAttackEventJob().ScheduleParallel();
        new ResetHealthEventJob().ScheduleParallel();
        new ResetSelectedEventJob().ScheduleParallel();
        
        /*foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            selected.ValueRW.onSelected = false;
            selected.ValueRW.onDeselected = false;
        }
        foreach (RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.onHealthChanged = false;
        }
        foreach (RefRW<ShootAttack> shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
        {
            shootAttack.ValueRW.onShoot.isTriggered = false;
        }*/
        
    }
}



[BurstCompile]
public partial struct ResetAttackEventJob : IJobEntity
{
    public void Execute(ref ShootAttack shootAttack)
    {
        shootAttack.onShoot.isTriggered = false;
    }
}

[BurstCompile]
public partial struct ResetHealthEventJob : IJobEntity
{
    public void Execute(ref Health health)
    {
        health.onHealthChanged = false;
    }
}
[BurstCompile]
[WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
public partial struct ResetSelectedEventJob : IJobEntity
{
    public void Execute(ref Selected selected)
    {
        selected.onSelected = false;
        selected.onDeselected = false;
    }
}
