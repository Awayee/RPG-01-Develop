using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System.Linq;
using System.Threading;
using Random = UnityEngine.Random;


//用于定位方向用
class DirectionUse
{
    public static Vector2 none = new Vector2(0, 0);
    public static Vector2 up = new Vector2(0, 1);
    public static Vector2 down = new Vector2(0, -1);
    public static Vector2 left = new Vector2(-1, 0);
    public static Vector2 right = new Vector2(1, 0);

    public static Vector2[] all = { up, down, left, right };
}
public class 随机生成自写版 : MonoBehaviour {
    
    //生成的房间数量
    public int RoomNum = 50;
    //对已经连接的房间和走廊再次连接，使迷宫不完美
    public int ReConnectNum = 10;
    //控制生成的房间大小
    public int RoomSize = 0;
    //控制迷宫的曲折程度
    public int windingPercent = 0;
    //地图的长和宽(必须为奇数)
    public int width = 51;
    public int height = 51;
    //用于绑定预制体，生成地图用
    public GameObject wall, floor;
    //用于在地图中生成玩家
    public GameObject player;
    //用于生成地图中的宝物
    public GameObject treasure;

    private Transform mapParent;
    
    //将生成位置有效的房间进行保存
    private List<Rect> ValidRooms;
    //将房间进行编号
    private int currentRegion = 0;
    // _regions存储各个区域的编号
    private int[,] _regions;

    //定义一个枚举类型，用于设置当前单元的类型（墙还是路）
    enum Types
    {
        Wall,
        Floor
    }

    private Types[,] map;


