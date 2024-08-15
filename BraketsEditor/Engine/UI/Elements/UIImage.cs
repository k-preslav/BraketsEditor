namespace BraketsEngine;

public class UIImage : UIElement
{
    public UIImage(string imageName="ui_default") : base(text: "", textureName: imageName)
    {

    }

    public void SetImage(string name)
    {
        base.textureName = name;
        base.texture = ResourceLoader.GetTexture(textureName);
    }
}