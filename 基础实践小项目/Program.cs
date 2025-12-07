using System.Xml.Serialization;

namespace 基础实践小项目
{

    //要点：
    //在发现要使用多次重复数据集合的时候申明结构体
    //在一个东西有很多类型或状态的时候使用枚举


    //使用枚举来表示不同格子类型
    /// <summary>
    /// 各自类型枚举
    /// </summary>
    enum E_Grid_Type
    {
        /// <summary>
        /// 普通格子
        /// </summary>
        Normal,
        /// <summary>
        /// 炸弹
        /// </summary>
        Boom,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 时空隧道
        /// </summary>
        Tunnel,

    }

    /// <summary>
    /// 玩家类型枚举
    /// </summary>
    enum E_Player_Type
    {
        /// <summary>
        /// 玩家
        /// </summary>
        Player,
        /// <summary>
        /// 电脑
        /// </summary>
        Computer,
    }


    //使用结构体来描述棋盘格子：
    //格子：不同样式，坐标信息
    //应构造函数初始化
    //画格子的方法
    /// <summary>
    /// 位置信息结构体
    /// </summary>
    struct Vector2
    {
        public int x;
        public int y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Grid
    {
        //格子的类型
        public E_Grid_Type type;
        //格子的位置
        public Vector2 position;

        //初始化构造函数
        public Grid( int x , int y ,E_Grid_Type type)
        {
            this.type = type;
            position.x = x;//参数没有和结构体内变量重名所以可以不用写this
            position.y = y;
        }

        //直接用结构体内变量做的一些行为
        public void Draw()
        {
            //提出来的目的 就是少写几行代码 引文他们不管哪种类型 都要设置了位置再画
            Console.SetCursorPosition(position.x, position.y);
            switch (type)
            {
                //普通格子怎么画
                case E_Grid_Type.Normal:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("□");
                    break;
                //炸弹怎么画
                case E_Grid_Type.Boom:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("●");
                    break;
                //暂停怎么画
                case E_Grid_Type.Pause:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("‖");
                    break;
                //时空隧道怎么画
                case E_Grid_Type.Tunnel:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("¤");
                    break;

            }
        }

    }


    //地图结构体：
    //格子数组
    //地图初始化构造函数
    //画地图方法

    struct Map
    {
        public Grid[] grids;


        //初始化中 初始了各个 格子的类型 和位置

        //构造函数
        public Map(int x , int y, int num)//思考要从外面传进来哪些东西  1.起始的坐标  2.格子的个数
        {
            grids = new Grid[num];

            Random r = new Random();
            int randomNum;
            //用于位置改变计数的变量
            //表示x变化的次数
            int indexX = 0;
            //表示y变化的次数
            int indexY = 0;

            //由于x方向画要+-变化
            //设置一个x步长：每次x变化多少
            int stepNum = 2;

            //画地图逻辑
            for (int i = 0; i < num; i++)
            {
                //应该初始化 格子类型
                randomNum = r.Next(0 ,101);


                //设置类型  普通格子
                //遇到随机数加条件分支判断则计算该条件条件概率就是此条件分支的概率
                //如果要控制条件分支的概率也用此方式
                //随机数0~100，取任意一个数的概率相同，则取数<85的概率为85%
                //有85%几率是普通格子（首尾两个格子 必为普通格子）
                if(randomNum < 85 || i ==0 || i==num-1)
                {
                    grids[i].type = E_Grid_Type.Normal;
                }
                //有5%的几率 是炸弹
                else if(randomNum >=85 && randomNum <= 90)
                {
                    grids[i].type = E_Grid_Type.Boom;
                }
                //有5%的几率 是暂停
                else if (randomNum >= 90 && randomNum <= 95)
                {
                    grids[i].type = E_Grid_Type.Pause;
                }
                //有5%的几率 是时空隧道
                else
                {
                    grids[i].type = E_Grid_Type.Tunnel;
                }

                //位置应该如何设置
                grids[i].position = new Vector2(x,y);
                //每次循环都应该按一定规则去变化位置
                //起始位置是x，y

                //indexX每加10次拐弯
                //每次循环按规定的规则改变x ，y的值赋值给下一个 格子结构体的position
                //实现给每个格子赋值上应有的位置值
                if(indexX == 10)
                {
                    y += 1;
                    ++indexY;
                    if( indexY == 2)
                    {
                        //y加了两次过后 ，把indexX加的次数记为0
                        indexX = 0;
                        indexY = 0;
                        //反向步长
                        stepNum = -stepNum;
                    }

                }
                else
                {
                    x += stepNum;
                    ++indexX;
                }

            }
        }

