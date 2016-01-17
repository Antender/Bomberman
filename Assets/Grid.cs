using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour {

    const int width = 9;
    const int height = 9;
    const int tileWidth = 32;
    const int tileHeight = 32;
    GameObject self;
    GameObject tilePrefab;

    GameObject[][] tiles;
    Sprite[] spritesheet;

    void Start() {
        self = GameObject.Find("Canvas");
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
        tile.GetComponent<Image>().color = new Color((float)(1.0 / width * x), (float)(1.0 / height * y), 0);
        tile.GetComponent<Image>().overrideSprite = spritesheet[0];
        tile.GetComponent<Button>().onClick.AddListener(() =>
        {
            tile.GetComponent<Image>().color = new Color(0, 0, 0);
        });
        tiles[x][y] = tile;
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

    void TileOnClick()
    {

    }

	void Update() {
	
	}
}
