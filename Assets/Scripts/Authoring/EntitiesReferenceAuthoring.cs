using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class EntitiesReferenceAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabGameObject;
    public GameObject zombiePrefabGameObject;
    public GameObject shootLightPrefabGameObject;
    public GameObject soldierPrefabGameObject;
    public GameObject scoutPrefabGameObject;
    
    public class Baker : Baker<EntitiesReferenceAuthoring>
    {
        public override void Bake(EntitiesReferenceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReference
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                zombiePrefabEntity = GetEntity(authoring.zombiePrefabGameObject, TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefabGameObject, TransformUsageFlags.Dynamic),
                scoutPrefabEntity = GetEntity(authoring.scoutPrefabGameObject, TransformUsageFlags.Dynamic),
                soldierPrefabEntity = GetEntity(authoring.soldierPrefabGameObject, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct EntitiesReference : IComponentData
{
    public Entity bulletPrefabEntity;
    public Entity zombiePrefabEntity;
    public Entity shootLightPrefabEntity;
    public Entity scoutPrefabEntity;
    public Entity soldierPrefabEntity;
}
