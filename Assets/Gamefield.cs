using UnityEngine;
using System.Collections.Generic;

public enum ObjectType
{
    SPACE, WALL, BRICKWALL, PERSON, BOMB, EXPLOSION, BONUS
}

public class Gamefield : MonoBehaviour
{

    const int width = 9;
    const int height = 9;
    GameObject self;
    Grid view;

    ObjectType[][] field;

    Dictionary<Point,BombState> bomblist;
    Dictionary<Point,ExplosionState> expllist;

    void Start()
    {
        self = GameObject.Find("Canvas");
        view = FindObjectOfType<Grid>();
        field = new ObjectType[width][];
        for (int i = 0; i < width; i++)
        {
            field[i] = new ObjectType[height];
        }
        bomblist = new Dictionary<Point, BombState>();
        expllist = new Dictionary<Point, ExplosionState>();
    }

    void Explode(int x, int y, int force)
    {
        view.SetTile(x, y, SpriteType.EXPL_CENTER);
        field[x][y] = ObjectType.EXPLOSION;
        bomblist[new Point(x, y)].exploded = true;
        Explode(x, y, -1, 0, force);
        Explode(x, y, 1, 0, force);
        Explode(x, y, 0, 1, force);
        Explode(x, y, 0, -1, force);
    }

    void Explode(int _x, int _y, int xdelta, int ydelta, int force)
    {
        int x = _x + xdelta;
        int y = _y + ydelta;
        if (x >= 0 && x < width && y >= 0 && y < height && field[x][y] != ObjectType.WALL)
        {
            ObjectType old = field[x][y];
            field[x][y] = ObjectType.EXPLOSION;
            switch (old)
            {
                case ObjectType.BOMB:
                    if (!bomblist[new Point(x, y)].exploded)
                    {
                        Explode(x, y, 2);
                    }
                    break;
                default:
                    if (xdelta != 0)
                    {
                        if (xdelta > 0)
                        {
                            view.SetTile(x, y, SpriteType.EXPL_RIGHT);
                        }
                        else
                        {
                            view.SetTile(x, y, SpriteType.EXPL_LEFT);
                        }
                    }
                    else
                    {
                        if (ydelta > 0)
                        {
                            view.SetTile(x, y, SpriteType.EXPL_UP);
                        }
                        else
                        {
                            view.SetTile(x, y, SpriteType.EXPL_DOWN);
                        }
                    }
                    break;
            }
        }
        force -= 1;
        if (force > 0)
        {
            Explode(x, y, xdelta, ydelta, force);
        }
    }

    void UpdateBombs(float delta)
    {
        foreach (var pos in bomblist.Keys)
        {
            BombState bomb = bomblist[pos];
            bomb.time -= delta;
            if (bomb.time < 0)
            {
                Explode(pos.x, pos.y, 2);
            }
            else
            {
                bomb.lastswitch += delta;
                if (bomb.lastswitch > 0.5)
                {
                    bomb.lastswitch = 0;
                    bomb.state ^= 1;
                    switch (bomb.state)
                    {
                        case 0: view.SetTile(pos.x, pos.y, SpriteType.BIGBOMB); break;
                        case 1: view.SetTile(pos.x, pos.y, SpriteType.SMALLBOMB); break;
                    }
                }
            }
        }
        foreach (var pos in bomblist.Keys)
        {
            var point = new Point(pos.x, pos.y);
            if (bomblist[point].exploded)
            {
                bomblist.Remove(point);
            }
        }
    }

    public void PlaceBomb(int x, int y)
    {
        field[x][y] = ObjectType.BOMB;
        bomblist.Add(new Point(x, y), new BombState());
    }

    void Update()
    {
        UpdateBombs(Time.deltaTime);
    }
}

class BombState
{
    public float time;
    public float lastswitch;
    public int state;
    public bool exploded;

    public BombState()
    {
        time = 3;
        lastswitch = 0;
        state = 0;
        exploded = false;
    }
}

class ExplosionState
{
    public float time;

    public ExplosionState()
    {
        time = 1;
    }
}

struct Point
{
    public int x;
    public int y;
    public Point(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}