        public void Draw()
        {
            for(int i =0;i< grids.Length; i++)
            {
                grids[i].Draw();
            }
        }
    }



    //玩家结构体：
    //玩家类型（电脑还是玩家）
    //当前所在格子位置
    //玩家初始化构造函数
    //画自己的方法

    struct Player
    {
        //玩家类型
        public E_Player_Type type;

        //位置信息 当前所在地图哪一个索引的格子 

        public int nowIndex;//存当前在地图格子的下标

        //是否暂停的标识
        public bool isPause;


        public Player (int index,E_Player_Type type)
        {
            this.type = type;
            nowIndex = index;
        }


        public void Draw( Map mapInfo)//必须要先得到地图才能够 得到我在地图上的哪一个格子

        {
            //从传入的地图中 得到 格子信息
            Grid grid = mapInfo.grids[nowIndex];

            //设置位置
            Console.SetCursorPosition(grid.position.x, grid.position.y);

            //画 设置 颜色 设置图标
            switch(type)
            {
                case E_Player_Type.Player:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("★");
                    break;
                case E_Player_Type.Computer:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("▲");
                    break;
            }

        }

    }




    /// <summary>
    /// 游戏场景枚举类型
    /// </summary>
    enum E_SceneType
    {
        /// <summary>
        /// 开始场景
        /// </summary>
        Begin,
        /// <summary>
        /// 游戏场景
        /// </summary>
        Game,
        /// <summary>
        /// 结束场景
        /// </summary>
        End,
    }
    internal class Program
    {

        //设置一个函数来绘制玩家和电脑

        static void DrawPlayer(Player player,Player computer,Map map)
        {
            //重合时
            if(player.nowIndex == computer.nowIndex)
            {
                //首先得到重合的位置
                Grid grid = map.grids[player.nowIndex];
                Console.SetCursorPosition(grid.position.x,grid.position.y);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("◎");
            }
            //不重合的时候  就调用自己结构体 的 画自己函数
            else
            {
                player.Draw(map);
                computer.Draw(map);
            }

        }

        static void Main(string[] args)
        {
            //1.需求分析
            //理清小项目的功能需求
            //整理流程图
            //学习面向对象过后 ————UML类图


            //2.定义函数实现控制台初始化
            int width = 50;
            int height = 30;
            ConsoleInit(width , height);

            //3.多个场景
            //申明一个 标识场景标识的变量
            E_SceneType nowSceneType = E_SceneType.Begin;
            while (true)
            {

                switch (nowSceneType)
                {
                    case E_SceneType.Begin:
                        //开始场景逻辑
                        Console.Clear();
                        BeginScene(width,height,ref nowSceneType);//光标放到函数上按F12跳转到对应函数
                        Console.Clear();
                        break;
                    case E_SceneType.Game:
                        //游戏场景逻辑                  
                        GameScene(width,height,ref nowSceneType);
                        break;
                    case E_SceneType.End:
                        //结束场景逻辑
                        Console.Clear();
                        BeginScene(width, height, ref nowSceneType);//光标放到函数上按F12跳转到对应函数
                        Console.Clear();
                        break;
                    default:
                        break;
                }

            }


        }
        /// <summary>
        /// 控制台初始化
        /// </summary>
        static void ConsoleInit(int width ,int height)
        {            
            //控制台基础设置
            //光标的隐藏
            Console.CursorVisible = false;
            //舞台的大小
            Console.SetWindowSize(width , height);
            Console.SetBufferSize(width , height);

        }
        /// <summary>
        /// 开始场景逻辑
        /// </summary>
        static void BeginScene(int width,int height,ref E_SceneType nowSceneType)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(nowSceneType ==E_SceneType.Begin ? width/2 - 3 :width/2-4,8);//光标的位置取决于下面绘制的内容需要在的位置
            Console.Write(nowSceneType == E_SceneType.Begin ? "飞行棋" : "游戏结束");
            //开始场景逻辑处理循环

