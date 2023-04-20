using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public Ghost[] ghosts;
    public Pacman pacman;
    public Transform pellets;

    public GameObject gameOverText;
    public Text scoreText;
    public Text livesText;
    public Text highestScoreText;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip deathAudio;
    [SerializeField]
    private AudioClip gameoverAudio;
    [SerializeField]
    private AudioClip winAudio;

    public int ghostMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int highestScore { get; private set; } = 0;
    public int lives { get; private set; }

    private void Start()
    {
        NewGame();  
    }

    private void Update()
    {
        if(lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }
    }


    private void NewGame()
    {
        SetLife(3);
        NewRound();
    }
    
    private void NewRound()
    {
        gameOverText.SetActive(false);
        foreach (Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }

        ResetState();
    }

    private void ResetState()
    {
        ResetGhostMultiplier();

        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].ResetState();
        }

        this.pacman.ResetState();
    }

    private void GameOver()
    {
        gameOverText.SetActive(true);
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
        this.audioSource.PlayOneShot(gameoverAudio);
    }
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(2, '0');
    }

    private void SetLife(int lives)
    {
        this.lives = lives;
        livesText.text = "x" + lives.ToString();
    }

    public void GhostEaten(Ghost ghost)
    {
        int points = ghost.points * this.ghostMultiplier;
        SetScore(this.score + points);
        this.ghostMultiplier++;
    }

    public void PacmanEaten()
    {
        this.pacman.DeathSequence();
        
        SetLife(this.lives - 1);
        
        if(this.lives > 0)
        {
            Invoke(nameof(ResetState), 3.0f);
            this.audioSource.PlayOneShot(deathAudio);
        }
        else
        {
            GameOver();
            if(this.score > this.highestScore)
            {
                this.highestScore = this.score;
                highestScoreText.text = "highest: " + highestScore.ToString().PadLeft(2, '0');
            }
        }
    }

    public void PelletEaten(Pellet pellet)
    {
        pellet.gameObject.SetActive(false);
        SetScore(this.score + pellet.point);

        if (!HasRemainingPellet())
        {
            this.pacman.gameObject.SetActive(false);
            Invoke(nameof(NewRound), 3.0f);
            this.audioSource.PlayOneShot(winAudio);
        }
    }

    public void PowerPelletEaten(PowerPellet pellet)
    {     
        for (int i =0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].frightened.Enable(pellet.duration);
        }

        PelletEaten(pellet);
        CancelInvoke();
        Invoke(nameof(ResetGhostMultiplier), pellet.duration);
    }

    private bool HasRemainingPellet()
    {
        foreach (Transform pellet in this.pellets)
        {
            if (pellet.gameObject.activeSelf)
            {
                return true; 
            }
        }
        return false;
    }

    private void ResetGhostMultiplier()
    {
        this.ghostMultiplier = 1;
    }

    public void LoadData(GameData data)
    {
        this.score = data.score;
        this.highestScore = data.highestScore;
        highestScoreText.text = "highest: " + highestScore.ToString().PadLeft(2, '0');
    }

    public void SaveData(GameData data)
    {
        data.score = this.score;
        data.highestScore = this.highestScore;
    }
}
