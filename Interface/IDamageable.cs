namespace HIEU_NL.Platformer.Script.Interface
{
    public interface IDamageable
    {
        public abstract bool ICanTakeDamage();
        public abstract bool ITakeDamage(HitData hitData);
    }
    
}