using UnityEngine;

public class Ghost : MonoBehaviour, IDataPersistence 
{
    public string ghostName;
    public Vector3 position;
    public Vector2 direction;
    public string ghostState;
    public bool ghostInHome = true;

    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }
    public GhostBehavior initialBehavior;
    public Transform target;

    public int points = 200;

    private void Awake()
    {
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened= GetComponent<GhostFrightened>();
    }

    private void Start()
    {
        //SetGhostState();
        //ResetState();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();
        this.ghostState = "scatter";

        if (this.initialBehavior != null)
        {
            this.initialBehavior.Enable();
            this.ghostInHome = true;
        }

        if (this.home != this.initialBehavior)
        {
            this.home.Disable();
            this.ghostInHome = false;
        }

       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (this.frightened.enabled)
            {
                FindObjectOfType<GameManager>().GhostEaten(this);
            }
            else
            {
                FindObjectOfType<GameManager>().PacmanEaten();
            }
        }
    }

    public void SetGhostState()
    {
        
        if(ghostState == "scatter")
        {
            this.frightened.Disable();
            this.chase.Disable();
            this.scatter.Enable();

        }
        if(ghostState == "chase")
        {
            this.frightened.Disable();
            this.chase.Enable();
            this.scatter.Disable();

        }

        if (ghostInHome == true)
        {
            this.home.Enable();
        }

    }
    public void LoadData(GameData data)
    {
        data.ghostPosition.TryGetValue(ghostName, out this.position);
        this.transform.position = position;
        data.ghostDirection.TryGetValue(ghostName, out this.direction);
        this.movement.SetDirection(this.direction);
        data.ghostInHome.TryGetValue(ghostName, out ghostInHome);
        data.ghostState.TryGetValue(ghostName, out this.ghostState);
        this.SetGhostState();

       
    }

    public void SaveData(GameData data)
    {
        if (data.ghostPosition.ContainsKey(ghostName))
        {
            data.ghostPosition.Remove(ghostName);
        }
        data.ghostPosition.Add(ghostName, this.transform.position);

        if (data.ghostState.ContainsKey(ghostName))
        {
            data.ghostState.Remove(ghostName);
        }
        data.ghostState.Add(ghostName, this.ghostState);

        if (data.ghostInHome.ContainsKey(ghostName))
        {
            data.ghostInHome.Remove(ghostName);
        }
        data.ghostInHome.Add(ghostName, this.ghostInHome);

        if (data.ghostDirection.ContainsKey(ghostName))
        {
            data.ghostDirection.Remove(ghostName);
        }
        data.ghostDirection.Add(ghostName, this.movement.direction);

    }
}
