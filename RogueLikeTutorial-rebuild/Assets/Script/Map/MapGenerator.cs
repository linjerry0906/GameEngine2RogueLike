//-----------------------------------------------
// 制作者：林　佳叡
// 制作日：2018.11.23
// 内容：昔のコードをUnityで再現、DungeonGenerator
//-----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockDef
{
    Wall = 0,
    Space = 1,
    Entry = 2,
    Exit = 3,
}

public class MapGenerator 
{
	private enum GenerateState
    {
        GenerateRoom,       //部屋を乱数生成
        Discrete,           //部屋を離散する
        SelectMainRoom,     //メインの部屋を確保
        LinkRoom,           //メインの部屋を接続する
        CreateHall,         //通路を生成
        ChooseSubRoom,      //通路上のサブ部屋を追加
        CheckMapSize,       //マップサイズを確定
        WriteToArray,       //書き出し
        SetEventPoint,      //入口や出口などの設置
        ReleaseData,        //要らないデータを削除
        End,                //処理完了
    }

    private int dungeonSize;            //マップの大きさ
    private int limitHeight;            //正規分布の縦の最大長さ
    private int limitWidth;             //正規文武の横の最大長さ

	private readonly int MAX_ROOM_SIZE = 4;
    private readonly int MIN_ROOM_SIZE = 1;
    private int minXRoomIndex = 0;      //マップのスタート点X用
    private int maxXRoomIndex = 0;      //マップのエンド点X用
    private int minZRoomIndex = 0;      //マップのスタート点Z用
    private int maxZRoomIndex = 0;      //マップのエンド点Z用

    private List<DungeonRoom> rooms;        //全部の部屋
    private List<DungeonRoom> mainRoom;     //メインの部屋
    private List<DungeonRoom> halls;        //通路
    private List<Edge> edges;           //接続計算用の辺

    private int[,] mapChip;             //マップチップ

    private GenerateState currentState;     //現在の生成状態

	public MapGenerator(int dungeonSize)
    {
        rooms = new List<DungeonRoom>();
        mainRoom = new List<DungeonRoom>();
        halls = new List<DungeonRoom>();
        edges = new List<Edge>();
        mapChip = new int[1,1];
        this.dungeonSize = dungeonSize;       //ダンジョンのサイズ

        //正規分布の楕円形の縦と横(集約させるためにさらに2を割る)
        limitWidth = (Random.Range(dungeonSize / 4, dungeonSize * 3 / 4)) / 2;
        limitHeight = (dungeonSize - limitWidth) / 2;

        currentState = GenerateState.GenerateRoom;      //生成状態
    }

	private void Release()
    {
        rooms.Clear();
        mainRoom.Clear();
        halls.Clear();
        edges.Clear();
    }

	private Vector2Int RandomPointInCircle(float width, float height)
        {
            //任意の円の角度（ラジアン）
            float t = 2 * Mathf.PI * Random.value;
            //半径決定（単位円内に集約）
            float u = Random.value + Random.value;
            float r = (u > 1) ? 2 - u: u;
            //単位円内の点×指定の縦、横
            return new Vector2Int((int)(width * r * Mathf.Cos(t)), (int)(height * r * Mathf.Sin(t)));
        }

	public void Update()
    {
        switch (currentState)
        {
            case GenerateState.GenerateRoom:        //部屋生成
                UpdateGenerate();
                break;
            case GenerateState.Discrete:            //部屋離散
                UpdateDiscrete();
                break;
            case GenerateState.SelectMainRoom:      //メイン部屋を選択
                UpdateSelectMainRoom();
                break;
            case GenerateState.LinkRoom:            //メイン部屋を接続
                UpdateLinkRoom();
                break;
            case GenerateState.CreateHall:          //廊下を生成
                UpdateCreateHall();
                break;
            case GenerateState.ChooseSubRoom:       //サブの部屋を選択
                UpdateChooseSubRoom();
                break;
            case GenerateState.CheckMapSize:        //マップサイズを確定
                UpdateCheckMapSize();
                break;
            case GenerateState.WriteToArray:        //マップチップ生成
                UpdateWriteToArray();
                break;
            case GenerateState.SetEventPoint:       //ランダムで特殊なマスを設置
                UpdateSetEventPoint();
                break;
            case GenerateState.ReleaseData:
                UpdateRelease();
                break;
            case GenerateState.End:
                break;
        }
    }

	private void UpdateGenerate()
	{
		for(int i = 0; i < dungeonSize; ++i)
		{
			Vector2Int pos = RandomPointInCircle(limitWidth, limitHeight);
			DungeonRoom room = 
				new DungeonRoom(
					i, 
					Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE) * 2,
					Random.Range(MIN_ROOM_SIZE, MAX_ROOM_SIZE) * 2,
					pos);
			rooms.Add(room);
		}

