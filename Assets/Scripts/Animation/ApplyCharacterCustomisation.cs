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
    [SerializeField] private Texture2D hairBaseTexture = null;
    private Texture2D farmerBaseTexture;

    [Header("OutputBase Texture To Be Used For Animation")]
    [SerializeField] private Texture2D farmerBaseCustomised = null;
    [SerializeField] private Texture2D hairCustomised = null;
    private Texture2D farmerBaseShirtsUpdated;
    private Texture2D selectedShirt;

    // 用于在inspect界面选择shirt种类
    [Header("Select Shirt Style")]
    [Range(0,1)]
    [SerializeField] private int inputShirtStyleNo = 0;

    [Header("Select Gender: 0=Male, 1=Female")]
    [Range(0, 1)]
    [SerializeField] private int inputSex = 0;

    [Header("Select Color for Trouser")]
    [SerializeField] private Color inputTrouserColor = Color.blue;

    [Header("Select style and color for hair")]
    [Range(0, 2)]
    [SerializeField] private int inputHairStyleNo = 0;
    [SerializeField] private Color inputHairColor = Color.black;


    private Facing[,] bodyFacingArray;          // 2D array
    private Vector2Int[,] bodyShirtOffsetArray;

    #region Params for body-arm-shirt
    private int bodyRows = 21;
    private int bodyColumns = 6;
    private int farmerSpriteWidth = 16;
    private int farmerSpriteHeight = 32;
    // 这里的36是4*9，相当于四个方向，在高度方向上堆叠
    private int shirtTextureWidth = 9;
    private int shirtTextureHeight = 36;
    // Sprite代表最基本的一个shirt的尺寸
    private int shirtSpriteWidth = 9;
    private int shirtSpriteHeight = 9;
    private int shirtStyleInSpriteWidth = 16;                       //shirtsBaseTexture一行能容纳的shirt的数量
    // 原生的arm的三种颜色
    private Color32 armTargetColor1 = new Color32(77, 13, 13, 255);
    private Color32 armTargetColor2 = new Color32(138, 41, 41, 255);
    private Color32 armTargetColor3 = new Color32(172, 50, 50, 255);
    private List<colorSwap> colorSwapList;
    #endregion

    #region Params for hair
    private int hairTextureWidth = 16;
    private int hairTextureHeight = 96;
    private int hairStyleInSpriteWidth = 8;

    #endregion



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
        ProcessTrousers();

        ProcessHair();

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
        // 获取body的朝向与偏移量
        bodyFacingArray = new Facing[bodyColumns, bodyRows];
        PopulateBodyFacingArray();
        bodyShirtOffsetArray = new Vector2Int[bodyColumns,bodyRows];
        PopulateBodyShirtOffsetArray();
        // 获取shirt的Texture
        AddShirtToTexture(inputShirtStyleNo);

        ApplyShirtTextureToBase();
    }

    private void PopulateBodyFacingArray()
    {
        for(int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < 10; ++j)
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

    private void PopulateBodyShirtOffsetArray()
    {
        for(int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < 10; ++j)
            {
                bodyShirtOffsetArray[i, j] = new Vector2Int(99, 99);
            }
        }

        bodyShirtOffsetArray[0, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 10] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[3, 10] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 10] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 10] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[1, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 11] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[3, 11] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 11] = new Vector2Int(4, 11);
        bodyShirtOffsetArray[5, 11] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[1, 12] = new Vector2Int(3, 9);
        bodyShirtOffsetArray[2, 12] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[3, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[4, 12] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 12] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 13] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[3, 13] = new Vector2Int(5, 9);
        bodyShirtOffsetArray[4, 13] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[5, 13] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 14] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[2, 14] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 14] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[4, 14] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 14] = new Vector2Int(4, 12);

        bodyShirtOffsetArray[0, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[1, 15] = new Vector2Int(4, 5);
        bodyShirtOffsetArray[2, 15] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 15] = new Vector2Int(4, 12);
        bodyShirtOffsetArray[4, 15] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[5, 15] = new Vector2Int(4, 5);

        bodyShirtOffsetArray[0, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[1, 16] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[2, 16] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[3, 16] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[4, 16] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 16] = new Vector2Int(4, 10);

        bodyShirtOffsetArray[0, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[1, 17] = new Vector2Int(4, 8);
        bodyShirtOffsetArray[2, 17] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 17] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 17] = new Vector2Int(4, 7);
        bodyShirtOffsetArray[5, 17] = new Vector2Int(4, 8);

        bodyShirtOffsetArray[0, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 18] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 18] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 18] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 19] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 19] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 19] = new Vector2Int(4, 9);

        bodyShirtOffsetArray[0, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[1, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[2, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[3, 20] = new Vector2Int(4, 10);
        bodyShirtOffsetArray[4, 20] = new Vector2Int(4, 9);
        bodyShirtOffsetArray[5, 20] = new Vector2Int(4, 9);
    }

    /// <summary>
    /// 获取目标shirt的9*36的texture
    /// </summary>
    private void AddShirtToTexture(int shirtStyleNo)
    {
        // 创建一个空的Texture2D
        selectedShirt = new Texture2D(shirtTextureWidth, shirtTextureHeight);
        selectedShirt.filterMode = FilterMode.Point;
        // 寻找目标Texture在shirtsBaseTexture中的位置并获取Texture
        int y = (shirtStyleNo / shirtStyleInSpriteWidth) * shirtTextureHeight;
        int x = (shirtStyleNo % shirtStyleInSpriteWidth) * shirtTextureWidth;
        Color[] shirtPixels = shirtsBaseTexture.GetPixels(x, y, shirtTextureWidth, shirtTextureHeight);
        // 更新selectedShirt
        selectedShirt.SetPixels(shirtPixels);
        selectedShirt.Apply();
    }

    private void ApplyShirtTextureToBase()
    {
        //创建一个空的Texture，内容为body+目标shirt的大表
        farmerBaseShirtsUpdated = new Texture2D(farmerBaseTexture.width, farmerBaseTexture.height);
        farmerBaseShirtsUpdated.filterMode = FilterMode.Point;
        SetTextureToTransparent(farmerBaseShirtsUpdated);
        // 获取单个方向的目标shirt的Texture
        Color[] frontShirtPixels;
        Color[] backShirtPixels;
        Color[] rightShirtPixels;
        frontShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 3, shirtSpriteWidth, shirtSpriteHeight);
        backShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 0, shirtSpriteWidth, shirtSpriteHeight);
        rightShirtPixels = selectedShirt.GetPixels(0, shirtSpriteHeight * 2, shirtSpriteWidth, shirtSpriteHeight);

        for(int x = 0; x < bodyColumns; ++x)
        {
            for(int y = 0; y < bodyRows; ++y)
            {
                // 根据偏移量获取正确的pixelX，Y坐标
                int pixelX = x * farmerSpriteWidth;
                int pixelY = y * farmerSpriteHeight;
                if (bodyShirtOffsetArray[x, y] != null)
                {
                    if(bodyShirtOffsetArray[x,y].x==99&& bodyShirtOffsetArray[x, y].y == 99) { continue; }
                    pixelX += bodyShirtOffsetArray[x, y].x;
                    pixelY += bodyShirtOffsetArray[x, y].y;
                }
                // 根据朝向和pixelX，Y坐标，将对应的shirt Sprite绘制上去
                switch (bodyFacingArray[x, y])
                {
                    case Facing.none:
                        break;
                    case Facing.front:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, frontShirtPixels);
                        break;
                    case Facing.back:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, backShirtPixels);
                        break;
                    case Facing.right:
                        farmerBaseShirtsUpdated.SetPixels(pixelX, pixelY, shirtSpriteWidth, shirtSpriteHeight, rightShirtPixels);
                        break;
                    default:
                        break;
                }
            }
        }
        farmerBaseShirtsUpdated.Apply();
    }

    private void SetTextureToTransparent(Texture2D texture2D)
    {
        Color[] fill = new Color[texture2D.height * texture2D.width];
        for(int i = 0; i < fill.Length; ++i) { fill[i] = Color.clear; }
        texture2D.SetPixels(fill);
    }

    private void ProcessArms()
    {
        Color[] farmerPixelsToRecolour = farmerBaseTexture.GetPixels(0, 0, 288, farmerBaseTexture.height);
        PopulateArmColorSwapList();
        ChangePixelColors(farmerPixelsToRecolour, colorSwapList);
        farmerBaseCustomised.SetPixels(0, 0, 288, farmerBaseTexture.height, farmerPixelsToRecolour);
        farmerBaseCustomised.Apply();
    }


    private void PopulateArmColorSwapList()
    {
        colorSwapList.Clear();
        colorSwapList.Add(new colorSwap(armTargetColor1, selectedShirt.GetPixel(0, 7)));
        colorSwapList.Add(new colorSwap(armTargetColor2, selectedShirt.GetPixel(0, 6)));
        colorSwapList.Add(new colorSwap(armTargetColor3, selectedShirt.GetPixel(0, 5)));
    }

    private void ChangePixelColors(Color[] baseArray, List<colorSwap> colorSwapList)
    {
        for(int i = 0; i < baseArray.Length; ++i)
        {
            for(int j = 0; j < colorSwapList.Count; ++j)
            {
                if (isSameColor(baseArray[i], colorSwapList[j].fromColor))
                {
                    baseArray[i] = colorSwapList[j].toColor;
                }
            }
        }
    }

    // 可能由于材质颜色出了问题， 完全匹配的颜色失败，此处改成了abs的差值小于阈值的方法
    // r和a正确    b和g不正确   小数点后第7位进位出错
    //private bool isSameColor(Color color1, Color color2)
    //{
    //    if ((color1.r == color2.r) && (color1.g == color2.g) && (color1.b == color2.b) && (color1.a == color2.a))
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    private bool isSameColor(Color color1, Color color2)
    {
        if (Mathf.Abs(color1.r - color2.r) < 0.0001 && Mathf.Abs(color1.g - color2.g) < 0.0001 && Mathf.Abs(color1.b - color2.b) < 0.0001 && (color1.a == color2.a))
        {
            return true;
        }
        return false;
    }

    private void ProcessTrousers()
    {
        // 读取原生的Trouser的texture
        Color[] farmerTrouserPixels = farmerBaseTexture.GetPixels(288, 0, 96, farmerBaseTexture.height);
        // 根据目标的color对其进行颜色替换
        TintPixelColors(farmerTrouserPixels, inputTrouserColor);
        // 保存
        farmerBaseCustomised.SetPixels(288, 0, 96, farmerBaseTexture.height, farmerTrouserPixels);
        farmerBaseCustomised.Apply();
    }

    private void TintPixelColors(Color[] basePixelArray, Color tintColor)
    {
        for (int i = 0; i < basePixelArray.Length; ++i)
        {
            basePixelArray[i].r = basePixelArray[i].r * tintColor.r;
            basePixelArray[i].g = basePixelArray[i].g * tintColor.g;
            basePixelArray[i].b = basePixelArray[i].b * tintColor.b;
        }
    }

    private void ProcessHair()
    {
        // 找到对应样式的hairTexture，并保存到hairCustomised
        AddHairToTexture(inputHairStyleNo);
        // 修改颜色
        Color[] farmerSelectedHairPixels = hairCustomised.GetPixels();
        TintPixelColors(farmerSelectedHairPixels, inputHairColor);
        // 保存到hairCustomised
        hairCustomised.SetPixels(farmerSelectedHairPixels);
        hairCustomised.Apply();
    }

    private void AddHairToTexture(int hairStyleNo)
    {
        int y = (hairStyleNo / hairStyleInSpriteWidth) * hairTextureHeight;
        int x = (hairStyleNo % hairStyleInSpriteWidth) * hairTextureWidth;
        Color[] hairPixels = hairBaseTexture.GetPixels(x, y, hairTextureWidth, hairTextureHeight);
        hairCustomised.SetPixels(hairPixels);
        hairCustomised.Apply();
    }


    private void MergeCustomisations()
    {
        Color[] farmerShirtPixels = farmerBaseShirtsUpdated.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        // Trouser已经保存在了farmerBaseCustomised中
        Color[] farmerTrouserPixelsSelection = farmerBaseCustomised.GetPixels(288, 0, 96, farmerBaseTexture.height);
        // 临时变量，用于保存叠加shirt和裤子的body
        Color[] farmerBodyPixels = farmerBaseCustomised.GetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height);
        // 将裤子和shirt叠加到body上
        MergeColourArray(farmerBodyPixels, farmerTrouserPixelsSelection);
        MergeColourArray(farmerBodyPixels, farmerShirtPixels);
        // farmerBaseCustomised已经设置好了arms， 仅需把叠加了shirt和裤子的body替换过来即可
        farmerBaseCustomised.SetPixels(0, 0, bodyColumns * farmerSpriteWidth, farmerBaseTexture.height, farmerBodyPixels);
        farmerBaseCustomised.Apply();

    }

    private void MergeColourArray(Color[] baseArray, Color[] mergeArray)
    {
        for(int i = 0; i < baseArray.Length; ++i)
        {
            if (mergeArray[i].a > 0)
            {
                if (mergeArray[i].a >= 1)
                {
                    baseArray[i] = mergeArray[i];
                }
                else
                {
                    float alpha = mergeArray[i].a;
                    baseArray[i].r += (mergeArray[i].r - baseArray[i].r) * alpha;
                    baseArray[i].g += (mergeArray[i].g - baseArray[i].g) * alpha;
                    baseArray[i].b += (mergeArray[i].b - baseArray[i].b) * alpha;
                    baseArray[i].a += mergeArray[i].a;
                }
            }
        }
    }
}
