using BraketsEngine;
using ImGuiNET;
using Microsoft.Xna.Framework;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BraketsEditor;

public class SubMenu
{
    public string name;
    public UIButton button;
    public Action<object> click;
}
public class Menu
{
    public string name;
    public int posX;
    public UIButton button;
    public List<SubMenu> subMenus = new List<SubMenu>();
}

public class GlobalMenuBar : UIScreen
{
    List<Menu> menus = new List<Menu>();
    List<UIImage> controlButtons = new List<UIImage>();

    ImGuiStylePtr imStyle = ImGui.GetStyle();
    UIImage background, subMenuBackground;

    UIText StatusText;

    bool isCreated = false;
    bool isOpen = false;

    public void Create()
    {
        background = new UIImage();
        background.Size.X = Globals.APP_Width;
        background.Size.Y = 35;
        background.BackgroundColor = new Color(imStyle.Colors[(int)ImGuiCol.MenuBarBg]);

        subMenuBackground = new UIImage();
        subMenuBackground.BackgroundColor = new Color(imStyle.Colors[(int)ImGuiCol.Border]);


        Globals.DEBUG_UI_MENUBAR_SIZE_X = (int)background.Size.X;
        Globals.DEBUG_UI_MENUBAR_SIZE_Y = (int)background.Size.Y;

        this.AddElements([background, subMenuBackground]);
        this.Show();

        AddStatusText();

        subMenuBackground.visible = false;

        isCreated = true;
        Globals.GlobalMenuBar = this;
    }

    public void AddMenu(string name)
    {
        UIButton menuButn = new UIButton(name);
        menuButn.SetFontSize(22);
        menuButn.ButtonBackground = new Color(imStyle.Colors[(int)ImGuiCol.Button]);
        menuButn.ButtonHoverBackground = new Color(imStyle.Colors[(int)ImGuiCol.ButtonHovered]);
        menuButn.ForegroundColor = new Color(imStyle.Colors[(int)ImGuiCol.Text]);
        menuButn.OnClick = OnMenuClick;

        int xPos = 0;
        foreach (var menu in menus)
        {
            xPos += (int)menu.button.GetTextSize().X + (int)menu.button.padding.X + 5;
        }
        menuButn.SetAlign(UIAllign.TopLeft, new Vector2(xPos + 5, 2));

        this.AddElement(menuButn);
        menus.Add(new Menu { name = name, posX = xPos, button = menuButn });
    }

    public void AddSubMenu(string name, string menuName, Action<object> onClick)
    {
        foreach (var menu in menus)
        {
            if (menu.name == menuName)
            {
                UIButton subMenuButn = new UIButton(name);
                subMenuButn.SetPaddding(new Vector2(20, 10));
                int xPos = (int)(menu.posX + 2 + subMenuButn.padding.X + subMenuButn.margin.X);
                subMenuButn.SetAlign(UIAllign.TopLeft, new Vector2(xPos, menu.subMenus.Count * 30 + 30));
                subMenuButn.ButtonBackground = subMenuBackground.BackgroundColor;
                subMenuButn.ButtonHoverBackground = new Color(imStyle.Colors[(int)ImGuiCol.ButtonHovered]);
                subMenuButn.ForegroundColor = new Color(imStyle.Colors[(int)ImGuiCol.Text]);
                subMenuButn.SetFontSize(22);
                subMenuButn.Clickable = false;
                subMenuButn.OnClick = (sender) =>
                {
                    onClick.Invoke(sender);
                    HideAllSubMenus();
                };

                this.AddElement(subMenuButn);

                subMenuButn.visible = false;

                menu.subMenus.Add(new SubMenu
                {
                    name = name,
                    click = onClick,
                    button = subMenuButn
                });
            }
        }
    }

    async void OnMenuClick(object sender)
    {
        ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.NoMouse;

        var button = sender as UIButton;

        Menu selectedMenu = new Menu();
        foreach (var menu in menus)
        {
            if (menu.name == button.Text.Trim())
            {
                selectedMenu = menu;
            }
        }

        if (selectedMenu.subMenus.Any(sm => sm.button.visible))
        {
            HideAllSubMenus();
            subMenuBackground.visible = false;
        }
        else
        {
            foreach (var menu in menus)
            {
                if (menu != selectedMenu)
                {
                    foreach (var subMenu in menu.subMenus)
                    {
                        subMenu.button.visible = false;
                        subMenu.button.Clickable = false;
                    }
                }
            }

            if (selectedMenu.subMenus.Count < 1)
                return;

            subMenuBackground.Size.X = selectedMenu.subMenus.Max(sm => sm.button.Size.X + 10);
            subMenuBackground.Size.Y = selectedMenu.subMenus.Max(sm => (sm.button.Size.Y * selectedMenu.subMenus.Count));

            subMenuBackground.SetAlign(UIAllign.TopLeft, button.margin + new Vector2(15, button.Size.Y));
            subMenuBackground.visible = true;

            await Task.Delay(10);

            int i = 0;
            float yPos = button.Size.Y + 10;
            foreach (var submenu in selectedMenu.subMenus)
            {
                i++;

                submenu.button.visible = true;
                submenu.button.Clickable = true;

                submenu.button.OnClick = (sender) =>
                {
                    HideAllSubMenus();
                    submenu.click.Invoke(sender);
                };

                yPos += submenu.button.Size.Y + 2;
            }
        }
    }

