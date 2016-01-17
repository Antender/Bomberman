using UnityEngine;
using System.Collections.Generic;

public enum ObjectType
{
    SPACE, WALL, BRICKWALL, BOMB, EXPLOSION, BONUS
}

public enum KeyType
{
    UP, DOWN, LEFT, RIGHT, BOMB
}

public class Gamefield : MonoBehaviour
{
    const int width = 9;
    const int height = 9;
    const int wallcount = 60;
    GameObject self;
    Grid view;

    ObjectType[][] field;

    Dictionary<Point, BombState> bombPositions;
    Dictionary<Point, ExplosionState> explosionPositions;
    HashSet<Point> bonusNominees;

    Point heroPosition;
    int heroForce = 1;

    KeyCode[] keyCodes;
    bool[] keyPressed;

    void Start()
    {
        view = FindObjectOfType<Grid>();
        field = new ObjectType[width][];
        for (int i = 0; i < width; i++)
        {
            field[i] = new ObjectType[height];
        }
        bombPositions = new Dictionary<Point, BombState>();
        explosionPositions = new Dictionary<Point, ExplosionState>();
        bonusNominees = new HashSet<Point>();
        GenerateField();
        PlaceHero();
        keyCodes = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B };
        keyPressed = new bool[5];
    }

    void GenerateField()
    {
        List<int> pos = new List<int>(width * height);
        for (int i = 0; i < width * height; i++)
        {
            pos.Add(i);
        }
        for (int i = wallcount; i > 0; i--)
        {
            int index = Random.Range(0, pos.Count - 1);
            int x = index % width;
            int y = index / width;
            field[index % width][index / width] = ObjectType.BRICKWALL;
            view.SetTile(x, y, SpriteType.BRICKWALL);
        }
        for (int i = 1; i < width; i += 2)
        {
            for (int j = 1; j < height; j += 2)
            {
                field[i][j] = ObjectType.WALL;
                view.SetTile(i, j, SpriteType.WALL);
            }
        }
    }

    void PlaceHero()
    {
        int x = Random.Range(0, width - 1);
        int y;
        if (x % 2 == 0)
        {
            y = Random.Range(0, height);
        }
        else
        {
            y = Random.Range(0, height / 2) * 2;
        }
        view.SetHeroPosition(x, y);
        RemoveObstacle(x, y);
        RemoveObstacle(x - 1, y);
        RemoveObstacle(x + 1, y);
        RemoveObstacle(x, y - 1);
        RemoveObstacle(x, y + 1);
        heroPosition.x = x;
        heroPosition.y = y;
    }

    void RemoveObstacle(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height && field[x][y] == ObjectType.BRICKWALL)
        {
            field[x][y] = ObjectType.SPACE;
            view.SetTile(x, y, SpriteType.SPACE);
        }
    }

    void Explode(int x, int y, int force)
    {
        view.SetTile(x, y, SpriteType.EXPL_CENTER);
        field[x][y] = ObjectType.EXPLOSION;
        explosionPositions[new Point(x, y)] = new ExplosionState();
        bombPositions[new Point(x, y)].exploded = true;
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
            explosionPositions[new Point(x, y)] = new ExplosionState();
            switch (old)
            {
                case ObjectType.BOMB:
                    if (!bombPositions[new Point(x, y)].exploded)
                    {
                        Explode(x, y, heroForce);
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
            if (old == ObjectType.BRICKWALL)
            {
                if (Random.value > 0.8)
                {
                    bonusNominees.Add(new Point(x, y));
                }
            }
            force -= 1;
            if (force > 0)
            {
                Explode(x, y, xdelta, ydelta, force);
            }
        }
    }

    void UpdateBombs(float delta)
    {
        foreach (var pos in bombPositions.Keys)
        {
            BombState bomb = bombPositions[pos];
            bomb.time -= delta;
            if (bomb.time < 0)
            {
                Explode(pos.x, pos.y, heroForce);
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
        List<Point> removal = new List<Point>();
        foreach (var pos in bombPositions.Keys)
        {
            var point = new Point(pos.x, pos.y);
            if (bombPositions[point].exploded)
            {
                removal.Add(point);
            }
        }
        foreach (Point p in removal)
        {
            bombPositions.Remove(p);
        }
    }

    void UpdateExplosions(float time)
    {
        List<Point> removal = new List<Point>();
        foreach (var pos in explosionPositions.Keys)
        {
            ExplosionState state = explosionPositions[pos];
            state.time -= time;
            if (state.time < 0)
            {
                if (bonusNominees.Contains(pos))
                {
                    field[pos.x][pos.y] = ObjectType.BONUS;
                    view.SetTile(pos.x, pos.y, SpriteType.BONUS);
                    bonusNominees.Remove(pos);
                }
                else
                {
                    field[pos.x][pos.y] = ObjectType.SPACE;
                    view.SetTile(pos.x, pos.y, SpriteType.SPACE);
                }
                removal.Add(pos);
            }
        }
        foreach (Point p in removal)
        {
            explosionPositions.Remove(p);
        }
    }

    public void PlaceBomb(int x, int y)
    {
        if (field[x][y] == ObjectType.SPACE)
        {
            field[x][y] = ObjectType.BOMB;
            bombPositions[new Point(x, y)] = new BombState();
            view.SetTile(x, y, SpriteType.BIGBOMB);
        }
    }

    public void HeroActs(KeyType key)
    {
        switch (key)
        {
            case KeyType.UP: Move(heroPosition.x, heroPosition.y + 1); break;
            case KeyType.DOWN: Move(heroPosition.x, heroPosition.y - 1); break;
            case KeyType.LEFT: Move(heroPosition.x - 1, heroPosition.y); break;
            case KeyType.RIGHT: Move(heroPosition.x + 1, heroPosition.y); break;
            case KeyType.BOMB: PlaceBomb(heroPosition.x, heroPosition.y); break;
        }
    }

    public void Move(int x, int y)
    {
        if (CanMove(x,y))
        {
            heroPosition.x = x;
            heroPosition.y = y;
            view.SetHeroPosition(x, y);
            if (field[x][y] == ObjectType.BONUS)
            {
                field[x][y] = ObjectType.SPACE;
                view.SetTile(x, y, SpriteType.SPACE);
                heroForce += 1;
            }
        }
    }

    public bool CanMove(int x, int y)
    {
        ObjectType checkedTile = field[x][y];
        return
            checkedTile != ObjectType.WALL &&
            checkedTile != ObjectType.BRICKWALL &&
            checkedTile != ObjectType.BOMB;
    }

    public void CheckKeys()
    {
        for (int i = 0; i < 5; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                keyPressed[i] = true;
            }
            else
            {
                if (keyPressed[i])
                {
                    HeroActs((KeyType)i);
                    keyPressed[i] = false;
                }
            }
        }
    }

    void Update()
    {
        UpdateBombs(Time.deltaTime);
        UpdateExplosions(Time.deltaTime);
        CheckKeys();
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
