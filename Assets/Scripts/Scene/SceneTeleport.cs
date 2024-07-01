using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{

    [SerializeField] private SceneName SceneNameGoto = SceneName.Scene1_Farm;
    [Tooltip("设置为0，则传送后的该位置分量与现位置分量相同")]
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            // 如果不设置
            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;
            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;
            float zPosition = 0f;

            SceneControllerManager.Instance.FadeAndLoadScene(SceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));

        }
    }
}
