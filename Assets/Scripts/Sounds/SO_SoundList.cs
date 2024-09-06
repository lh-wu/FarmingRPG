using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptable Objects/Sounds/Sound List",fileName ="so_SoundList")]
public class SO_SoundList : ScriptableObject
{
    [SerializeField]
    public List<SoundItem> soundDetails;
}
