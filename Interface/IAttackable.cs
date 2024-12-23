
namespace HIEU_NL.Platformer.Script.Interface
{
    public interface IAttackable
    {
        public abstract void IBeginAttack();
        public abstract void IAttacking();
        public abstract void IFinishAttack();

        public abstract void IBeginTrace();
        public abstract void ITracing();
        public abstract void IFinishTrace();

    }

}