            //当前选项的编号
            int nowSelIndex = 0;
            bool isQuitBegin = false;
            while(true)
            {
                Console.SetCursorPosition(width / 2 - 4, 13);
                Console.ForegroundColor = nowSelIndex == 0 ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write(nowSceneType == E_SceneType.Begin ? "开始游戏":"回到主菜单");//合理利用复制粘贴

                Console.SetCursorPosition(width / 2 - 4, 15);
                Console.ForegroundColor = nowSelIndex == 1 ? ConsoleColor.Red : ConsoleColor.White;
                //ConsoleColor也是系统自带的枚举类型
                Console.Write("退出游戏");

                // 通过 ReadKey().Key (注意不是keychar) 可以 得到是一个的ConsoleKey枚举类型：对应的枚举项名 和 键入 键 一致, 这个枚举里有所有键盘按键的枚举项 且不区分大小写
                //枚举中不能有同名枚举项但是可以也可以有不同名的枚举项但常量值相同
                //即可以同值但不可同名
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        --nowSelIndex;
                        if(nowSelIndex < 0)
                        {
                            nowSelIndex = 0;
                        }
                            break;
                    case ConsoleKey.S:
                        ++nowSelIndex;
                        if(nowSelIndex > 1)
                        {
                            nowSelIndex = 1;
                        }
                        break;
                    case ConsoleKey.J:
                        if(nowSelIndex ==0)
                        {
                            //进入游戏场景
                            nowSceneType = nowSceneType == E_SceneType.Begin ? E_SceneType.Game :E_SceneType.Begin;
                            isQuitBegin = true;
                        }
                        else
                        {
                            //退出游戏
                            //直接关闭控制台
                            Environment.Exit(0);
                            
                        }
                            break;
                }

                if(isQuitBegin)
                {
                    break;
                }

            }
        }
        

        /// <summary>
        /// 游戏场景逻辑
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="nowSceneType"></param>
        static void GameScene(int width ,int height ,ref E_SceneType nowSceneType)
        {
            //绘制必需的基本信息
            DrawWall(width,height);
            //绘制地图
            //初始化一张地图
            Map map = new Map(14, 3, 80);
            map.Draw();
            //绘制玩家
            Player player = new Player(0,E_Player_Type.Player);
            Player computer =new Player(0,E_Player_Type.Computer);
            DrawPlayer(player, computer,map);

            bool isGameOver = false;

            //游戏逻辑部分
            while (true)
            {
                //之后的逻辑


                //玩家扔骰子的逻辑

                //检测输入
                Console.ReadKey(true);

                //扔骰子的逻辑
                isGameOver=RandomMove(width,height,ref player,ref computer ,map);
                //绘制地图;这里绘制是覆盖掉上次玩家的图像
                map.Draw();
                //绘制玩家
                DrawPlayer(player, computer,map);
                //判断是否要结束游戏
                if (isGameOver)
                {
                    //卡住程序 让玩家按任意键
                    Console.ReadKey();
                    //改变场景ID
                    nowSceneType= E_SceneType.End;
                    //直接跳出循环
                    break;
                }
                //电脑扔骰子的逻辑

                //检测输入
                Console.ReadKey(true);

                //扔骰子的逻辑
                isGameOver=RandomMove(width, height, ref computer, ref player, map);

                //绘制地图
                map.Draw();

                //绘制玩家
                DrawPlayer(player, computer, map);

                //判断是否要结束游戏
                if (isGameOver)
                {
                    //卡住程序 让玩家按任意键
                    Console.ReadKey();
                    //改变场景ID
                    nowSceneType = E_SceneType.End;
                    //直接跳出循环
                    break;
                }

            }



        }