    public void HideAllSubMenus()
    {
        foreach (var menu in menus)
        {
            foreach (var submenu in menu.subMenus)
            {
                submenu.button.visible = false;
                submenu.button.Clickable = false;
            }
        }

        subMenuBackground.visible = false;
        ImGui.GetIO().ConfigFlags &= ~ImGuiConfigFlags.NoMouse;
    }

    public void AddControlButton(string tag, string imageName, Action<object> action)
    {
        UIImage control = new UIImage(imageName);
        control.Clickable = true;
        control.Tag = tag;
        control.OnClick = action;
        control.Size = new Vector2(20);
        control.SetAlign(UIAllign.TopLeft, new Vector2((Globals.APP_Width / 2 - 300) + controlButtons.Count * 42, 8));

        controlButtons.Add(control);
        this.AddElement(control);
    }
    public UIImage GetControlButton(string tag)
    {
        foreach (var control in controlButtons)
        {
            if (control.Tag == tag)
                return control;
        }

        Debug.Error($"No control button with tag '{tag}' was found!", this);
        return null;
    }

    internal void AddStatusText()
    {
        StatusText = new UIText("Status");
        this.AddElement(StatusText);
    }

    public void OnAppResize()
    {
        if (!isCreated)
            return;

        background.Size.X = Globals.APP_Width;

        int i = 0;
        foreach (var control in controlButtons)
        {
            control.SetAlign(UIAllign.TopLeft, new Vector2((Globals.APP_Width / 2 - 300) + i * 42, 8));
            i++;
        }
    }

    public void Update()
    {
        StatusText.Text = Globals.EditorManager.Status;
        StatusText.SetAlign(UIAllign.TopLeft, new Vector2(Globals.APP_Width - StatusText.GetTextSize().X / 2 - 25, 0));
    }

    public static void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Tools"))
            {
                if (ImGui.MenuItem("Diagnostics"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Diagnostics",
                        view = DiagnosticsView.Draw,
                        update = DiagnosticsView.Update
                    });
                }
                if (ImGui.MenuItem("Level Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Level Editor",
                        view = LevelEditor.Draw,
                        update = LevelEditor.Update
                    });
                }
                if (ImGui.MenuItem("Tilemap Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Tilemap Editor",
                        view = DiagnosticsView.Draw
                    });
                }
                if (ImGui.MenuItem("Particle Editor"))
                {
                    MainToolsWindow.AddTab(new ToolTab
                    {
                        name = "Particle Editor",
                        view = ParticleEditor.Draw,
                        update = ParticleEditor.Update
                    });
                    ParticleEditor.Init(type: "new");
                }
                ImGui.EndMenu();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5, 0).ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 0).ToNumerics());

            ImGui.SetCursorPosX(275);
            ImGui.Text("|"); ImGui.Text(Globals.projectName);

            ImGui.SetCursorPosX((ImGui.GetWindowSize().X - 300) / 2);

            ImGui.EndDisabled();
            BuildManager.runningTimer += Globals.DEBUG_DT;
            ImGui.PopStyleVar(2);

            if (Globals.IS_DEV_BUILD)
            {
                ImGui.SetCursorPosX((ImGui.GetWindowSize().X - 500));
                ImGui.TextColored(Color.Gray.ToVector4().ToNumerics(), "DEV_BUILD");
            }

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(5, 0).ToNumerics());
            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(0, 0).ToNumerics());
            ImGui.SetCursorPosX(ImGui.GetWindowSize().X - ImGui.CalcTextSize(Globals.EditorManager.Status).X - 10);
            Throbber.Draw(ImGui.GetWindowSize().X - ImGui.CalcTextSize(Globals.EditorManager.Status).X - 35, 18);
            ImGui.Text(Globals.EditorManager.Status);
            ImGui.PopStyleVar(2);

            Globals.DEBUG_UI_MENUBAR_SIZE_X = (int)ImGui.GetWindowSize().X;
            Globals.DEBUG_UI_MENUBAR_SIZE_Y = (int)ImGui.GetWindowSize().Y;

            ImGui.EndMainMenuBar();
        }
    }
}