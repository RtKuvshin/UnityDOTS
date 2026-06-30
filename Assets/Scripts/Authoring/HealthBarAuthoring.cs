using Unity.Entities;
using UnityEngine;

public class HealthBarAuthoring : MonoBehaviour
{
    public GameObject barGameObject;
    public GameObject healthGameObject;
    
    public class Baker: Baker<HealthBarAuthoring>
    {
        public override void Bake(HealthBarAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HealthBar()
            {
                healthEntity = GetEntity(authoring.healthGameObject, TransformUsageFlags.Dynamic),
                barEntity = GetEntity(authoring.barGameObject, TransformUsageFlags.NonUniformScale)
            });
        }
    }
    
}

public struct HealthBar : IComponentData
{
    public Entity healthEntity;
    public Entity barEntity;
}
