using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instantiateOBjects : MonoBehaviour
{
    public int roomNum=1;//保存房间的数量

    private GameObject room;//获取主物体
    public GameObject zoulang1; //绑定走廊1的预制体
    public GameObject zoulang2; //绑定走廊2的预制体
    public GameObject zoulang3; //绑定走廊3的预制体
    public GameObject room1; //绑定房间1的预制体
    public GameObject room2; //绑定房间2的预制体
    public GameObject room3; //绑定房间3的预制体
    public GameObject connect1; //绑定连接1的预制体
    public GameObject connect2; //绑定连接2的预制体
    public GameObject connect3; //绑定连接3的预制体
    public GameObject qiang;//绑定墙的预制体

    public GameObject Treasure1;//绑定财宝1的预制体
    public GameObject Treasure2;//绑定财宝2的预制体
    public GameObject Treasure3;//绑定财宝3的预制体

    public GameObject Lamp1;//绑定灯1的预制体
    public GameObject Lamp2;//绑定灯2的预制体
    public GameObject Lamp3;//绑定灯3的预制体

    public GameObject Table;//绑定桌子的预制体
    public GameObject BOOK;//绑定桌子上书的预制体

    private GameObject[] AllObs;//获取场景中所有物件


    private int i; //获取随机数
    private int j;


    private Transform roomMark; //房间的mark
    private Transform zoulangMark; //走廊的mark
    private int roomMarkNum; //获取房间的mark序号
    private int zoulangMarkNum; //获取走廊的mark序号

    int count; //获取主物块的子物体数
    int count1; //获取副物块的子物体数
    

    private int xunhuan; //获得循环生成的次数

    // Use this for initialization
    void Start()
    {

        /*主物块*/
        room = GameObject.FindGameObjectWithTag("room");
        initialCreate();
        
        /*迭代进行建立*/
        while(roomNum!=3)//当生成的房间少于3时,一直生成
        {
            FindNextOb();//寻找下一个要连接的类型
            NormalInstantiate();
        }


        //检测出所有没用使用的标记，封上墙
        FengQiang();

        //接着在地图上随机生成财宝
        CreateTreasure();


    }
    
    // Update is called once per frame
    void Update()
    {

    }
    void initialCreate()//用于场景第一个场景的建立
    {
        
        
        /*通过随机方式获得需要连接的mark*/
        count = room.transform.childCount;
        /*遍历所有的mark,如果符合条件，就加入数组中*/
        var MarkList = new List<int>();
        
        for (j = 1; j <20; j++)
        {
            if (room.transform.Find("标记" + j)!=null)
            {
                if (room.transform.Find("标记" + j).tag == "true")
            {
                MarkList.Add(j); //将标记数加入列表中
            }
            }
            
        }
        /*遍历主物体所有还未使用的mark*/
        xunhuan = MarkList.Count;
        for (int k = 1; k <= xunhuan; k++)
        {
            MarkList = new List<int>();
            for (j = 1; j < 20; j++)
            {
                if (room.transform.Find("标记" + j)!=null)
                {
                     if (room.transform.Find("标记" + j).tag == "true")
                {
                    MarkList.Add(j); //将标记数加入列表中
                }
                }
               
            }

            if (MarkList.Count > 1) //如果数组还有多个元素，进行随机
            {
                roomMarkNum = Random.Range(0, MarkList.Count); //随机获得主物块要连接的mark
            }

            else //如果数组只剩一个元素，选择剩下的这个元素
            {
                roomMarkNum = 0;
            }

   
            roomMark = room.transform.Find("标记" + MarkList[roomMarkNum]); //所选择的主物块的mark
            roomMark.tag = "false"; //将选择到的主物块的tag置为false


            /*获得想要使用的走廊*/
            MarkList = new List<int>(); //每次要使用前将LIST重置

            GameObject connectSelect = zoulang1;
            i = Random.Range(1, 4); //随机一个数
            if (i == 1)
            {
                connectSelect = zoulang1; //连接走廊1
            }
            else if (i == 2)
            {
                connectSelect = zoulang2; //连接走廊2
            }
            else if (i == 3)
            {
                connectSelect = zoulang3; //连接走廊3
            }

            GameObject instance = Instantiate(connectSelect, transform.position, Quaternion.Euler(0, 0, 0)); //实例化走廊
            count1 = instance.transform.childCount; //计算走廊所拥有的mark数



            for (j = 1; j < 20; j++)
            {
                if (instance.transform.Find("标记" + j)!=null)
                {
                    if (instance.transform.Find("标记" + j).tag == "true")
                {
                    MarkList.Add(j); //将标记数加入列表中
                }
                }
                
            }

            if (MarkList.Count > 1) //如果数组还有多个元素，进行随机
            {
                zoulangMarkNum = Random.Range(0, MarkList.Count); //随机获得副物块要连接的mark
            }
            else //如果数组只剩一个元素，选择剩下的这个元素
            {
                zoulangMarkNum = 0;
            }

            zoulangMark = instance.transform.Find("标记" + MarkList[zoulangMarkNum]); //副物块的mark
            zoulangMark.tag = "false"; //副物块的mark置为false
            MarkList = new List<int>();




            Vector3 differences =
                roomMark.rotation.eulerAngles - zoulangMark.rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
            /*根据mark的旋转量进行旋转*/
            differences = new Vector3(differences.x, 180 + differences.y, differences.z);
            instance.transform.Rotate(differences);
            /*进行相对的位移调整位置*/
            Vector3 move = zoulangMark.position - roomMark.position;
            instance.transform.position = instance.transform.position - move;
        }
        
    }
    //用于找到下一个主物体
    void FindNextOb()
    {
        AllObs = FindObjectsOfType<GameObject>();

        var SceneObs = new List<GameObject>();//获得场景中符合作为主物件的物件集合

        /*进行遍历，将满足条件的物件放到list中*/
        foreach (GameObject i in AllObs)
        {
            if (i.transform.tag == "room1" || i.transform.tag == "room2" || i.transform.tag == "room3" || i.transform.tag == "zoulang1" || i.transform.tag == "zoulang2" || i.transform.tag == "zoulang3" || i.transform.tag == "connect1" || i.transform.tag == "connect2" || i.transform.tag == "connect3")
            {
                SceneObs.Add(i);
            }
        }
        /*获得下一个主物体*/
        int num = Random.Range(0, SceneObs.Count);
        /*赋给主物块*/
        room = SceneObs[num];

    }
    //常规的生成，具有一定的匹配规则
    void NormalInstantiate()
    {
        /*通过随机方式获得需要连接的mark*/
        count = room.transform.childCount;
        /*遍历所有的mark,如果符合条件，就加入数组中*/
        var MarkList = new List<int>();

        if (room.tag=="room1"|| room.tag == "room2" || room.tag == "room3" ||room.tag=="room")
        {
            for (j = 1; j < 20; j++)
            {
                if (room.transform.Find("标记" + j)!=null)
                {
                     if (room.transform.Find("标记" + j).tag == "true")
                {
                    MarkList.Add(j); //将标记数加入列表中
                }
                }
               
            }
        }
        else if (room.tag == "zoulang1" || room.tag == "zoulang2" || room.tag == "zoulang3" )
        {
            for (j = 1; j < 20; j++)
            {
                if (room.transform.Find("标记" + j) != null)
                {
                    if (room.transform.Find("标记" + j).tag == "true")
                {
                    MarkList.Add(j); //将标记数加入列表中
                }
                }
                
            }
        }
        else
        {
            for (j = 1; j < count ; j++)
            {
                if (room.transform.Find("标记" + j).tag == "true")
                {
                    MarkList.Add(j); //将标记数加入列表中
                }
            }
        }
        
        /*遍历主物体所有还未使用的mark*/
        xunhuan = MarkList.Count;
        for (int k = 1; k <= xunhuan; k++)
        {
            MarkList = new List<int>();
            if (room.tag == "room1" || room.tag == "room2" || room.tag == "room3"||room.tag=="room")
            {
                for (j = 1; j < 20; j++)
                {
                    if (room.transform.Find("标记" + j)!=null)
                    {
                        if (room.transform.Find("标记" + j).tag == "true")
                    {
                        MarkList.Add(j); //将标记数加入列表中
                    }
                    }
                    
                }
            }
            else if (room.tag == "zoulang1" || room.tag == "zoulang2" || room.tag == "zoulang3")
            {
                for (j = 1; j < 20; j++)
                {
                    if (room.transform.Find("标记" + j) != null)
                    {
                        if (room.transform.Find("标记" + j).tag == "true")
                    {
                        MarkList.Add(j); //将标记数加入列表中
                    }
                    }
                    
                }

            }
            
            else
            {
                for (j = 1; j < count; j++)
                {
                    if (room.transform.Find("标记" + j).tag == "true")
                    {
                        MarkList.Add(j); //将标记数加入列表中
                    }
                }
            }

            if (MarkList.Count > 1) //如果数组还有多个元素，进行随机
            {
                roomMarkNum = Random.Range(0, MarkList.Count); //随机获得主物块要连接的mark
            }

            else //如果数组只剩一个元素，选择剩下的这个元素
            {
                roomMarkNum = 0;
            }

            roomMark = room.transform.Find("标记" + MarkList[roomMarkNum]); //所选择的主物块的mark
            roomMark.tag = "false"; //将选择到的主物块的tag置为false


            /*如果主物体是房间，只能连接走廊*/
            if (room.tag=="room1"|| room.tag == "room2"|| room.tag == "room3") { 
            /*获得想要使用的走廊*/
            MarkList = new List<int>(); //每次要使用前将LIST重置

            GameObject connectSelect = zoulang1;
            i = Random.Range(1, 4); //随机一个数
            if (i == 1)
            {
                connectSelect = zoulang1; //连接走廊1
            }
            else if (i == 2)
            {
                connectSelect = zoulang2; //连接走廊2
            }
            else if (i == 3)
            {
                connectSelect = zoulang3; //连接走廊3
            }

            GameObject instance = Instantiate(connectSelect, transform.position, Quaternion.Euler(0, 0, 0)); //实例化走廊
            count1 = instance.transform.childCount; //计算走廊所拥有的mark数



            for (j = 1; j < 20; j++)
            {
                if (instance.transform.Find("标记" + j) != null)
                {
                    if (instance.transform.Find("标记" + j).tag == "true")
                {
                    MarkList.Add(j); //将标记数加入列表中
                }
                }
                
            }

            if (MarkList.Count > 1) //如果数组还有多个元素，进行随机
            {
                zoulangMarkNum = Random.Range(0, MarkList.Count); //随机获得副物块要连接的mark
            }
            else //如果数组只剩一个元素，选择剩下的这个元素
            {
                zoulangMarkNum = 0;
            }

            zoulangMark = instance.transform.Find("标记" + MarkList[zoulangMarkNum]); //副物块的mark
            zoulangMark.tag = "false"; //副物块的mark置为false
            MarkList = new List<int>();




            Vector3 differences =
                roomMark.rotation.eulerAngles - zoulangMark.rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
            /*根据mark的旋转量进行旋转*/
            differences = new Vector3(differences.x, 180 + differences.y, differences.z);
            instance.transform.Rotate(differences);
            /*进行相对的位移调整位置*/
            Vector3 move = zoulangMark.position - roomMark.position;
            instance.transform.position = instance.transform.position - move;
            }

            /*如果主物体是连接处，只能连接走廊*/
            if (room.tag == "connect1" || room.tag == "connect2" || room.tag == "connect3")
            {
                /*获得想要使用的走廊*/
                MarkList = new List<int>(); //每次要使用前将LIST重置

                GameObject connectSelect = zoulang1;
                i = Random.Range(1, 4); //随机一个数
                if (i == 1)
                {
                    connectSelect = zoulang1; //连接走廊1
                }
                else if (i == 2)
                {
                    connectSelect = zoulang2; //连接走廊2
                }
                else if (i == 3)
                {
                    connectSelect = zoulang3; //连接走廊3
                }

                GameObject instance = Instantiate(connectSelect, transform.position, Quaternion.Euler(0, 0, 0)); //实例化走廊
                count1 = instance.transform.childCount; //计算走廊所拥有的mark数



                for (j = 1; j <20; j++)
                {
                    if (instance.transform.Find("标记" + j) != null)
                    {
                        if (instance.transform.Find("标记" + j).tag == "true")
                    {
                        MarkList.Add(j); //将标记数加入列表中
                    }
                    }
                    
                }

                if (MarkList.Count > 1) //如果数组还有多个元素，进行随机
                {
                    zoulangMarkNum = Random.Range(0, MarkList.Count); //随机获得副物块要连接的mark
                }
                else //如果数组只剩一个元素，选择剩下的这个元素
                {
                    zoulangMarkNum = 0;
                }

                zoulangMark = instance.transform.Find("标记" + MarkList[zoulangMarkNum]); //副物块的mark
                zoulangMark.tag = "false"; //副物块的mark置为false
                MarkList = new List<int>();




                Vector3 differences =
                    roomMark.rotation.eulerAngles - zoulangMark.rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                                                                                      /*根据mark的旋转量进行旋转*/
                differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                instance.transform.Rotate(differences);
                /*进行相对的位移调整位置*/
                Vector3 move = zoulangMark.position - roomMark.position;
                instance.transform.position = instance.transform.position - move;
            }



            /*如果是走廊，生成房间或者连接处*/
            if (room.tag == "zoulang1" || room.tag == "zoulang2" || room.tag == "zoulang3")
            {
                /*获得想要使用的走廊*/
                MarkList = new List<int>(); //每次要使用前将LIST重置
                GameObject connectSelect = room1;
                /*进行随机，如果结果是1，就生成房间，如果是2，就生成连接*/
                int panduan = Random.Range(0,2);
                if (panduan==0)
                {
                    roomNum++;
                i = Random.Range(1, 4); //随机一个数
                if (i == 1)
                {
                    connectSelect = room1; //连接走廊1
                }
                else if (i == 2)
                {
                    connectSelect = room2; //连接走廊2
                }
                else if (i == 3)
                {
                    connectSelect = room3; //连接走廊3
                }
                }

                if (panduan == 1)
                {
                    i = Random.Range(1, 4); //随机一个数
                    if (i == 1)
                    {
                        connectSelect = connect1; //连接走廊1
                    }
                    else if (i == 2)
                    {
                        connectSelect = connect2; //连接走廊2
                    }
                    else if (i == 3)
                    {
                        connectSelect = connect3; //连接走廊3
                    }
                }

                GameObject instance = Instantiate(connectSelect, transform.position, Quaternion.Euler(0, 0, 0)); //实例化走廊
                count1 = instance.transform.childCount; //计算走廊所拥有的mark数


                if (instance.tag=="room1"|| instance.tag == "room2"|| instance.tag == "room3" || instance.tag == "room")
                {
                    for (j = 1; j < 20; j++)
                    {
                        if (instance.transform.Find("标记" + j)!=null)
                        {
                            if (instance.transform.Find("标记" + j).tag == "true")
                        {
                            MarkList.Add(j); //将标记数加入列表中
                        }
                        }
                        
                    }
                }
                else
                {
                    for (j = 1; j < count1; j++)
                    {
                        if (instance.transform.Find("标记" + j).tag == "true")
                        {
                            MarkList.Add(j); //将标记数加入列表中
                        }
                    }
                }

                if (MarkList.Count > 1) //如果数组还有多个元素，进行随机
                {
                    zoulangMarkNum = Random.Range(0, MarkList.Count); //随机获得副物块要连接的mark
                }
                else //如果数组只剩一个元素，选择剩下的这个元素
                {
                    zoulangMarkNum = 0;
                }

  
                zoulangMark = instance.transform.Find("标记" + MarkList[zoulangMarkNum]); //副物块的mark
                zoulangMark.tag = "false"; //副物块的mark置为false
                MarkList = new List<int>();




                Vector3 differences =
                    roomMark.rotation.eulerAngles - zoulangMark.rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                                                                                      /*根据mark的旋转量进行旋转*/
                differences = new Vector3(differences.x, 180 + differences.y, differences.z);

                instance.transform.Rotate(differences);
                /*进行相对的位移调整位置*/
                Vector3 move = zoulangMark.position - roomMark.position;
                instance.transform.position = instance.transform.position - move;

            }




        }
    }
    //将没有使用的标记用墙封住
    void FengQiang()
    {
        Debug.Log("执行封墙代码");
        GameObject[] Allroom3;
        GameObject[] Allroom1;
        GameObject[] Allroom2;
        GameObject[] Allzoulang1;
        GameObject[] Allzoulang2;
        GameObject[] Allzoulang3;
        GameObject[] Allconnect1;
        GameObject[] Allconnect2;
        GameObject[] Allconnect3;

        Allroom3 = GameObject.FindGameObjectsWithTag("room3");
        Allroom1 = GameObject.FindGameObjectsWithTag("room1");
        Allroom2 = GameObject.FindGameObjectsWithTag("room2");
        Allzoulang1 = GameObject.FindGameObjectsWithTag("zoulang1");
        Allzoulang2 = GameObject.FindGameObjectsWithTag("zoulang2");
        Allzoulang3 = GameObject.FindGameObjectsWithTag("zoulang3");
        Allconnect1 = GameObject.FindGameObjectsWithTag("connect1");
        Allconnect2 = GameObject.FindGameObjectsWithTag("connect2");
        Allconnect3 = GameObject.FindGameObjectsWithTag("connect3");

        //遍历所有的物件，判断是否有true的标记，有就封墙
        foreach (var tamp in Allroom1)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i)!=null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                {
                    num.Add(i);//添加进列表中
                    tamp.transform.Find("标记" + i).tag = "false";
                    wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                    //进行旋转调整
                    Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                    /*根据mark的旋转量进行旋转*/
                    differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                    wall[wall.Count-1].transform.GetChild(0).Rotate(differences);
                }
                }
                

                
            }
