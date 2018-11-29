using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom
{
	private enum Direction      //部屋衝突判定の方向
        {
            Xplus,
            Xminus,
            Zplus,
            Zminus
        }

        private int id;             //部屋の番号
        private int widthCell;      //横サイズ（単位：マス）
        private int lengthCell;     //縦サイズ（単位：マス）
        private Vector2Int cellPos;

		public DungeonRoom(int id, int widthCell, int lengthCell, Vector2Int cellPos)
        {
            this.id = id;
            this.widthCell = widthCell;
            this.lengthCell = lengthCell;
			this.cellPos = cellPos;
        }

		public Rect Rect()
        {
            return new Rect(
				new Vector2(cellPos.x - widthCell / 2, cellPos.y - lengthCell / 2),
				new Vector2(widthCell, lengthCell));
        }

		public bool RoomCollision(DungeonRoom other)
        {
            return Rect().Overlaps(other.Rect());
        }

		public void Hit(DungeonRoom other)
		{
			Direction direction = CheckDirection(other);    //他の部屋のどの方向

            //一マスずつ修正
            switch (direction)
            {
                case Direction.Xplus:
                    cellPos.x += 1;
                    break;
                case Direction.Xminus:
                    cellPos.x -= 1;
                    break;
                case Direction.Zplus:
                    cellPos.y += 1;
                    break;
                case Direction.Zminus:
                    cellPos.y -= 1;
                    break;
            }
		}

		 private Direction CheckDirection(DungeonRoom other)
        {
            Vector2 dir = cellPos - other.cellPos;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))      //X軸の変化量がY軸より大きい場合
            {
                if (dir.x > 0)                          //自分のXが相手のXより大きい場合
                    return Direction.Xplus;
                else
                    return Direction.Xminus;
            }

            if (dir.y > 0)                              //自分のZが相手のZより大きい場合
            {
                return Direction.Zplus;
            }
            return Direction.Zminus;
        }

		public int ID
        {
            get { return id; }
        }

		public int Length
        {
            get { return lengthCell; }
        }

        public int Width
        {
            get { return widthCell; } 
        }

		 public Vector2Int Pos
        {
            get { return cellPos; }
        }
}