        //擦除提示的函数
        static void ClearInfo(int w,int h)
        {
            //擦除之前显示的提示信息
            Console.SetCursorPosition(2, h - 5);
            Console.Write("                                       ");
            Console.SetCursorPosition(2, h - 4);
            Console.Write("                                       ");
            Console.SetCursorPosition(2, h - 3); 
            Console.Write("                                       ");
            Console.SetCursorPosition(2, h - 2);
            Console.Write("                                       ");
              
        }


        /// <summary>
        /// 扔骰子函数
        /// </summary>
        /// <param name="w">窗口的宽</param>
        /// <param name="h">窗口的高</param>
        /// <param name="p">扔骰子的对象</param>
        /// <param name="map">地图信息</param>
        /// <returns>默认返回false 代表游戏有没有结束 </returns>
        static bool RandomMove(int w,int h, ref Player p, ref Player otherP,Map map)//按F2改名字这个变量的所有位置都会变
        {
            //擦除之前显示的提示信息
            ClearInfo(w,h);

            //根据扔骰子的玩家类型 决定信息的颜色
            Console.ForegroundColor = p.type == E_Player_Type.Player ? ConsoleColor.Cyan : ConsoleColor.Magenta;

            //扔骰子之前 判断 玩家是否处于暂停状态
            if (p.isPause)
            {
                Console.SetCursorPosition(2,h - 5);
                Console.Write("处于暂停点,{0}需要暂停一回合", p.type == E_Player_Type.Player ? "你" : "电脑");
                //停止暂停
                p.isPause = false;
                return false;
            }
            else
            {
                //扔骰子目的 是改变 玩家或者电脑的位置 计算位置的变化


                //扔骰子 随机一个1~6的数 加上去
                Random r = new Random();
                int randomNum = r.Next(1, 7);
                p.nowIndex += randomNum;

                //打印扔的点数
                Console.SetCursorPosition(2, h - 5);
                Console.Write("{0}扔出的点数为{1}：", p.type == E_Player_Type.Player ? "你":"电脑",randomNum);

                //首先判断是否到终点
                //到了终点了
                if (p.nowIndex >= map.grids.Length - 1)
                {

                    p.nowIndex = map.grids.Length - 1;
                    Console.SetCursorPosition(2, h - 4);     
                    if(p.type == E_Player_Type.Player)
                    {
                        Console.Write("恭喜你，你率先到达了终点");
                    }
                    else
                    {
                        Console.Write("很遗憾，电脑率先到达了终点");

                    }
                    Console.SetCursorPosition (2, h - 3);
                    Console.Write("请按任意键结束游戏");
                    return true;
                }
                //没有到终点 就判断 当前对象 到了一个怎么样的格子
                else
                {
                    Grid grid = map.grids[p.nowIndex];

                    switch (grid.type)
                    {
                        case E_Grid_Type.Normal:
                            //普通格子不同处理
                            Console.SetCursorPosition(2, h - 4);
                            Console.Write("{0}到了一个安全位置", p.type == E_Player_Type.Player ? "你" : "电脑");
                            Console.SetCursorPosition(2, h - 4);
                            Console.Write("请按任意键，让{0}开始扔骰子", p.type == E_Player_Type.Player ? "电脑" : "你");
                            break;
                        case E_Grid_Type.Boom:
                            //炸弹退格
                            p.nowIndex -= 5;
                            //不能比起点还小
                            if (p.nowIndex < 0)
                            {
                                p.nowIndex = 0;
                            }
                            Console.SetCursorPosition(2, h - 4);
                            Console.Write("{0}踩到了炸弹，退后5格", p.type == E_Player_Type.Player ? "你" : "电脑");
                            Console.SetCursorPosition(2, h - 4);
                            Console.Write("请按任意键，让{0}开始扔骰子", p.type == E_Player_Type.Player ? "电脑" : "你");

                            break;
                        case E_Grid_Type.Pause:
                            //暂停一回合
                            p.isPause=true;
                            //暂停目前 只有加一个对象的暂停标识 才知道 下一回合它是不是不能扔骰子
                            break;
                        case E_Grid_Type.Tunnel:
                            Console.SetCursorPosition(2, h - 4);
                            Console.Write("{0}踩到了时空隧道", p.type == E_Player_Type.Player ? "你" : "电脑");
                            Console.SetCursorPosition(2, h - 4);
                            Console.Write("请按任意键，让{0}开始扔骰子", p.type == E_Player_Type.Player ? "电脑" : "你");

                            //随机
                            randomNum = r.Next(1, 91);
                            //触发倒退
                            if(randomNum <= 30)
                            {
                                p.nowIndex -= 5;
                                if(p.nowIndex < 0)
                                {
                                    p.nowIndex=0;
                                }
                                Console.SetCursorPosition(2, h - 3);
                                Console.Write("触发倒退5格");

                            }
                            //触发暂停
                            else if( randomNum <= 60 )//可以不用写>30原原因这在<=30分支的后面
                            {
                                p.isPause = false;
                                Console.SetCursorPosition(2, h - 3);
                                Console.Write("触发暂停一回合");

                            }
                            //触发换位置
                            else
                            {
                                int temp =p.nowIndex;
                                p.nowIndex = otherP.nowIndex;
                                otherP.nowIndex = temp;
                                Console.SetCursorPosition(2, h - 3);
                                Console.Write("惊喜，惊喜，双方交换位置");

                            }
                            Console.SetCursorPosition(2, h - 2);
                            Console.Write("请按任意键，让{0}开始扔骰子", p.type == E_Player_Type.Player ? "你" : "电脑");


                            break;

                    }
                }

            }

            //默认没有结束
            return false;
        }


