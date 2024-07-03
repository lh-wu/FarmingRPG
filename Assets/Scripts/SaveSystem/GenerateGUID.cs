using UnityEngine;


//使其可以在编辑时也起作用
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        // 使得仅在编辑时候运行
        if (!Application.IsPlaying(gameObject))
        {
            if (_gUID == "")
            {
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
