using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventsSystem : ISystem
{

    private NativeArray<JobHandle> jobHandleNativeArray;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        jobHandleNativeArray = new NativeArray<JobHandle>(4, Allocator.Persistent);
    }

    
    public void OnUpdate(ref SystemState state)
    {
        jobHandleNativeArray[0] =  new ResetAttackEventJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[1] = new ResetHealthEventJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[2] = new ResetSelectedEventJob().ScheduleParallel(state.Dependency);
        jobHandleNativeArray[3] = new ResetMeleeAttackEventJob().ScheduleParallel(state.Dependency);

        NativeList<Entity> onBarracksUnitQueueChangedEntityList = new NativeList<Entity>(Allocator.TempJob);
        new ResetBuildingBarracksEventJobs()
        {
            onUnitQueueChangedEntityList = onBarracksUnitQueueChangedEntityList.AsParallelWriter()
        }.ScheduleParallel(state.Dependency).Complete();
        
        
        DOTSEventsManager.Instance.TriggerOnBarracksUnitQueueChanged(onBarracksUnitQueueChangedEntityList);
        state.Dependency = JobHandle.CombineDependencies(jobHandleNativeArray);
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
public partial struct ResetMeleeAttackEventJob : IJobEntity
{
    public void Execute(ref MeleeAttack meleeAttack)
    {
        meleeAttack.onAttacked = false;
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
[BurstCompile]
public partial struct ResetBuildingBarracksEventJobs : IJobEntity
{
    public NativeList<Entity>.ParallelWriter onUnitQueueChangedEntityList;
    public void Execute(ref BuildingBarracks buildingBarracks, Entity entity)
    {
        if (buildingBarracks.onUnitQueueChanged)
        {
            onUnitQueueChangedEntityList.AddNoResize(entity);
        }
        buildingBarracks.onUnitQueueChanged = false;
    }
}
