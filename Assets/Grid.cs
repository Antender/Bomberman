using UnityEngine;
using UnityEngine.UI;

public enum SpriteType
{
    WALL, SPACE, BRICKWALL, PERSON,
    BIGBOMB, SMALLBOMB,
    EXPL_CENTER, EXPL_LEFT, EXPL_UP, EXPL_DOWN, EXPL_RIGHT,
    BONUS
}

public class Grid : MonoBehaviour
{

    const int width = 9;
    const int height = 9;
    const int tileWidth = 32;
    const int tileHeight = 32;
    GameObject self;
    Gamefield model;
    GameObject tilePrefab;

    GameObject[][] tiles;
    Sprite[] spritesheet;


    void Start()
    {
        self = GameObject.Find("Canvas");
        model = FindObjectOfType<Gamefield>();
        tilePrefab = Resources.Load<GameObject>("Tile");
        tiles = new GameObject[width][];
        spritesheet = Resources.LoadAll<Sprite>("Spritesheet");
        for (int i = 0; i < width; i++)
        {
            tiles[i] = new GameObject[height];
        }
        AddTiles(width, height);
    }

    void AddTile(int x, int y)
    {
        GameObject tile = Instantiate(tilePrefab);
        tile.transform.SetParent(self.transform, true);
        RectTransform rt = tile.GetComponent<RectTransform>();
        rt.localPosition = new Vector3(x * tileWidth, y * tileHeight);
        tile.GetComponent<Button>().onClick.AddListener(() =>
        {
            model.PlaceBomb(x, y);
        });
        tiles[x][y] = tile;
        SetTile(x, y, SpriteType.SPACE);
    }

    public void SetTile(int x, int y, SpriteType type)
    {
        tiles[x][y].GetComponent<Image>().overrideSprite = spritesheet[(int)type];
    }

    void AddTiles(int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                AddTile(i, j);
            }
        }
    }

    void Update()
    {

    }
}

