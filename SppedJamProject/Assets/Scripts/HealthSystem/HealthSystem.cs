using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public AudioSource damageAudioSource;
    public AudioClip dieClip;
    public AudioClip damageClip;
    public ElectricityController electricity;

    private float health;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        this.damageAudioSource.clip = damageClip;
        this.damageAudioSource.Play();

        health -= damageAmount;
        //Debug.Log("Current health: " + health);
        if (health <= 0)
        {
            health = 0;
            //Debug.Log("dead: " + health);
            Die();
        }

    }

    private void Die()
    {
        if (this.tag == "Player")
        {
            this.damageAudioSource.clip = dieClip;
            this.damageAudioSource.Play();
            this.gameObject.SetActive(false);
            Invoke("GameOver", 2);
        }
        if (this.tag == "Enemy")
        {
            GameManager.instance.AddEnemyDeadCount();
            Debug.Log("enemy dead counter: " + GameManager.instance.GetEnemyDeadCount());
            electricity.RemoveEnemyInRage(this.gameObject);
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        Application.Quit();
    }
}
