namespace HIEU_NL.Platformer.Script.Interface
{
    public interface IDamageable
    {
        public int Health { get;}
        public int MaxHealth { get;}
        
        public abstract bool ITakeDamage(HitData hitData);
    }
    
}