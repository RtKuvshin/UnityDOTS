using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimationDataListSO : ScriptableObject
{
    public List<AnimationDataSO> animationDataListSO;

    public AnimationDataSO GetAnimationDataSO(AnimationDataSO.AnimationType animationType)
    {
        foreach (AnimationDataSO animationDataSo in animationDataListSO)
        {
            if (animationDataSo.animationType == animationType)
            {
                return animationDataSo;
            }
        }
        Debug.LogError("Couldn't find AnimationDataSO for AnimationType " + animationType);
        return null;
    }
}
