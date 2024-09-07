using System.Numerics;
using BraketsEngine;
using ImGuiNET;

namespace BraketsEditor;

public class WindowTheme
{
    public static bool rounded = false;
    public static string currentTheme = "light";

    public static void Refresh()
    {
        if (currentTheme == "light") Light();
        else Dark();
    }

    public static void Dark()
    {
        currentTheme = "dark";

        var style = ImGuiNET.ImGui.GetStyle();

        style.Alpha = 1.0f;
        style.DisabledAlpha = 0.6000000238418579f;
        style.WindowPadding = new Vector2(8.0f, 8.0f);
        style.WindowRounding = rounded ? 5f : 0f;
        style.WindowBorderSize = 0.1f;
        style.WindowMinSize = new Vector2(32.0f, 32.0f);
        style.WindowTitleAlign = new Vector2(0.0f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.Left;
        style.ChildRounding = 0.0f;
        style.ChildBorderSize = 1.0f;
        style.PopupRounding = 0.0f;
        style.PopupBorderSize = 1.0f;
        style.FramePadding = new Vector2(8.0f, 8.0f);
        style.FrameRounding = rounded ? 3f : 0f;
        style.FrameBorderSize = 1.0f;
        style.ItemSpacing = new Vector2(6.0f, 6.0f);
        style.ItemInnerSpacing = new Vector2(6.0f, 6.0f);
        style.CellPadding = new Vector2(4.0f, 4.0f);
        style.IndentSpacing = 20.0f;
        style.ColumnsMinSpacing = 6.0f;
        style.ScrollbarSize = 10f;
        style.ScrollbarRounding = rounded ? 2f : 0f;
        style.GrabMinSize = 5.0f;
        style.GrabRounding = rounded ? 2f : 0f;
        style.TabRounding = rounded ? 3.0f : 0f;
        style.TabBorderSize = 0.0f;
        style.TabMinWidthForCloseButton = 0.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);

        style.Colors[(int)ImGuiCol.Text] = new Vector4(0.9527897238731384f, 0.9527801871299744f, 0.9527801871299744f, 1.0f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.4980392158031464f, 0.4980392158031464f, 0.4980392158031464f, 1.0f);
        style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1287553906440735f, 0.1287540942430496f, 0.1287540942430496f, 1.0f);
        style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.1502146124839783f, 0.1502131074666977f, 0.1502131074666977f, 1.0f);
        style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.1587982773780823f, 0.1587966829538345f, 0.1587966829538345f, 1.0f);
        style.Colors[(int)ImGuiCol.Border] = new Vector4(0.2145922780036926f, 0.2145901322364807f, 0.2145901322364807f, 1.0f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(9.999999974752427e-07f, 9.999899930335232e-07f, 9.999899930335232e-07f, 0.0f);
        style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(0.1802574992179871f, 0.1802556961774826f, 0.1802556961774826f, 1.0f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.1949566304683685f, 0.556094229221344f, 0.8412017226219177f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.3006134033203125f, 0.6492553353309631f, 0.8755365014076233f, 0.3004291653633118f);
        style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.1330472230911255f, 0.1330458968877792f, 0.1330458968877792f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.124462254345417f, 0.1244629099965096f, 0.1244634985923767f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.1330472230911255f, 0.1330458968877792f, 0.1330458968877792f, 1.0f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.1459227204322815f, 0.1459212601184845f, 0.1459212601184845f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.2145922780036926f, 0.2145901322364807f, 0.2145901322364807f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.3819742202758789f, 0.3819704055786133f, 0.3819704055786133f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.5f, 0.5f, 0.5f, 1f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.5f, 0.5f, 0.5f, 1f);
        style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.2186814844608307f, 0.7229830026626587f, 0.9098712205886841f, 0.9184549450874329f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.686274528503418f, 0.686274528503418f, 0.686274528503418f, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
        style.Colors[(int)ImGuiCol.Button] = new Vector4(0.1502146124839783f, 0.1502131074666977f, 0.1502131074666977f, 1.0f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.155501127243042f, 0.3888762891292572f, 0.5751073360443115f, 1.0f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.1214979067444801f, 0.2854548096656799f, 0.4163089990615845f, 1.0f);
        style.Colors[(int)ImGuiCol.Header] = new Vector4(0.1759656667709351f, 0.1759639084339142f, 0.1759639084339142f, 1.0f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.0f, 0.4666666686534882f, 0.8392156958580017f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.1502882689237595f, 0.5891833305358887f, 0.8540772199630737f, 0.5193133354187012f);
        style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.464477151632309f, 0.4644781351089478f, 0.54935622215271f, 0.5f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.08554219454526901f, 0.3882894217967987f, 0.7381974458694458f, 0.7799999713897705f);
        style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.4806867241859436f, 0.7215782403945923f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.2988450825214386f, 0.5790952444076538f, 0.8927038908004761f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 0.6700000166893005f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.2618025541305542f, 0.6006572246551514f, 1.0f, 0.949999988079071f);
        style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.304718017578125f, 0.3047196269035339f, 0.3047210574150085f, 0.8619999885559082f);
        style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.222475990653038f, 0.4808828830718994f, 0.7854077219963074f, 0.800000011920929f);
        style.Colors[(int)ImGuiCol.TabSelected] = new Vector4(0.1490909606218338f, 0.4032223224639893f, 0.6094420552253723f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.1410782933235168f, 0.5708754062652588f, 0.8884119987487793f, 0.5965665578842163f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.0f, 0.4274509847164154f, 0.3490196168422699f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.1410782933235168f, 0.5708754062652588f, 0.8884119987487793f, 0.5965665578842163f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.0f, 0.6000000238418579f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.2141870260238647f, 0.2141870409250259f, 0.2188841104507446f, 1.0f);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.3098039329051971f, 0.3098039329051971f, 0.3490196168422699f, 1.0f);
        style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.2102983444929123f, 0.2102983444929123f, 0.2103004455566406f, 1.0f);
        style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.2317596673965454f, 0.2317573428153992f, 0.2317573428153992f, 0.1673820018768311f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.0f, 1.0f, 1.0f, 0.05999999865889549f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 0.3499999940395355f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.0f, 1.0f, 0.0f, 0.8999999761581421f);
        style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 1.0f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.0f, 1.0f, 1.0f, 0.699999988079071f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.3499999940395355f);

        Globals.Camera.BackgroundColor = new Microsoft.Xna.Framework.Color(style.Colors[(int)ImGuiCol.Border]);
    }

    public static void Light()
    {

        currentTheme = "light";

        var style = ImGuiNET.ImGui.GetStyle();

        style.Alpha = 1.0f;
        style.DisabledAlpha = 0.6000000238418579f;
        style.WindowPadding = new Vector2(8.0f, 8.0f);
        style.WindowRounding = rounded ? 5f : 0f;
        style.WindowBorderSize = 0.1f;
        style.WindowMinSize = new Vector2(32.0f, 32.0f);
        style.WindowTitleAlign = new Vector2(0.0f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.Left;
        style.ChildRounding = 0.0f;
        style.ChildBorderSize = 1.0f;
        style.PopupRounding = 0.0f;
        style.PopupBorderSize = 1.0f;
        style.FramePadding = new Vector2(8.0f, 8.0f);
        style.FrameRounding = rounded ? 2.5f: 0f;
        style.FrameBorderSize = 1.0f;
        style.ItemSpacing = new Vector2(6.0f, 6.0f);
        style.ItemInnerSpacing = new Vector2(6.0f, 6.0f);
        style.CellPadding = new Vector2(4.0f, 2.0f);
        style.IndentSpacing = 20.0f;
        style.ColumnsMinSpacing = 6.0f;
        style.ScrollbarSize = 10f;
        style.ScrollbarRounding = rounded ? 2f : 0f;
        style.GrabMinSize = 5.0f;
        style.GrabRounding = rounded ? 2f : 0f;
        style.TabRounding = rounded ? 3.0f: 0f;
        style.TabBorderSize = 0.0f;
        style.TabMinWidthForCloseButton = 0.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);

        style.Colors[(int)ImGuiCol.Text] = new Vector4(0.09803921729326248f, 0.09803921729326248f, 0.09803921729326248f, 1.0f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.4980392158031464f, 0.4980392158031464f, 0.4980392158031464f, 1.0f);
        style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.9490196108818054f, 0.9490196108818054f, 0.9490196108818054f, 1.0f);
        style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.9490196108818054f, 0.9490196108818054f, 0.9490196108818054f, 1.0f);
        style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.Border] = new Vector4(0.6000000238418579f, 0.6000000238418579f, 0.6000000238418579f, 1.0f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.0f, 0.4666666686534882f, 0.8392156958580017f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.3004291653633118f, 0.6894875764846802f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(1.0f, 0.9999899864196777f, 0.9999899864196777f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.9999899864196777f, 0.9999938607215881f, 1.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(1.0f, 0.9999899864196777f, 0.9999899864196777f, 1.0f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.686274528503418f, 0.686274528503418f, 0.686274528503418f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.0f, 0.0f, 0.0f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
        style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.09803921729326248f, 0.09803921729326248f, 0.09803921729326248f, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.686274528503418f, 0.686274528503418f, 0.686274528503418f, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
        style.Colors[(int)ImGuiCol.Button] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.66f, 0.8f, 0.91f, 1f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.8f, 0.89f, 0.97f, 1f);
        style.Colors[(int)ImGuiCol.Header] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.0f, 0.4666666686534882f, 0.8392156958580017f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.2344858348369598f, 0.5575844645500183f, 0.8154506683349609f, 0.4334763884544373f);
        style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.4274509847164154f, 0.4274509847164154f, 0.4980392158031464f, 0.5f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.08554219454526901f, 0.3882894217967987f, 0.7381974458694458f, 0.7799999713897705f);
        style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.09803921729326248f, 0.4000000059604645f, 0.7490196228027344f, 1.0f);
        style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 0.6700000166893005f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 0.949999988079071f);
        style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.7682326436042786f, 0.7682364583015442f, 0.7682403326034546f, 0.8619999885559082f);
        style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.6f, 0.76f, 0.86f, 1f);
        style.Colors[(int)ImGuiCol.TabSelected] = new Vector4(0.66f, 0.8f, 0.91f, 1f);
        style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.6078431606292725f, 0.6078431606292725f, 0.6078431606292725f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.0f, 0.4274509847164154f, 0.3490196168422699f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.8980392217636108f, 0.6980392336845398f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.0f, 0.6000000238418579f, 0.0f, 1.0f);
        style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.5665179491043091f, 0.5665179491043091f, 0.5665236115455627f, 1.0f);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.3098039329051971f, 0.3098039329051971f, 0.3490196168422699f, 1.0f);
        style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.2102983444929123f, 0.2102983444929123f, 0.2103004455566406f, 1.0f);
        style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.2317596673965454f, 0.2317573428153992f, 0.2317573428153992f, 0.1673820018768311f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.0f, 1.0f, 1.0f, 0.05999999865889549f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 0.3499999940395355f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.0f, 1.0f, 0.0f, 0.8999999761581421f);
        style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.2588235437870026f, 0.5882353186607361f, 0.9764705896377563f, 1.0f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.0f, 1.0f, 1.0f, 0.699999988079071f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.2000000029802322f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.3499999940395355f);
        
        Globals.Camera.BackgroundColor = new Microsoft.Xna.Framework.Color(style.Colors[(int)ImGuiCol.Border]);
    }

    public static void PushAccent()
    {
        Vector4 color = ImGui.GetStyle().Colors[currentTheme == "dark" ? (int)ImGuiCol.ButtonHovered : (int)ImGuiCol.ButtonActive];
        ImGui.PushStyleColor(ImGuiCol.Button, color * (currentTheme == "dark" ? 0.8f : 1f));
    }
    public static void PopAccent() => ImGui.PopStyleColor();
}