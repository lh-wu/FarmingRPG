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
        // 从so文件中读取soundItem并填充到Dictionary中
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
            // 从池中获取一个对应prefab的gameobject
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();
            sound.SetSound(soundItem);
            soundGameObject.SetActive(true);        // Sound的OnEnable方法使得声音播放
            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));  //根据该声音的持续时间，自动在一定时间后停止(disable)
        }
    }

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }

}
