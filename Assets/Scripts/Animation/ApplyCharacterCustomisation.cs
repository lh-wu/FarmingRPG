using System;
using System.Collections.Generic;
using UnityEngine;


public class colorSwap
{
    public Color fromColor;
    public Color toColor;
    public colorSwap(Color fromColor, Color toColor)
    {
        this.fromColor = fromColor;
        this.toColor = toColor;
    }
}


public class ApplyCharacterCustomisation : MonoBehaviour
{
    [Header("Base Textures")]
    [SerializeField] private Texture2D maleFarmerBaseTexture = null;
    [SerializeField] private Texture2D femaleFarmerBaseTexture = null;
    [SerializeField] private Texture2D shirtsBaseTexture = null;
    private Texture2D farmerBaseTexture;

    [Header("OutputBase Texture To Be Used For Animation")]
    [SerializeField] private Texture2D farmerBaseCustomised = null;
    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D selectedShirt;

    // 用于在inspect界面选择shirt种类
    [Header("Select Shirt Style")]
    [Range(0,1)]
    [SerializeField] private int inputShirtStyleNo = 0;

    [Header("Select Gender: 0=Male, 1=Female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    private Facing[,] bodyFacingArray;          // 2D array
    private Vector2Int[,] bodyShirtOffsetArray;

    private int bodyRows = 21;
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;
    // 每个shirtTexture都是9*9的，这里的36是4*9，相当于四个方向
    private int shirtTextureWidth = 9;
    private int shirtTextureHeight = 36;
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    private int shirtStyleInSpriteWidth = 16;

    private List<colorSwap> colorSwapList;

    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);
    private Color32 armTargetColor2 = new Color32(138,41,41,255);
    private Color32 armTargetColor3 = new Color32(172,50,50,255);

    private void Awake()
    {
        colorSwapList = new List<colorSwap>();
        ProcessCustomisation();
    }

    private void ProcessCustomisation()
    {
        ProcessGender();
        ProcessShirt();
        ProcessArms();
        MergeCustomisations();

    }

    private void ProcessGender()
    {
        if (inputSex == 0) { farmerBaseTexture = maleFarmerBaseTexture; }
        else { farmerBaseTexture = femaleFarmerBaseTexture; }

        Color[] farmerBasePixels = farmerBaseTexture.GetPixels();

        farmerBaseCustomised.SetPixels(farmerBasePixels);
        farmerBaseCustomised.Apply();
    }

    private void ProcessShirt()
    {
        bodyFacingArray = new Facing[bodyColumns, bodyRows];

        PopulateBodyFacingArray();

        bodyShirtOffsetArray = new Vector2Int[bodyColumns,bodyRows];

        PopulateBodyShirtOffsetArray();

        AddShirtToTexture(inputShirtStyleNo);

        ApplyShirtTextureToBase();
    }

    private void PopulateBodyFacingArray()
    {
        for(int i = 0; i < 6; ++i)
        {
            for(int j = 0; j <= 9; ++j)
            {
                bodyFacingArray[i, j] = Facing.none;
            }
        }
        bodyFacingArray[0, 10] = Facing.back;
        bodyFacingArray[1, 10] = Facing.back;
        bodyFacingArray[2, 10] = Facing.right;
        bodyFacingArray[3, 10] = Facing.right;
        bodyFacingArray[4, 10] = Facing.right;
        bodyFacingArray[5, 10] = Facing.right;

        bodyFacingArray[0, 11] = Facing.front;
        bodyFacingArray[1, 11] = Facing.front;
        bodyFacingArray[2, 11] = Facing.front;
        bodyFacingArray[3, 11] = Facing.front;
        bodyFacingArray[4, 11] = Facing.back;
        bodyFacingArray[5, 11] = Facing.back;

        bodyFacingArray[0, 12] = Facing.back;
        bodyFacingArray[1, 12] = Facing.back;
        bodyFacingArray[2, 12] = Facing.right;
        bodyFacingArray[3, 12] = Facing.right;
        bodyFacingArray[4, 12] = Facing.right;
        bodyFacingArray[5, 12] = Facing.right;

        bodyFacingArray[0, 13] = Facing.front;
        bodyFacingArray[1, 13] = Facing.front;
        bodyFacingArray[2, 13] = Facing.front;
        bodyFacingArray[3, 13] = Facing.front;
        bodyFacingArray[4, 13] = Facing.back;
        bodyFacingArray[5, 13] = Facing.back;

        bodyFacingArray[0, 14] = Facing.back;
        bodyFacingArray[1, 14] = Facing.back;
        bodyFacingArray[2, 14] = Facing.right;
        bodyFacingArray[3, 14] = Facing.right;
        bodyFacingArray[4, 14] = Facing.right;
        bodyFacingArray[5, 14] = Facing.right;

        bodyFacingArray[0, 15] = Facing.front;
        bodyFacingArray[1, 15] = Facing.front;
        bodyFacingArray[2, 15] = Facing.front;
        bodyFacingArray[3, 15] = Facing.front;
        bodyFacingArray[4, 15] = Facing.back;
        bodyFacingArray[5, 15] = Facing.back;

        bodyFacingArray[0, 16] = Facing.back;
        bodyFacingArray[1, 16] = Facing.back;
        bodyFacingArray[2, 16] = Facing.right;
        bodyFacingArray[3, 16] = Facing.right;
        bodyFacingArray[4, 16] = Facing.right;
        bodyFacingArray[5, 16] = Facing.right;

        bodyFacingArray[0, 17] = Facing.front;
        bodyFacingArray[1, 17] = Facing.front;
        bodyFacingArray[2, 17] = Facing.front;
        bodyFacingArray[3, 17] = Facing.front;
        bodyFacingArray[4, 17] = Facing.back;
        bodyFacingArray[5, 17] = Facing.back;

        bodyFacingArray[0, 18] = Facing.back;
        bodyFacingArray[1, 18] = Facing.back;
        bodyFacingArray[2, 18] = Facing.back;
        bodyFacingArray[3, 18] = Facing.right;
        bodyFacingArray[4, 18] = Facing.right;
        bodyFacingArray[5, 18] = Facing.right;

        bodyFacingArray[0, 19] = Facing.right;
        bodyFacingArray[1, 19] = Facing.right;
        bodyFacingArray[2, 19] = Facing.right;
        bodyFacingArray[3, 19] = Facing.front;
        bodyFacingArray[4, 19] = Facing.front;
        bodyFacingArray[5, 19] = Facing.front;

        bodyFacingArray[0, 20] = Facing.front;
        bodyFacingArray[1, 20] = Facing.front;
        bodyFacingArray[2, 20] = Facing.front;
        bodyFacingArray[3, 20] = Facing.back;
        bodyFacingArray[4, 20] = Facing.back;
        bodyFacingArray[5, 20] = Facing.back;


    }
}
