using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class ZombieSpawnAuthoring : MonoBehaviour
{
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int nearbyZombieAmountMax;
    public float nearbyZombieDistance;
    public class Baker: Baker<ZombieSpawnAuthoring>
    {
        public override void Bake(ZombieSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawn
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                nearbyZombieDistance = authoring.nearbyZombieDistance,
                nearbyZombieAmountMax = authoring.nearbyZombieAmountMax
            });
        }
    }
}
public struct ZombieSpawn: IComponentData
{
    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int nearbyZombieAmountMax;
    public float nearbyZombieDistance;
}


