using UnityEngine;

public class CharacterStats : Interactable
{
    [SerializeField] private string characterName;
    private int maxHealth = 100;
    private int currentHealth;
    private int ammo = 99999999;
    private double damageDealingMultiplier = 1;
    private double damageReceivingMultiplier = 1;
    private double baseDamage = 25;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private bool isCompanion = false;
    [SerializeField] private bool isZombie = false;
    private bool isDowned = false;
    [SerializeField] GameObject gameOverOverlay;

    public void setStats(string name, int maxHealth, int ammo, double damageDealingMultiplier, double damageReceivingMultiplier)
    {
        this.characterName = name;
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth; // Start with full health
        this.ammo = ammo;
        this.damageDealingMultiplier = damageDealingMultiplier;
        this.damageReceivingMultiplier = damageReceivingMultiplier;
    }

    public string getName()
    {
        return name;
    }

    public void takeDamage(double damageTaken) {
        currentHealth -= (int)(damageTaken * damageReceivingMultiplier);
        if (currentHealth < 0)
        {
            this.down();
        }
    }

    public void dealDamage(double damageToDeal, CharacterStats opponent) {
        double damageAfterMultiplier = damageToDeal * damageDealingMultiplier;
        opponent.takeDamage((int)damageAfterMultiplier);
    }

    public void down()
    {
        if (isCompanion) {
            setDowned(true);
        }
        if (isPlayer)
        {
            setDowned(true);
            killed();
        }
        if (isZombie)
        {
            killed();
        }
    }

    public void killed() {
        if (isZombie) {
            this.playDeathAnimation();
            this.gameObject.gameObject.SetActive(false);
        }
        if (isPlayer) {
            this.gameOver();
        }
    }

    public void gameOver() {
        gameOverOverlay.SetActive(true);
    }

    public void playDeathAnimation() {
    
    }

    public override void Interact()
    {
        if (isDowned) {
            //start timer with visual feedback for revival
            revive();
        }
        else
        {
            RaiseInteraction();
        }
    }

    public bool isCharacterDowned()
    {
        return isDowned;
    }

    public void revive() {
        this.currentHealth = this.maxHealth; 
        this.setDowned(false);
    }

    public void setDowned(bool isDownedPassed) {
        this.isDowned = isDownedPassed;
        if (isDowned) {
            this.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (!isDowned)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            GameObject shooter = collision.gameObject.GetComponent<Bullet>().getShooter();
            if (shooter != null && shooter != this.gameObject)
            {
                CharacterStats shooterStats = shooter.GetComponent<CharacterStats>();
                if (shooterStats != null)
                {
                    shooterStats.dealDamage(shooterStats.getBaseDamage(), this);
                }
            }
            collision.gameObject.SetActive(false);
        }
    }

    public double getBaseDamage()
    {
        return baseDamage;
    }

    public string getCharacterName()
    {
        return characterName;
    }
}
