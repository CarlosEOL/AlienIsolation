namespace NPCs
{
    public interface IEnemies
    {
        void Chase();
    }
    
    public class Enemy : NPC, IEnemies
    {
        void IEnemies.Chase()
        {
            
        }
    }

}
