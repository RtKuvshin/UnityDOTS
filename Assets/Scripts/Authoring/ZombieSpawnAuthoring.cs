using Unity.Entities;
using UnityEngine;

public class ZombieSpawnAuthoring : MonoBehaviour
{
    public float timerMax;
    public class Baker: Baker<ZombieSpawnAuthoring>
    {
        public override void Bake(ZombieSpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ZombieSpawn
            {
                timerMax = authoring.timerMax
            });
        }
    }
}
public struct ZombieSpawn: IComponentData
{
    public float timer;
    public float timerMax;
}


