using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Painting : MonoBehaviour
{

    private RenderTexture texRender; //画布
    public Material mat; //给定的shader新建材质
    public Texture brushTypeTexture; //画笔纹理，半透明
                                     //public Texture clearBrush;//橡皮擦
                                     //private Camera mainCamera;
    private float brushScale = 0.5f; //当前
    public Color brushColor = Color.black;
    private RawImage rawImg; //使用UGUI的RawImage显示，方便进行添加UI,将pivot设为(0.5,0.5)
    [SerializeField] private CanvasGroup Msk; //遮罩

    // List<Vector2>[] linePoints = new List<Vector2>[10]; //每一条线上的点
    private int[] a = new int[10];
    private Vector2[,] PositionArray1 = new Vector2[4, 10];
    private int[] b = new int[10];
    private float[,] speedArray = new float[4, 10];
    private int[] s = new int[10];
    public int num = 50;
    //Vector2 rawImgSize; //该图片尺寸

    //Vector2 rawMousePosition; //raw图片的左下角对应鼠标位置
    float rawWidth; //raw图片宽度
    float rawHeight; //raw图片长度
    void Start()
    {

        //raw图片鼠标位置，宽度计算
        rawImg = GetComponent<RawImage>(); //获取本图片
        rawWidth = rawImg.rectTransform.sizeDelta.x;
        rawHeight = rawImg.rectTransform.sizeDelta.y;
        Vector2 rawanchorPositon = new Vector2(rawImg.rectTransform.anchoredPosition.x - rawImg.rectTransform.sizeDelta.x / 2.0f, rawImg.rectTransform.anchoredPosition.y - rawImg.rectTransform.sizeDelta.y / 2.0f);
        //rawMousePosition = rawanchorPositon + new Vector2 (Screen.width / 2.0f, Screen.height / 2.0f);
        //rawImgSize = rawImg.rectTransform.sizeDelta;//得到图片尺寸
        texRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        Clear(texRender);
        Input.multiTouchEnabled = true; //开启多点触控

    }

    //最多支持10点触控
    Vector2[] startPosition = new Vector2[10];
    Vector2[] endPosition = new Vector2[10];
    void Update()
    {
        //如果没有触控，则逐渐隐藏
        if (Input.touchCount == 0)
        {
            if (Msk.alpha < 0.05f)
            {
                Clear(texRender);
                Msk.alpha = 0.05f;
                return;
            }
            Msk.alpha = Mathf.Lerp(Msk.alpha, 0, Time.deltaTime);

            return;
        }
        //记录触控点
        int tCount = Input.touchCount;
        Touch[] inputTouches = Input.touches;

        //遍历每个点
        for (int i = 0; i <= tCount - 1; i++)
        {
            //-------------------开始触控---------------------------------
            if (inputTouches[i].phase == TouchPhase.Began)
            {
                //如果点击到了UI组件
                // if (EventSystem.current.IsPointerOverGameObject(inputTouches[i].fingerId))
                // {
                //     //print("点击到了按钮");
                //     continue;//终止本次循环
                // }

                if (Msk.alpha < 0.5f) Clear(texRender);
                Msk.alpha = 1.0f;
                //记录起始点
                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点
                a[fID] = 0;
                b[fID] = 0;
                s[fID] = 0;
                startPosition[fID] = inputTouches[i].position;
                endPosition[fID] = inputTouches[i].position;
                //记录经过的点
                //linePoints.Add(endPosition[fID]);
            }
            //-------------------悬停---------------------------------
            else if (inputTouches[i].phase == TouchPhase.Stationary)
            {
                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点
                startPosition[fID] = inputTouches[i].position;
                endPosition[fID] = inputTouches[i].position;
            }
            //-------------------移动---------------------------------
            else if (inputTouches[i].phase == TouchPhase.Moved)
            {
                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点
                endPosition[fID] = inputTouches[i].position;

                float distance = Vector3.Distance(endPosition[fID], startPosition[fID]);
                brushScale = SetScale(distance);
                //画出贝第i条塞尔曲线
                ThreeOrderBézierCurse(endPosition[fID], distance, 4.5f, fID);
                DrawImage();
            }

            //-------------------离开---------------------------------
            else if (inputTouches[i].phase == TouchPhase.Ended)
            {
                //当前手指下的线条消失
                //ThreeOrderBézierCurse(endPosition[i], distance, 4.5f, i)
                // OnMouseUp();

                //清除这些点//DrawBrush(texRender,new Rect(linePoints[l].x, linePoints[l].y, brushTypeTexture.width, brushTypeTexture.height),Texture.,new Color(0,0,0,0),5);

                int fID = inputTouches[i].fingerId; //利用FingerID来辨识触控点

                //重置点
                startPosition[fID] = Vector2.zero;
                endPosition[fID] = Vector2.zero;
                //brushScale = 0.5f;
                a[fID] = 0;
                b[fID] = 0;
                s[fID] = 0;
                //speedArray[i] = 0;

            }

        }

    }
    void DrawImage()
    {
        rawImg.texture = texRender;
    }
    public void OnClickClear()
    {
        Clear(texRender);
    }

    // void OnMouseUp()
    // {
    //     startPosition = Vector3.zero;
    //     //brushScale = 0.5f;
    //     a = 0;
    //     b = 0;
    //     s = 0;
    // }
    //设置画笔宽度
    float SetScale(float distance)
    {
        float Scale = 0;
        if (distance < 100)
        {
            Scale = 0.8f - 0.005f * distance;
        }
        else
        {
            Scale = 0.425f - 0.00125f * distance;
        }
        if (Scale <= 0.05f)
        {
            Scale = 0.05f;
        }
        return Scale;
    }

    // void DrawLine (Vector2 pos) {

    //     float distance = Vector3.Distance (pos, pos);
    //     brushScale = SetScale (distance);
    //     ThreeOrderBézierCurse (pos, distance, 4.5f);

    //     //startPosition = endPosition;
    //     lastDistance = distance;
    // }

    void Clear(RenderTexture destTexture) //画布清零
    {
        Graphics.SetRenderTarget(destTexture);
        GL.PushMatrix();
        GL.Clear(true, true, Color.white);
        GL.PopMatrix();
    }

    void DrawBrush(RenderTexture destTexture, Vector2 drawPoint, Texture sourceTexture, Color color, float scale)
    {
        DrawBrush(destTexture, new Rect(drawPoint.x, drawPoint.y, sourceTexture.width, sourceTexture.height),
            sourceTexture, color, scale);
    }
    void DrawBrush(RenderTexture destTexture, Rect destRect, Texture sourceTexture, Color color, float scale)
    {

        float left = destRect.xMin - destRect.width * scale / 2.0f;
        float right = destRect.xMin + destRect.width * scale / 2.0f;
        float top = destRect.yMin - destRect.height * scale / 2.0f;
        float bottom = destRect.yMin + destRect.height * scale / 2.0f;

        Graphics.SetRenderTarget(destTexture);//指定目标纹理图

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetTexture("_MainTex", sourceTexture);//设置画笔纹理
        mat.SetColor("_Color", color);//设置画笔颜色
        mat.SetPass(0);//画笔通道

        GL.Begin(GL.QUADS);//开始作画

        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(left / Screen.width, top / Screen.height, 0);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(right / Screen.width, top / Screen.height, 0);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(right / Screen.width, bottom / Screen.height, 0);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(left / Screen.width, bottom / Screen.height, 0);

        GL.End();
        GL.PopMatrix();
    }
    //bool bshow = true;

    void ClearPaint(Vector2 point, RenderTexture destTexture, Color color) //清除第i条线
    {
        float left = point.x - 32;
        float right = point.x + 32;
        float top = point.y - 32;
        float bottom = point.y + 32;

        Graphics.SetRenderTarget(destTexture);

        GL.PushMatrix();
        GL.LoadOrtho();

        //mat.SetTexture("_MainTex", clearBrush);
        mat.SetColor("_Color", color);
        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(left / Screen.width, top / Screen.height, 0);
        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(right / Screen.width, top / Screen.height, 0);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(right / Screen.width, bottom / Screen.height, 0);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(left / Screen.width, bottom / Screen.height, 0);

        GL.End();
        GL.PopMatrix();
    }

    //三阶贝塞尔曲线，获取连续4个点坐标，通过调整中间2点坐标，画出部分（我使用了num/1.5实现画出部分曲线）来使曲线平滑;通过速度控制曲线宽度。
    private void ThreeOrderBézierCurse(Vector3 pos, float distance, float targetPosOffset, int i)
    {
        //记录坐标
        PositionArray1[b[i], i] = pos;
        b[i]++;
        //记录速度
        speedArray[s[i], i] = distance;
        s[i]++;
        //暂存点
        Vector2 _point;

        if (b[i] == 4)
        {
            Vector2 temp1 = PositionArray1[1, i];
            Vector2 temp2 = PositionArray1[2, i];

            //修改中间两点坐标
            Vector2 middle = (PositionArray1[0, i] + PositionArray1[2, i]) / 2;
            PositionArray1[1, i] = (PositionArray1[1, i] - middle) * 1.5f + middle;
            middle = (temp1 + PositionArray1[3, i]) / 2;
            PositionArray1[2, i] = (PositionArray1[2, i] - middle) * 2.1f + middle;

            for (int index1 = 0; index1 < num / 1.5f; index1++)
            {
                float t1 = (1.0f / num) * index1;
                Vector3 target = Mathf.Pow(1 - t1, 3) * PositionArray1[0, i] +
                    3 * PositionArray1[1, i] * t1 * Mathf.Pow(1 - t1, 2) +
                    3 * PositionArray1[2, i] * t1 * t1 * (1 - t1) + PositionArray1[3, i] * Mathf.Pow(t1, 3);
                //float deltaspeed = (float)(distance - lastDistance) / num;
                //获取速度差值（存在问题，参考）
                float deltaspeed = (float)(speedArray[3, i] - speedArray[0, i]) / num;
                //float randomOffset = Random.Range(-1/(speedArray[0] + (deltaspeed * index1)), 1 / (speedArray[0] + (deltaspeed * index1)));
                //模拟毛刺效果
                float randomOffset = Random.Range(-targetPosOffset, targetPosOffset);
                _point = new Vector2((int)(target.x + randomOffset), (int)(target.y + randomOffset));
                DrawBrush(texRender, _point,
                brushTypeTexture, brushColor, SetScale(speedArray[0, i] + (deltaspeed * index1)));
            }

            PositionArray1[0, i] = temp1;
            PositionArray1[1, i] = temp2;
            PositionArray1[2, i] = PositionArray1[3, i];

            speedArray[0, i] = speedArray[1, i];
            speedArray[1, i] = speedArray[2, i];
            speedArray[2, i] = speedArray[3, i];
            b[i] = 3;
            s[i] = 3;
        }
        else
        {
            _point = new Vector2((int)pos.x, (int)pos.y);

            DrawBrush(texRender, _point, brushTypeTexture,
                brushColor, brushScale);
        }

    }
}