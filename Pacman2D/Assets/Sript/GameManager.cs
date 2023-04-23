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
    public GameObject ESC_Menu;
    public GameObject Start_Menu;


    public float pauseTime;

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
        this.Start_Menu.SetActive(true);
        this.PauseGame();
    }

    private void Update()
    {
        if(lives <= 0 && Input.anyKeyDown)
        {
            NewGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.ESC_Menu.gameObject.activeSelf)
            {
                this.ESC_Menu.gameObject.SetActive(false);
                this.UnPauseGame();
            }
            else
            {
                this.ESC_Menu.gameObject.SetActive(true);
                this.PauseGame();
            }
        }
    }

    public void NewGame()
    {
        SetLife(3);
        NewRound();
        SetScore(0);
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
        pellet.collected = true;
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
        this.lives = data.lives;
        this.pacman.transform.position = data.playerPosition;
        this.pacman.movement.SetDirection(data.pacmanDirection);
        this.LoadDisplay();

    }

    public void SaveData(GameData data)
    {
        data.score = this.score;
        data.highestScore = this.highestScore;
        data.lives = this.lives;
        data.playerPosition = this.pacman.transform.position;
        data.pacmanDirection = this.pacman.movement.direction;
    }

    public void LoadDisplay()
    {
        scoreText.text = score.ToString().PadLeft(2, '0');
        highestScoreText.text = "highest: " + highestScore.ToString().PadLeft(2, '0');
        livesText.text = "x" + lives.ToString();
    }

    public void PauseGame()
    {
        this.pacman.movement.enabled = false;
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].movement.enabled = false;
            if (this.ghosts[i].ghostInHome)
            {
                this.ghosts[i].home.Enable(pauseTime);
            }
        }
    }

    public void UnPauseGame()
    {
        this.pacman.movement.enabled = true;
        for (int i = 0; i < this.ghosts.Length; i++)
        {
            this.ghosts[i].movement.enabled = true;
            if (this.ghosts[i].ghostInHome)
            {
                this.ghosts[i].home.Enable();
            }
        }
    }

    public void ResumeButtonClick()
    {
        this.ESC_Menu.gameObject.SetActive(false);
        this.UnPauseGame();
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }

    public void ContinueButtonClick()
    {
        this.Start_Menu.SetActive(false);
        this.UnPauseGame();
    }

    public void NewGameButtonClick()
    {
        this.Start_Menu.SetActive(false);
        this.NewGame();
        this.UnPauseGame();
    }
}
