using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int score;
    public int highestScore;
    public int lives;
    public Vector3 playerPosition;
    public Vector2 pacmanDirection;
    public SerializableDictionary<string, Vector3> ghostPosition;
    public SerializableDictionary<string, Vector2> ghostDirection;
    public SerializableDictionary<string, string> ghostState;
    public SerializableDictionary<string, bool> ghostInHome;

    public SerializableDictionary<Vector3, bool> pelletsCollected;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        this.score = 0;
        this.highestScore = 0;
        this.lives = 3;
        playerPosition = new Vector3(0, -9.5f, -5f);
        pacmanDirection = Vector2.right;

        ghostPosition = new SerializableDictionary<string, Vector3>();
        ghostDirection = new SerializableDictionary<string, Vector2>();
        ghostState = new SerializableDictionary<string, string>();

        pelletsCollected = new SerializableDictionary<Vector3, bool>();
    }
}
