using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{   // �ýű���attach��player�µ�һ����object��
    [SerializeField] private GameObject character = null;                           //ָ��player
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;        //���ڶ�ȡ���е����ж���
    // ���������ֵ����ö�Ϊ����<��>�������SO_AnimationType[]��������
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    /// <summary>
    /// ����hierarchy�ж�ȡ��soAnimationTypeArray���������ֵ���г�ʼ��
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
    /// �������ΪcharacterAttribute��List��ÿһ��Ԫ��ָ���˱��嶯����<��λ����ɫ���壬���ͱ���>
    /// ��animator���б��嶯�����滻��������ڱ��壩
    /// </summary>
    /// <param name="characterAttributeList"></param>
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributeList)
    {

        foreach(CharacterAttribute characterAttribute in characterAttributeList)
        {
            Animator currentAnimator = null;
            string animatorSOAssetName = characterAttribute.characterPart.ToString();   //Ŀ�겿��(Arms��Body������)
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();  //��ҵ�����animator

            // ԭ������Ŀ�궯����list
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // Ѱ������Ҫoverride��<����>��Ӧ��animator����arms��body��
            foreach(Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }
            // �õ���ǰanimator��List<AnimationClip>
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach(AnimationClip animationClip in animationsList)
            {
                // ���ڱ��嶯��ָֻ���� ����Ĳ�λ����ɫ�����͡�������Ķ����������߻���û��ָ����
                // ����Ҫ��ȡ���ܵĶ������� ��idle run walk�ȣ��ô���so_AnimationTypeΪ�������
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {   // ����ָ���ı��岿λ����ɫ������ �Ϳ��ܵĶ������֣��Ȳ�ѯ�Ƿ���ڸö����ı���
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColor.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();
                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);
                    if (foundSwapAnimation)
                    {   // ���ڱ��壬����animsKeyValuePairList
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            aoc.ApplyOverrides(animsKeyValuePairList);              // ����ԭ����-Ŀ�Ķ�����list�������滻
            currentAnimator.runtimeAnimatorController = aoc;        // �����滻��aoc��Ч�����Ҫ���Ǹ�����ʹ�õ�animator
        }
    }


}
