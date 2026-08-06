using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem), UpdateInGroup(typeof(PostBakingSystemGroup))]
partial struct AnimationDataHolderBakerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        AnimationDataListSO animationDataListSO = null;
        foreach (var animationDataHolderObjectData in SystemAPI.Query<RefRO<AnimationDataHolderObjectData>>())
        {
            animationDataListSO = animationDataHolderObjectData.ValueRO.unityObjectRefAnimationDataListSO.Value;
        }
        
        Dictionary<AnimationDataSO.AnimationType, int[]> blobAssetDataDictionary = new Dictionary<AnimationDataSO.AnimationType, int[]>();

        foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(
                     typeof(AnimationDataSO.AnimationType)))
        {
            AnimationDataSO animationDataSo = animationDataListSO.GetAnimationDataSO(animationType);
            blobAssetDataDictionary[animationType] = new int[animationDataSo.meshArray.Length];
        }

        foreach ((RefRO<AnimationDataHolderSubEntity> animationDataHolderSubEntity, RefRO<MaterialMeshInfo> materialMeshInfo) in SystemAPI.Query<RefRO<AnimationDataHolderSubEntity>, RefRO<MaterialMeshInfo>>())
        {
            blobAssetDataDictionary[animationDataHolderSubEntity.ValueRO.animationType][
                animationDataHolderSubEntity.ValueRO.meshIndex] = materialMeshInfo.ValueRO.Mesh;
            
            //Debug.Log(animationDataHolderSubEntity.ValueRO.animationType + " :: " +
             //         animationDataHolderSubEntity.ValueRO.meshIndex + " = " + materialMeshInfo.ValueRO.Mesh);
        }

        foreach (var animationDataHolder in SystemAPI.Query<RefRW<AnimationDataHolder>>())
        {
            BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);
            ref BlobArray<AnimationData> animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();
            
            BlobBuilderArray<AnimationData> animationDataBlobBuilderArray =
                blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)).Length);
            
            int index = 0;
                foreach (AnimationDataSO.AnimationType animationType in System.Enum.GetValues(typeof(AnimationDataSO.AnimationType)))
                {
                    AnimationDataSO animationDataSo = animationDataListSO.GetAnimationDataSO(animationType);
                    
                    BlobBuilderArray<int> blobBuilderArray =
                        blobBuilder.Allocate<int>(ref animationDataBlobBuilderArray[index].intMeshIDBlobArray,
                            animationDataSo.meshArray.Length);
                    
                    animationDataBlobBuilderArray[index].frameTimerMax = animationDataSo.frameTimerMax;
                    animationDataBlobBuilderArray[index].frameMax = animationDataSo.meshArray.Length;
                    
                    
                    for (int i = 0; i < animationDataSo.meshArray.Length; i++)
                    {
                        blobBuilderArray[i] = blobAssetDataDictionary[animationType][i];
                    }

                    index++;
                }
            
                animationDataHolder.ValueRW.animationDataBlobArrayBlobAssetReference =
                        blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(Allocator.Persistent);
                blobBuilder.Dispose();
        }
    }

}
