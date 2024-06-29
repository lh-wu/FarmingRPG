using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{   // 该脚本被attach到player下的一个子object中
    [SerializeField] private GameObject character = null;                           //指向player
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;        //用于读取已有的所有动画
    // 下面两个字典作用都为根据<键>对上面的SO_AnimationType[]进行索引
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    /// <summary>
    /// 根据hierarchy中读取的soAnimationTypeArray，对两个字典进行初始化
    /// </summary>
    private void Start()
    {
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);

            string key = item.characterPart.ToString() + item.partVariantColor.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }

    }

    /// <summary>
    /// 传入参数为characterAttribute的List，每一个元素指明了变体动画的<部位，颜色变体，类型变体>
    /// 对animator进行变体动画的替换（如果存在变体）
    /// </summary>
    /// <param name="characterAttributeList"></param>
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributeList)
    {

        foreach(CharacterAttribute characterAttribute in characterAttributeList)
        {
            Animator currentAnimator = null;
            string animatorSOAssetName = characterAttribute.characterPart.ToString();   //目标部件(Arms或Body或其他)
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();  //玩家的所有animator

            // 原动画，目标动画的list
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // 寻找与需要override的<部件>对应的animator，如arms、body等
            foreach(Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }
            // 得到当前animator的List<AnimationClip>
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach(AnimationClip animationClip in animationsList)
            {
                // 由于变体动画只指明了 变体的部位，颜色，类型。而具体的动画名字如走或跑没有指明；
                // 所以要获取可能的动画名字 如idle run walk等；该处的so_AnimationType为这个作用
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {   // 根据指定的变体部位，颜色，类型 和可能的动画名字，先查询是否存在该动画的变体
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColor.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();
                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);
                    if (foundSwapAnimation)
                    {   // 存在变体，加入animsKeyValuePairList
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            aoc.ApplyOverrides(animsKeyValuePairList);              // 根据原动画-目的动画的list，进行替换
            currentAnimator.runtimeAnimatorController = aoc;        // 上述替换对aoc生效，因此要覆盖给真正使用的animator
        }
    }


}
