using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour {

    const int width = 7;
    const int height = 7;
    GameObject self;
    GameObject tilePrefab;

    GameObject[][] tiles;

    void Start() {
        self = GameObject.Find("Canvas");
        tilePrefab = Resources.Load<GameObject>("Tile");
        tiles = new GameObject[width][];
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
        rt.localPosition = new Vector3(x * 30, y * 30);
        tile.GetComponent<Image>().color = new Color((float)(1.0 / 7.0 * x), (float)(1.0 / 7.0 * y), 0);
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