for (int k = 0; k < num.Count; k++)
                {
                    wall[k].transform.SetParent(tamp.transform);
                    wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition+new Vector3(0,0.8f,0);
                }
        }
        foreach (var tamp in Allroom2)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {

                if (tamp.transform.Find("标记" + i)!=null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                {
                    num.Add(i);//添加进列表中
                    tamp.transform.Find("标记" + i).tag = "false";
                    wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);



                    //进行旋转调整
                    Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                    /*根据mark的旋转量进行旋转*/
                    differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                    wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                }
                }
                


            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 0.8f, 0);
            }
        }
        foreach (var tamp in Allroom3)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i)!=null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                {
                    num.Add(i);//添加进列表中
                    tamp.transform.Find("标记" + i).tag = "false";
                    wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                    //进行旋转调整
                    Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                    /*根据mark的旋转量进行旋转*/
                    differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                    wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                }
                }
                


            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition+new Vector3(0, 0.8f, 0);
            }
        }




        foreach (var tamp in Allzoulang1)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i) != null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                    {
                        num.Add(i);//添加进列表中
                        tamp.transform.Find("标记" + i).tag = "false";
                        wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                        //进行旋转调整
                        Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                        /*根据mark的旋转量进行旋转*/
                        differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                        wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                    }
                }



            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 2f, 0);
            }
        }
        foreach (var tamp in Allzoulang2)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i) != null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                    {
                        num.Add(i);//添加进列表中
                        tamp.transform.Find("标记" + i).tag = "false";
                        wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                        //进行旋转调整
                        Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                        /*根据mark的旋转量进行旋转*/
                        differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                        wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                    }
                }



            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 0.8f, 0);
            }
        }
        foreach (var tamp in Allzoulang3)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i) != null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                    {
                        num.Add(i);//添加进列表中
                        tamp.transform.Find("标记" + i).tag = "false";
                        wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                        //进行旋转调整
                        Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                        /*根据mark的旋转量进行旋转*/
                        differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                        wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                    }
                }



            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 0.8f, 0);
            }
        }


        foreach (var tamp in Allconnect1)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i) != null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                    {
                        num.Add(i);//添加进列表中
                        tamp.transform.Find("标记" + i).tag = "false";
                        wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                        //进行旋转调整
                        Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                        /*根据mark的旋转量进行旋转*/
                        differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                        wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                    }
                }



            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 0.8f, 0);
            }
        }
        foreach (var tamp in Allconnect2)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i) != null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                    {
                        num.Add(i);//添加进列表中
                        tamp.transform.Find("标记" + i).tag = "false";
                        wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                        //进行旋转调整
                        Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                        /*根据mark的旋转量进行旋转*/
                        differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                        wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                    }
                }



            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 0.8f, 0);
            }
        }
        foreach (var tamp in Allconnect3)
        {
            List<GameObject> wall = new List<GameObject>();
            List<int> num = new List<int>();//用于记录标记号
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("标记" + i) != null)
                {
                    if (tamp.transform.Find("标记" + i).tag == "true")
                    {
                        num.Add(i);//添加进列表中
                        tamp.transform.Find("标记" + i).tag = "false";
                        wall.Add(Instantiate(qiang, transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);

                        //进行旋转调整
                        Vector3 differences = tamp.transform.Find("标记" + i).transform.rotation.eulerAngles - wall[wall.Count - 1].transform.GetChild(0).rotation.eulerAngles; //比对两个将要连接的版块的mark的旋转量
                        /*根据mark的旋转量进行旋转*/
                        differences = new Vector3(differences.x, 180 + differences.y, differences.z);
                        wall[wall.Count - 1].transform.GetChild(0).Rotate(differences);
                    }
                }



            }
            for (int k = 0; k < num.Count; k++)
            {
                wall[k].transform.SetParent(tamp.transform);
                wall[k].transform.localPosition = tamp.transform.Find("标记" + num[k]).localPosition + new Vector3(0, 0.8f, 0);
            }
        }

    }
    //生成财宝
    void CreateTreasure()
    {
        //找到所有的房间
        GameObject[] Allroom3;
        GameObject[] Allroom1;
        GameObject[] Allroom2;
        GameObject[] Allzoulang1;
        GameObject[] Allzoulang2;
        GameObject[] Allzoulang3;
        Allroom3 = GameObject.FindGameObjectsWithTag("room3");
        Allroom1 = GameObject.FindGameObjectsWithTag("room1");
        Allroom2 = GameObject.FindGameObjectsWithTag("room2");
        Allzoulang1 = GameObject.FindGameObjectsWithTag("zoulang1");
        Allzoulang2 = GameObject.FindGameObjectsWithTag("zoulang2");
        Allzoulang3 = GameObject.FindGameObjectsWithTag("zoulang3");
        bool isBook = true;

        //遍历所有的房间
        foreach (var tamp in Allroom1)
        {
            if (tamp.transform.Find("桌标记").tag=="Table")
                {
                    tamp.transform.Find("桌标记").tag = "false";
                    GameObject table=Instantiate(Table, tamp.transform.Find("桌标记").position, tamp.transform.Find("桌标记").rotation);
                    if (isBook)
                    {
                        isBook = false;
                        Instantiate(BOOK, table.transform.Find("卡牌生成处").position, Quaternion.identity);
                }

                   
                   
                }
            for (int i = 1; i < 20; i++)
            {
                
                if (tamp.transform.Find("宝物标记" + i)!=null)
                {

                    if (tamp.transform.Find("宝物标记" + i).tag == "Treasure")
                    {

                        int num = Random.Range(1, 4);
                        GameObject tres;//用于实例化财宝
                        switch (num)
                        {
                            case 1:
                                tres = Instantiate(Treasure1, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                            case 2:
                                tres = Instantiate(Treasure2, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                            case 3:
                                tres = Instantiate(Treasure3, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                        }
                    }
                }
               
            }

        }

        foreach (var tamp in Allroom2)
        {
            if (tamp.transform.Find("桌标记").tag == "Table")
            {
                tamp.transform.Find("桌标记").tag = "false";
                Instantiate(Table, tamp.transform.Find("桌标记").position, tamp.transform.Find("桌标记").rotation);
            }

            for (int i = 1; i < 20; i++)
            {
                
                
                if (tamp.transform.Find("宝物标记" + i) != null)
                {
                    if (tamp.transform.Find("宝物标记" + i).tag == "Treasure")
                    {

                        int num = Random.Range(1, 4);
                        GameObject tres;//用于实例化财宝
                        switch (num)
                        {
                            case 1:
                                tres = Instantiate(Treasure1, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                            case 2:
                                tres = Instantiate(Treasure2, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                            case 3:
                                tres = Instantiate(Treasure3, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                        }
                    }
                }

            }

        }
        foreach (var tamp in Allroom3)
        {
            if (tamp.transform.Find("桌标记").tag == "Table")
            {
                tamp.transform.Find("桌标记").tag = "false";
                Instantiate(Table, tamp.transform.Find("桌标记").position, tamp.transform.Find("桌标记").rotation);
            }
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("宝物标记" + i) != null)
                {
                    if (tamp.transform.Find("宝物标记" + i).tag == "Treasure")
                    {

                        int num = Random.Range(1, 4);
                        GameObject tres;//用于实例化财宝
                        switch (num)
                        {
                            case 1:
                                tres = Instantiate(Treasure1, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                            case 2:
                                tres = Instantiate(Treasure2, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                            case 3:
                                tres = Instantiate(Treasure3, tamp.transform.Find("宝物标记" + i).position, tamp.transform.Find("宝物标记" + i).rotation);
                                break;
                        }
                    }
                }

            }

        }


        //遍历所有的走廊
        foreach (var tamp in Allzoulang1)
        {
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("灯标记" + i) != null)
                {
                    if (tamp.transform.Find("灯标记" + i).tag == "Lamp")
                    {

                        int num = Random.Range(1, 4);
                        GameObject tres;//用于实例化财宝
                        switch (num)
                        {
                            case 1:
                                tres = Instantiate(Lamp1, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                            case 2:
                                tres = Instantiate(Lamp2, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                            case 3:
                                tres = Instantiate(Lamp3, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                        }

                    }
                }
            }
        }

        foreach (var tamp in Allzoulang2)
        {
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("灯标记" + i) != null)
                {
                    if (tamp.transform.Find("灯标记" + i).tag == "Lamp")
                    {

                        int num = Random.Range(1, 4);
                        GameObject tres;//用于实例化财宝
                        switch (num)
                        {
                            case 1:
                                tres = Instantiate(Lamp1, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                            case 2:
                                tres = Instantiate(Lamp2, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                            case 3:
                                tres = Instantiate(Lamp3, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                        }

                    }
                }
            }
        }


        foreach (var tamp in Allzoulang3)
        {
            for (int i = 1; i < 20; i++)
            {
                if (tamp.transform.Find("灯标记" + i) != null)
                {
                    if (tamp.transform.Find("灯标记" + i).tag == "Lamp")
                    {

                        int num = Random.Range(1, 4);
                        GameObject tres;//用于实例化财宝
                        switch (num)
                        {
                            case 1:
                                tres = Instantiate(Lamp1, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                            case 2:
                                tres = Instantiate(Lamp2, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                            case 3:
                                tres = Instantiate(Lamp3, tamp.transform.Find("灯标记" + i).position, tamp.transform.Find("灯标记" + i).rotation);
                                break;
                        }

                    }
                }
            }
        }

    }
}