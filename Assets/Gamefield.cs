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

    LinkedList<BombState> bomblist;
    LinkedList<ExplosionState> expllist;

    void Start()
    {
        self = GameObject.Find("Canvas");
        view = FindObjectOfType<Grid>();
        field = new ObjectType[width][];
        for (int i = 0; i < width; i++)
        {
            field[i] = new ObjectType[height];
        }
        bomblist = new LinkedList<BombState>();
        expllist = new LinkedList<ExplosionState>();
    }

    void UpdateBombs(float delta)
    {
        int removecount = 0;
        foreach (BombState bomb in bomblist)
        {
            bomb.time -= delta;
            if (bomb.time < 0)
            {
                view.SetTile(bomb.x, bomb.y, SpriteType.EXPL_CENTER);
                removecount++;
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
                        case 0: view.SetTile(bomb.x, bomb.y, SpriteType.BIGBOMB); break;
                        case 1: view.SetTile(bomb.x, bomb.y, SpriteType.SMALLBOMB); break;
                    }
                }
            }
        }
        for (int i = 0; i < removecount; i++)
        {
            bomblist.RemoveLast();
        }
    }

    public void PlaceBomb(int x, int y)
    {
        field[x][y] = ObjectType.BOMB;
        bomblist.AddFirst(new BombState(x, y));
    }

    void Update()
    {
        UpdateBombs(Time.deltaTime);
    }
}

class BombState
{
    public int x;
    public int y;
    public float time;
    public float lastswitch;
    public int state;

    public BombState(int _x, int _y)
    {
        x = _x;
        y = _y;
        time = 3;
    }
}

class ExplosionState
{
    public int x;
    public int y;
    public float time;

    public ExplosionState(int _x, int _y)
    {
        x = _x;
        y = _y;
        time = 1;
    }
}
