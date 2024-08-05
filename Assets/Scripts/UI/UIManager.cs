using UnityEngine.UI;
using UnityEngine;

public class UIManager : SingletonMonobehavior<UIManager>
{
    private bool _pauseMenuOn = false;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;           // 指向pauseMenuCanvas(在hierarchy中)
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    /// <summary>
    /// 布尔变量，表示暂停菜单是否处于激活状态
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

    // 取消拖拽与取消选择;打开暂停菜单，禁用玩家移动输入，禁用时间流逝；回收垃圾；设置button的背景颜色
    private void EnablePauseMenu()
    {
        // 取消拖拽与取消选择
        uiInventoryBar.DestoryCurrentlyDraggedItems();
        uiInventoryBar.ClearCurrentlySelectedItems();
        // 打开暂停菜单，禁用玩家移动输入，禁用时间流逝
        PauseMenuOn = true;
        PlayerController.Instance.EnablePlayerInput = false;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        // 回收垃圾
        System.GC.Collect();
        // 设置button的背景颜色（根据是否选中）
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