        /// <summary>
        /// 绘制不变的内容：红墙和文字说明信息
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        static void DrawWall(int width ,int height)
        {
            
            Console.ForegroundColor = ConsoleColor.Red;
            //画墙
            //横着的墙
            for(int i = 0; i < width; i+=2)
            {
                //最上方的墙
                Console.SetCursorPosition(i,0);
                Console.Write("■");

                //最下方的墙
                Console.SetCursorPosition(i, height - 1);
                Console.Write("■");

                //中间的墙
                Console.SetCursorPosition(i, height - 6);
                Console.Write("■");
                Console.SetCursorPosition(i, height - 11);
                Console.Write("■");


            }
            //竖着的墙
            for (int i = 0;i < height; i++)
            {
                //最左边的墙
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                //最右边的墙
                Console.SetCursorPosition(width-2, i);
                Console.Write("■");


            }

            //文字信息
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2, height - 10);
            Console.Write("□:普通格子");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition(2, height - 9);
            Console.Write("‖:暂停，一回合不动");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(26, height - 9);
            Console.Write("●:炸弹，倒退5格");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(2, height - 8);
            Console.Write("¤:时空隧道，随机倒退，暂停，换位置");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(2, height - 7);
            Console.Write("★:玩家");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(14, height - 7);
            Console.Write("▲:电脑");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition(26, height - 7);
            Console.Write("◎:玩家和电脑重合");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(2 ,  height-5);
            Console.Write("按任意键开始扔骰子");

        }

    }
}
//C#基础实践————————飞行棋总结
//控制台基础设置
//多个场景
//开始场景逻辑实现
//游戏场景逻辑实现  1.不变的红墙 2.格子类型枚举和格子结构体 3.地图结构体 4.玩家和电脑结构体 5。扔骰子
//结束场景逻辑实现
//使用函数来抽象行为和逻辑：如投骰子，绘制地图和玩家等等
//使用结构体和枚举来描述一个对象的数据集合