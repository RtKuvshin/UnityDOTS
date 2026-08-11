using UnityEngine;
[CreateAssetMenu]
public class AnimationDataSO : ScriptableObject
{
    public enum AnimationType
    {
        None,
        SoldierIdle,
        SoldierWalk,
        ZombieIdle,
        ZombieWalk,
        SoldierAim,
        SoldierShoot,
        ZombieAttack,
        ScoutIdle,
        ScoutWalk,
        ScoutShoot,
        ScoutAim
    }

    public AnimationType animationType;
    public Mesh[] meshArray;
    public float frameTimerMax;

    public static bool IsAnimationUnInteractable(AnimationType _animationType)
    {
        switch (_animationType)
        {
            default:
                return false;
            case AnimationType.ScoutShoot:
            case AnimationType.SoldierShoot:
            case AnimationType.ZombieAttack:
                return true; 
        }
    }
}
