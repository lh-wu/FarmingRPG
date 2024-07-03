using UnityEngine;


//ʹ������ڱ༭ʱҲ������
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        // ʹ�ý��ڱ༭ʱ������
        if (!Application.IsPlaying(gameObject))
        {
            if (_gUID == "")
            {
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
