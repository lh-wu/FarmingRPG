using UnityEngine.UI;
using UnityEngine;

public class UIManager : SingletonMonobehavior<UIManager>
{
    private bool _pauseMenuOn = false;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;           // ָ��pauseMenuCanvas(��hierarchy��)
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    /// <summary>
    /// ������������ʾ��ͣ�˵��Ƿ��ڼ���״̬
    /// </summary>
    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    // ȡ����ק��ȡ��ѡ��;����ͣ�˵�����������ƶ����룬����ʱ�����ţ���������������button�ı�����ɫ
    private void EnablePauseMenu()
    {
        // ȡ����ק��ȡ��ѡ��
        uiInventoryBar.DestoryCurrentlyDraggedItems();
        uiInventoryBar.ClearCurrentlySelectedItems();
        // ����ͣ�˵�����������ƶ����룬����ʱ������
        PauseMenuOn = true;
        PlayerController.Instance.EnablePlayerInput = false;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        // ��������
        System.GC.Collect();
        // ����button�ı�����ɫ�������Ƿ�ѡ�У�
        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();
        PauseMenuOn = false;
        PlayerController.Instance.EnablePlayerInput = true;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab()
    {
        for(int i = 0; i < menuTabs.Length; ++i)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }


    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for(int i = 0; i < menuTabs.Length; ++i)
        {
            if (i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }
        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
