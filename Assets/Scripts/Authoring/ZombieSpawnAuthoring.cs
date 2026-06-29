using Unity.Entities;
using UnityEngine;

public class ZombieSpawnAuthoring : MonoBehaviour
{
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public class Baker: Baker<ZombieSpawnAuthoring>
    {
        public override void Bake(ZombieSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawn
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax
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
}


