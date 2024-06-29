using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{   // 该脚本被attach到player下的一个子object中
    [SerializeField] private GameObject character = null;                           //指向player
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

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

    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributeList)
    {

        foreach(CharacterAttribute characterAttribute in characterAttributeList)
        {
            Animator currentAnimator = null;
            string animatorSOAssetName = characterAttribute.characterPart.ToString();   //目标部件(arms或body或其他)
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
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach(AnimationClip animationClip in animationsList)
            {
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColor.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }


}
