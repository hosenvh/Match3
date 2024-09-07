using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.SubSystems.VacuumCleanerMechanic
{
    public struct VacuumCleanerMechanicProperties : Component
    {
        public readonly bool vacuumCleanerMustHitOnlyOnce;

        public VacuumCleanerMechanicProperties(bool vacuumCleanerMustHitOnlyOnce)
        {
            this.vacuumCleanerMustHitOnlyOnce = vacuumCleanerMustHitOnlyOnce;
        }
    }
}