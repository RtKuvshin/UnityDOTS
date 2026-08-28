using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using BoxCollider = UnityEngine.BoxCollider;

public class BuildingPlacementManager : MonoBehaviour
{
    [SerializeField] private BuildingTypeSO buildingTypeSO;
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (CanPlaceBuilding())
            {
                Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
            
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReference));
                EntitiesReference entitiesReference = entityQuery.GetSingleton<EntitiesReference>();

                Entity spawnedEntity = entityManager.Instantiate(entitiesReference.buildingTowerPrefabEntity);
                entityManager.SetComponentData(spawnedEntity, LocalTransform.FromPosition(mouseWorldPosition));
            }
        }
    }

    private bool CanPlaceBuilding()
    {
        Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();
            
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
        CollisionFilter collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << GameAssets.BUILDINGS_LAYER,
            GroupIndex = 0
        };

        BoxCollider boxCollider = buildingTypeSO.prefab.GetComponent<BoxCollider>();
        float bonusExtents = 1.1f; 
        NativeList<DistanceHit> distanceHitNativeList = new NativeList<DistanceHit>(Allocator.Temp);
        if (collisionWorld.OverlapBox(
                mouseWorldPosition,
                Quaternion.identity,
                boxCollider.size*0.5f*bonusExtents,
                ref distanceHitNativeList,
                collisionFilter))
        {
            return false;
        }
        distanceHitNativeList.Clear();

        if (collisionWorld.OverlapSphere(mouseWorldPosition,
                buildingTypeSO.buildingDistanceMin,
                ref distanceHitNativeList,
                collisionFilter))
        {
            foreach (var distanceHit in distanceHitNativeList)
            {
                if (entityManager.HasComponent<BuildingTypeHolder>(distanceHit.Entity))
                {
                    BuildingTypeHolder buildingTypeHolder =
                        entityManager.GetComponentData<BuildingTypeHolder>(distanceHit.Entity);
                    if (buildingTypeHolder.buildingType == buildingTypeSO.buildingType )
                    {
                        return false;
                    }
                    
                }
            }
        }

        return true;
    }
}
