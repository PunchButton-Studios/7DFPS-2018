using System.IO;
using UnityEngine;

public class ActivatablePoster : ActivatableBaseObject
{
    private string filepath = string.Empty;
    private Material material;

    public override string ActivateInfo
    {
        get
        {
            return "Change Image";
        }
    }

    private void Awake()
    {
        material = new Material(GetComponent<MeshRenderer>().material);
        GetComponent<MeshRenderer>().material = material;
    }

    public override void Activate(EntityPlayer player) => FindObjectOfType<PosterMenu>().Open(this);

    public override void ApplyExtraData(string extraData)
    {
        filepath = extraData;
        Texture2D texture2D = new Texture2D(512, 512);
        if(File.Exists(filepath))
        {
            byte[] data = File.ReadAllBytes(filepath);
            texture2D.LoadImage(data);
        }
        material.mainTexture = texture2D;
        Slot.extraData = extraData;
    }
}