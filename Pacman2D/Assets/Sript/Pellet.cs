using UnityEngine;

public class Pellet : MonoBehaviour,IDataPersistence
{
    public int point = 10;
    public bool collected = false;

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            collected = false;
        }
    }
    protected virtual void Eat()
    {
        FindObjectOfType<GameManager>().PelletEaten(this);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            Eat();
        }
    }

    public void LoadData(GameData data)
    {
        data.pelletsCollected.TryGetValue(this.transform.position, out collected);
        if (collected)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.pelletsCollected.ContainsKey(this.transform.position))
        {
            data.pelletsCollected.Remove(this.transform.position);
        }
        data.pelletsCollected.Add(this.transform.position, collected);
    }
}
