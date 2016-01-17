using UnityEngine;

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

    void Start()
    {
        self = GameObject.Find("Canvas");
        view = FindObjectOfType<Grid>();
        field = new ObjectType[width][];
        for (int i = 0; i < width; i++)
        {
            field[i] = new ObjectType[height];
        }
    }

    public void Clicked(int x, int y)
    {
        view.SetTile(x, y, SpriteType.BIGBOMB);
    }

    void Update()
    {

    }
}