	// Use this for initialization
	void Start () {
		ValidRooms=new List<Rect>();//房间数组进行初始化
        map=new Types[width,height];
        _regions=new int[width,height];
        mapParent = GameObject.FindGameObjectWithTag("mapParent").transform;//获得tag为mapParent的物体
        Generate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //该函数用于触发生成算法的执行
    public void Generate()
    {
        //由于之后的填充迷宫的算法，要求迷宫的长宽都为奇数
        if (width % 2 == 0 || height % 2 == 0)
        {
            Debug.Log("地图长宽不能为偶数");
            return;
        }

        InitMap();//将迷宫全部初始化为墙
        AddRooms();//随机增加房间
        FillMaze();//寻找一个路径起点，并生成迷宫
        ConnectRegions();//将房间和通道进行连接
        RemoveDeadEnds();//将死路删除
        InstanceMaze();//实例化出地牢
    }

    /*将地图初始化为墙*/
    public void InitMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                map[i, j] = Types.Wall;
            }
        }
    }

    /*生成房间（大小在一定范围内随机，奇数），将不重叠的房间加入数组中*/
    public void AddRooms()
    {
        //这里的RoomNum指的是尝试的次数
        for (int i = 0; i <RoomNum; i++)
        {
            int size = Random.Range(1, 3 + RoomSize) * 2 + 1;//+1为了保证长宽是奇数
            int rectangularity = Random.Range(0, 1 + size / 2) * 2;
            int w = size, h = size;
            if (0 == Random.Range(0, 1))
            {
                w += rectangularity;
            }
            else
            {
                h += rectangularity;
            }
            /*确定绘画点的位置（此坐标位置与map的坐标一致）*/
            int x = Random.Range(0, (width - w) / 2) * 2 + 1;
            int y = Random.Range(0, (height - h) / 2) * 2 + 1;
            Rect room = new Rect(x, y, w, h);

            //判断房间是否出现重叠
            bool isOverLap = false;
            foreach (Rect r in ValidRooms)
            {
                if (room.Overlaps(r))
                {
                    isOverLap = true;
                    break;
                }
            }

            if (isOverLap)
            {
                continue;
            }
            //如果不重叠，加入到有效房间数组中
            ValidRooms.Add(room);
            StartRegion();

            for (int x1 = x;x1 < x+w; x1++)
            {
                for (int y1 = y; y1 < y+h; y1++)
                {
                    ResetType(new Vector2(x1,y1));//对于成为房间的位置，重设类型并编号
                }
            }
        }
    }
    //当前房间编号加加
    private void StartRegion()
    {
        currentRegion++;
    }

    //对当前区域进行编号,并重新设置类型
    private void ResetType(Vector2 pos, Types type = Types.Floor)
    {
        int x = (int)pos.x, y = (int)pos.y;
        map[x, y] = Types.Floor;//map数组用于记录类型

        _regions[x, y] = currentRegion;//_regions数组用于记录编号
    }

    //寻找一个迷宫的起点，并生成迷宫
    private void FillMaze()
    {
        //这个算法中判断的距离以2为一个元单位
        for (int x = 1; x < width; x+=2)
        {
            for (int y = 1; y < height; y+=2)
            {
                Vector2 pos=new Vector2(x,y);
                if (map[x,y]==Types.Wall)//如果该点不是房间
                {
                    GrowMaze(pos);//就以这个点为起点生成迷宫
                }
            }
        }
    }

    //以传入的点为起点开始生成迷宫
    private void GrowMaze(Vector2 start)
    {
        List<Vector2> cells=new List<Vector2>();
        Vector2 LastDir = Directions.none;//上一次生成的最后方向
        StartRegion();
        ResetType(start);//把起始点设置为floor类型
        cells.Add(start);


        while (cells != null && cells.Count != 0)//cells的会先从1加到最大，接着推出，知道数组变零，此时循环就结束了
        {
            Vector2 cell = cells[cells.Count - 1];//cell会一直保持在最新的一个
            //可以扩展的方向的集合
            List<Vector2> unmadeCells = new List<Vector2>();
            //加入能扩展迷宫的方向
            foreach (Vector2 dir in Directions.all)//遍历上下左右四个方向，将满足扩张条件的下一个cell添加到unmadeCells列表中
            {
                if (CanCarve(cell, dir))//传入的是当前最新的细胞和方向，如果下一个点能够连接
                {
                    unmadeCells.Add(dir);//将方向加入到unmadeCells列表中
                }
            }
            //unmadeCells.Count表示可以连接的方向个数
            if (unmadeCells != null && unmadeCells.Count != 0)//如果有可以连接的下一单元
            {
                Vector2 dir;
                //得到扩展方向 windingPercent用来控制是否为原方向
                if (unmadeCells.Contains(LastDir) && Random.Range(0, 100) > windingPercent)//windingPercent用于调整迷宫的曲折程度，取值（0，100），因为如果lastDir总是跟上一个一样，就会变成一条直线，不曲折了
                {
                    dir = LastDir;
                }
                else//如果跟上一个方向不一样，则任一方向都行
                {
                    dir = unmadeCells[Random.Range(0, unmadeCells.Count - 1)];
                }

                ResetType(cell + dir);//每次跳两个点遍历时不要忘记中间的点的type也变为floor
                ResetType(cell + dir * 2);
                //添加第二个单元
                cells.Add(cell + dir * 2);//将最新的那个点加入数组
                LastDir = dir;//保存上一个方向的选择
            }
            else
            {
                //没有相邻可以连接的单元了，就删除，后退一位
                cells.Remove(cells[cells.Count - 1]);
                //置空向量路径
                LastDir = Directions.none;
            }

        }
    }

    //dir是方向，pos是当前最新的像素点位置
    private bool CanCarve(Vector2 pos, Vector2 dir)
    {
        Vector2 temp = pos + 3 * dir;//因为位置是+2+2的添加下一个单元的，所以得判断+3位置的点
        int x = (int)temp.x, y = (int)temp.y;
        //判断是否超过边界
        if (x < 0 || x > width || y < 0 || y > height)
        {
            return false;
        }
        //需要判断方向第二个单元的原因是cells中需要添加下一个cell
        //所以下一个cell要变为Floor,然后需要判断是否第二个单元是否为墙
        //如果不为墙，则第一个cell被变为Floor为，和第二个单元就连通了，不可行
        //判断第二个单元主要用来判断不能＆其他房间或走廊（regions）连通

        temp = pos + 2 * dir;//此时的temp就是下一个点的坐标
        x = (int)temp.x;
        y = (int)temp.y;
        //是墙则能雕刻迷宫
        return map[x, y] == Types.Wall;//如果下一个点不是房间（既可以连，则返回真，否则返回否）
    }

    //连接房间和迷宫
    private void ConnectRegions()
    {
        //找到区域所有可连接的空间墙wall
        //dictionary的用法，下面vector2是key，而List<int>是值，比如到时候要用的话是List<int>  a=connectorRegions[(1.0)];会获得一个list<int>类型的数据
        Dictionary<Vector2, List<int>> connectorRegions = new Dictionary<Vector2, List<int>>();

        //遍历所有的点
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < height-1; j++)
            {
                if (map[i,j]!=Types.Wall)
                {
                    continue;
                  
                }
                //如果是墙的话
                //regions是用来计算这个点四周有多少也是房间或者迷宫的点（比如说如果某个点A周围4个点都是墙，则可以确定开通A点也是没办法连通房间和迷宫的）
                List<int> regions = new List<int>();
                foreach (Vector2 dir in Directions.all)
                {
                    int region = _regions[i + (int)dir.x, j + (int)dir.y];//获得这个单元上下左右单元的编号

                    //如果周围不是墙（墙的编号为0，因为还没有被编号）且还没添加到regions中
                    //加入regions中
                    //因为region是编号值，!regions.Contains(region)语句可以避免这个点接触的两个非墙面都是区域A
                    if (region != 0 && !regions.Contains(region))
                        regions.Add(region);
                }
                //如果regions.Count小于2，那么说明周围有3，4个是墙，证明连上这个点也没法连通房间和迷宫，那么就不是我们想要的点
                if (regions.Count < 2)
                    continue;

                connectorRegions[new Vector2(i, j)] = regions;//在i,j点记录下该点的list<int>，列表中都是该点中能连接的房间或者迷宫的编号
                //标志连接点
                //SetConnectCube(i,j);

            }
        }
        //所有连接点
        List<Vector2> connectors = connectorRegions.Keys.ToList<Vector2>();//connectorRegions.Keys获得connectorRegions中的所有key,这样connectors获得所有满足条件的点
        //跟踪哪些区域已合并。将区域索引映射为它已合并的区域索引。

        List<int> merged = new List<int>();
        List<int> openRegions = new List<int>();
        /*循环编号*/
        for (int i = 0; i <= currentRegion; i++)
        {
            merged.Add(i);
            openRegions.Add(i);
        }
        //使区域连接最终只剩下一个编号，既全部连通
        while (openRegions.Count > 1)
        {
            //随机选择一个连接点
            Vector2 connector = connectors[Random.Range(0, connectors.Count - 2)];
            //连接，即把连接点的type变为floor
            AddJunction(connector);


            //新建一个List<int>的regions用了记录已经连接过的区域编号
            List<int> regions = connectorRegions[connector];
            for (int i = 0; i < regions.Count; i++)
            {
                regions[i] = merged[regions[i]];
            }
            int dest = regions[0];//dest是第一个编号的值
            regions.RemoveAt(0);
            List<int> sources = regions;
            //合并所有受影响的区域

            //这一块循环的作用，比方说，连接了1和4区域，那么就把区域4的编号变为1，把他们看成一个整体
            for (int i = 0; i < currentRegion; i++)
            {
                if (sources.Contains(merged[i]))
                {
                    merged[i] = dest;
                }
            }



            //移除已经连接的区域
            foreach (int s in sources)
            {
                openRegions.RemoveAll(value => (value == s));
            }
            connectors.RemoveAll(index => IsRemove(merged, connectorRegions, connector, index));//将与这两个连接区域有关的点都移除
        }
    }


    //进行连接
    private void AddJunction(Vector2 pos)
    {
        map[(int)pos.x, (int)pos.y] = Types.Floor;
    }



    //删除不需要的连接点
    private bool IsRemove(List<int> merged, Dictionary<Vector2, List<int>> ConnectRegions, Vector2 connector, Vector2 pos)
    {
        //不让连接器相连（包括斜向相连）
        if ((connector - pos).SqrMagnitude() < 2)
        {
            return true;
        }
        List<int> temp = ConnectRegions[pos];
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i] = merged[temp[i]];
        }
        HashSet<int> set = new HashSet<int>(temp);
        //判断连接点是否和两个区域相邻，不然移除
        if (set.Count > 1)
        {
            return false;
        }
        //增加连接，使得地图连接不是单连通的
        if (Random.Range(0, ReConnectNum) == 0) AddJunction(pos);
        return true;
    }




    /*简化迷宫*/
    private void RemoveDeadEnds()
    {
        bool done = false;
        while (!done)
        {
            done = true;
            /*遍历整个地图*/
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    if (map[i, j] == Types.Wall)
                        continue;
                    /*找到非墙的点*/
                    int exists = 0;
                    foreach (Vector2 dir in Directions.all)
                    {
                        if (map[i + (int)dir.x, j + (int)dir.y] != Types.Wall)//计算该点四周有多少迷宫或房间区域
                        {
                            exists++;
                        }
                    }
                    //如果exists==1则是三面环墙
                    //如果exists != 1，证明不是死路，继续下次循环
                    if (exists != 1)
                    {
                        continue;
                    }
                    /*如果是三面环墙，则是死路*/
                    done = false;
                    _regions[i, j] = 0;//重新变为变成墙
                    map[i, j] = Types.Wall;
                }
            }
        }
    }



    //生成地牢
    private void InstanceMaze()
    {
        bool isPlayer = true;//用于判断玩家是否已生成
        bool isTreasure = true;//设置要生成的宝物数量
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] == Types.Floor)
                {
                    GameObject go = Instantiate(floor, new Vector3(i, 1, j), Quaternion.identity) as GameObject;
                    go.transform.SetParent(mapParent);
                    //设置层级
                    //go.layer = LayerMask.NameToLayer("floor");


                    if (isPlayer && _regions[i, j] == 1)//如果是第一个房间
                    {
                        GameObject player1 = Instantiate(player, new Vector3(i, 2, j), Quaternion.identity) as GameObject;
                        player1.transform.SetParent(mapParent);
                        //player1.layer = LayerMask.NameToLayer("player");
                        isPlayer = false;
                    }


                    if (isTreasure && _regions[i, j] == currentRegion-1)//如果是最后一个房间
                    {
                        GameObject treasures = Instantiate(treasure, new Vector3(i, 2, j), Quaternion.identity) as GameObject;
                        treasures.transform.SetParent(mapParent);
                        //treasures.layer = LayerMask.NameToLayer("player");
                        isTreasure=false;

                    }

                    
                }
                else if (map[i, j] == Types.Wall)
                {
                    GameObject go = Instantiate(wall, new Vector3(i, 1, j), Quaternion.identity) as GameObject;
                    go.transform.SetParent(mapParent);
                    //go.layer = LayerMask.NameToLayer("wall");
                }
            }
        }
    }
}
