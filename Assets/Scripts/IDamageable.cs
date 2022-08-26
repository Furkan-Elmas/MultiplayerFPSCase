public interface IDamageable
{
    public int CurrentHealth { get; set; }

    public void Die(Player target);
    public void Respawn(Player target);
}
