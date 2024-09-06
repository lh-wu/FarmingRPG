using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : SingletonMonobehavior<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null;

    [Header("Other")]
    [SerializeField] private SO_SoundList so_soundList = null;

    private Dictionary<SoundName, SoundItem> soundDictionary;
    protected override void Awake()
    {
        base.Awake();
        // ��so�ļ��ж�ȡsoundItem����䵽Dictionary��
        soundDictionary = new Dictionary<SoundName, SoundItem>();
        foreach(SoundItem soundItem in so_soundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }
    }

    public void PlaySound(SoundName soundName)
    {
        if(soundDictionary.TryGetValue(soundName,out SoundItem soundItem) && soundPrefab != null)
        {
            // �ӳ��л�ȡһ����Ӧprefab��gameobject
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();
            sound.SetSound(soundItem);
            soundGameObject.SetActive(true);        // Sound��OnEnable����ʹ����������
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));  //���ݸ������ĳ���ʱ�䣬�Զ���һ��ʱ���ֹͣ(disable)
        }
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }

}
