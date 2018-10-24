using System.IO;
using UnityEngine;

public class ActivatablePoster : ActivatableBaseObject, IRelaxationObject
{
    private const string MAIN_TEXTURE_NODE_NAME = "_BaseColorMap";

    private string filepath = string.Empty;
    private Material material;

    public float anxietyDecreaseAmount = 0.3f;

    public override string ActivateInfo
    {
        get
        {
            return "Change Image";
        }
    }

    public float AnxietyDecreaseAmount
    {
        get
        {
            return anxietyDecreaseAmount;
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
            texture2D.filterMode = FilterMode.Point;
        }
        material.SetTexture(MAIN_TEXTURE_NODE_NAME, texture2D);
        Slot.extraData = extraData;
    }
}