        currentState = GenerateState.Discrete;      //部屋を離散する
	}

	private void UpdateDiscrete()
	{
		int counter = 0;        //部屋が重なる数
		for(int i = 0; i < rooms.Count; ++i)
		{
			for(int j = 0; j < rooms.Count; ++j)
			{
				if(i == j || !rooms[i].RoomCollision(rooms[j]))
					continue;
				++counter;
				rooms[i].Hit(rooms[j]);
				rooms[j].Hit(rooms[i]);
			}
		}
        if (counter <= 0)       //全部修正完了すると次の段階へ移行
        {
            currentState = GenerateState.SelectMainRoom;
        }
	}

	private void UpdateSelectMainRoom()
	{
		for(int i = 0; i < rooms.Count; ++i)
		{
			//縦横サイズが指定より大きい場合
            if (rooms[i].Length > (int)(MAX_ROOM_SIZE * 2 * 0.65f) &&
                rooms[i].Width > (int)(MAX_ROOM_SIZE * 2 * 0.65f))
            {
                    mainRoom.Add(rooms[i]);            //リストに追加
            }
		}

		if (mainRoom.Count < 4)             //量が少なすぎるとやり直し
        {
            currentState = GenerateState.GenerateRoom;
            Release();
            return;
        }

        currentState = GenerateState.LinkRoom;      //次の段階へ移行
	}
	
	private void UpdateLinkRoom()
	{
		for (int i = 0; i < mainRoom.Count - 1; ++i)
        {
            int minIndex = i + 1;   //残りの部屋と一番近い部屋の番号
            for (int j = i + 1; j < mainRoom.Count; ++j)        //残りの部屋との距離判定
            {
                //今記録している部屋との距離より近い場合
                if ((mainRoom[i].Pos- mainRoom[minIndex].Pos).sqrMagnitude
                    > (mainRoom[i].Pos - mainRoom[j].Pos).sqrMagnitude)
                {
                    minIndex = j;   							//部屋番号を指定する
                }
            }
            //繋ぐ線を追加
            Edge edge = new Edge(mainRoom[i].Pos, mainRoom[minIndex].Pos);
            if (!edges.Contains(edge))
            {
                edges.Add(edge);
            }
        }
        currentState = GenerateState.CreateHall;    //次の段階へ移行
	}

	private void UpdateCreateHall()
	{
		for(int i = 0; i < edges.Count; ++i)
		{
			Edge e = edges[i];
			Vector2Int center = new Vector2Int(
                (e.Start.x + e.End.x) / 2,
                (e.Start.y + e.End.y) / 2);

			DungeonRoom hall = new DungeonRoom(
				halls.Count,
				Mathf.Abs(e.Start.x - e.End.x) + 2,
				2,
				new Vector2Int(center.x, e.End.y)
			);
			halls.Add(hall);

			hall = new DungeonRoom(
				halls.Count,
				2,
				Mathf.Abs(e.Start.y - e.End.y) + 2,
				new Vector2Int(e.Start.x, center.y)
			);
			halls.Add(hall);
		}
        currentState = GenerateState.ChooseSubRoom;     //次の段階へ移行
	}

	private void UpdateChooseSubRoom()
	{
		for(int h = 0; h < halls.Count; ++h)
		{
			for(int d = 0; d < rooms.Count; ++d)
			{
				if (mainRoom.Contains(rooms[d]))   		//メインの部屋は判定しない
                        continue;
                //当たっていたらメインに追加
                if (halls[h].RoomCollision(rooms[d]) && 
                    (rooms[d].Length > MAX_ROOM_SIZE * 2 * 0.3f || rooms[d].Width >MAX_ROOM_SIZE * 2 * 0.3f))
                {
                    mainRoom.Add(rooms[d]);
                }
			}
		}
		currentState = GenerateState.CheckMapSize;
	}

	private void UpdateCheckMapSize()
	{
		minXRoomIndex = mainRoom[0].ID;     //最大最小値を先頭に設定
        maxXRoomIndex = mainRoom[0].ID;
        minZRoomIndex = mainRoom[0].ID;
        maxZRoomIndex = mainRoom[0].ID;

        for (int i = 0; i < mainRoom.Count; ++i)     //部屋ごとに辺を比較する
        {
			Rect rect = mainRoom[i].Rect();
            if (rect.xMin < mainRoom.Find((DungeonRoom min) => min.ID == minXRoomIndex).Rect().xMin)
            {
                minXRoomIndex = mainRoom[i].ID;
            }
            else if (rect.xMax > mainRoom.Find((DungeonRoom max) => max.ID == maxXRoomIndex).Rect().xMax)
            {
                maxXRoomIndex = mainRoom[i].ID;
            }
            if (rect.yMin < mainRoom.Find((DungeonRoom min) => min.ID == minZRoomIndex).Rect().yMin)
            {
                minZRoomIndex = mainRoom[i].ID;
            }
            else if (rect.yMax > mainRoom.Find((DungeonRoom max) => max.ID == maxZRoomIndex).Rect().yMax)
            {
                maxZRoomIndex = mainRoom[i].ID;
            }
        }

        currentState = GenerateState.WriteToArray;
	}

	private void UpdateWriteToArray()
	{
		//縦と横のマス数でマップチップの配列を生成
        int width = (int)(rooms[maxXRoomIndex].Rect().xMax - rooms[minXRoomIndex].Rect().xMin) + 1;
        int length = (int)(rooms[maxZRoomIndex].Rect().yMax - rooms[minZRoomIndex].Rect().yMin) + 1;
        mapChip = new int[length + 2, width + 2];   		 //四つの辺に壁を置く

        int minX = (int)rooms[minXRoomIndex].Rect().xMin;
        int minZ = (int)rooms[minZRoomIndex].Rect().yMin;

        for (int i = 0; i < mainRoom.Count; ++i)             //メインの部屋を先に置く
        {
			Rect rect = mainRoom[i].Rect();
            for (int y = (int)rect.yMin - minZ + 1; y < (int)rect.yMax - minZ + 1; ++y)         //基準となる部屋の座標を引く
            {
                for (int x = (int)rect.xMin - minX + 1; x < (int)rect.xMax - minX + 1; ++x)     //基準となる部屋の座標を引く
                {
                    mapChip[y, x] = (int)BlockDef.Space;
                }
            }
        }
        for (int i = 0; i < halls.Count; ++i)                 //廊下を置く
        {
			Rect rect = halls[i].Rect();
            for (int y = (int)rect.yMin - minZ + 1; y < (int)rect.yMax - minZ + 1; ++y)         //基準となる部屋の座標を引く
            {
                for (int x = (int)rect.xMin - minX + 1; x < (int)rect.xMax - minX + 1; x++)     //基準となる部屋の座標を引く
                {
                    mapChip[y, x] = (int)BlockDef.Space;
                }
            }
        }
        currentState = GenerateState.SetEventPoint;
	}

	private void UpdateSetEventPoint()
	{
		//違う部屋に設定（ランダム）
        int entryRoom = Random.Range(0, mainRoom.Count);      //入口の部屋（添え字）
        //メインの部屋ではないと選択しなおし
        while ((mainRoom[entryRoom].Width < (int)(MAX_ROOM_SIZE * 2 * 0.65f) &&
                mainRoom[entryRoom].Length < (int)(MAX_ROOM_SIZE * 2 * 0.65f)))
        {
            entryRoom = Random.Range(0, mainRoom.Count);
        }
        int exitRoom = 0;                                               //出口の部屋（添え字）
        for (int i = 1; i < mainRoom.Count; ++i)                        //一番遠い部屋で出口を指定
        {
            if (i == entryRoom)
                continue;
            if ((mainRoom[entryRoom].Pos - mainRoom[exitRoom].Pos).sqrMagnitude <
                (mainRoom[entryRoom].Pos - mainRoom[i].Pos).sqrMagnitude)
            {
                exitRoom = i;
            }
        }

        //マップのX, Y最小値   基準座標
        int minX = (int)rooms[minXRoomIndex].Rect().xMin;
        int minZ = (int)rooms[minZRoomIndex].Rect().yMin;

		Rect mainRect = mainRoom[entryRoom].Rect();
		Rect exitRect = mainRoom[exitRoom].Rect();

        //部屋内の乱数座標を設定（壁とつながっていないマス）
        Vector2Int entryPoint = new Vector2Int(
            Random.Range((int)mainRect.xMin - minX + 1, (int)mainRect.xMax - minX),
            Random.Range((int)mainRect.yMin - minZ + 1, (int)mainRect.yMax - minZ));
        Vector2Int exitPoint = new Vector2Int(
            Random.Range((int)exitRect.xMin - minX + 1, (int)exitRect.xMax - minX),
            Random.Range((int)exitRect.yMin - minZ + 1, (int)exitRect.yMax - minZ));

        mapChip[entryPoint.y, entryPoint.x] = (int)BlockDef.Entry;
        mapChip[exitPoint.y, exitPoint.x] = (int)BlockDef.Exit;

        currentState = GenerateState.ReleaseData;
	}

	public bool IsEnd()
    {
        return currentState == GenerateState.End;
    }

	public int[,] MapChip
    {
        get { return mapChip; }
    }

	private void UpdateRelease()
    {
        Release();
        currentState = GenerateState.End;
    }
}
