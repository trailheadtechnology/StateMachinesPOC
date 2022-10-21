using Stateless;

namespace StatePOC
{
    public class EntityStateMachine 
    {
        public EntityStateMachine()
        {
            StateMachine = GetEntityStateMachine();
            EntitySourceValue = EntitySource.NotSet;
        }

        public EntitySource EntitySourceValue { get; private set; }

        public StateMachine<EntityState, EntityTrigger> StateMachine { get; private set; }

        StateMachine<EntityState, EntityTrigger> GetEntityStateMachine()
        {
            var entityStateMachine = new StateMachine<EntityState, EntityTrigger>(EntityState.NotSet);

            entityStateMachine.Configure(EntityState.NotSet)
                .OnActivate(() => EntitySourceValue = EntitySource.NotSet)
                .OnEntryFrom(EntityTrigger.SaveAndClose,() => EntitySourceValue = EntitySource.NotSet)
                .OnEntryFrom(EntityTrigger.Close,() => EntitySourceValue = EntitySource.NotSet)
                .OnEntryFrom(EntityTrigger.Discard,() => EntitySourceValue = EntitySource.NotSet)
                .PermitIf(EntityTrigger.Load, EntityState.LoadingFromServer, () => EntitySourceValue == EntitySource.NotSet)
                .PermitIf(EntityTrigger.Load, EntityState.LoadingLocally, () => EntitySourceValue == EntitySource.Server)
                .PermitIf(EntityTrigger.Load, EntityState.Creating, () => EntitySourceValue == EntitySource.Local);

            entityStateMachine.Configure(EntityState.Loading)
                .Permit(EntityTrigger.LoadFail, EntityState.NotSet)
                .Permit(EntityTrigger.Setted, EntityState.Set);

            
            // Substates are not really well handled on the generated diagram

            #region Substates of Loading

            entityStateMachine.Configure(EntityState.LoadingFromServer)
                .OnEntry(() => EntitySourceValue = EntitySource.Server)
                .SubstateOf(EntityState.Loading);
            
            entityStateMachine.Configure(EntityState.LoadingLocally)
                .OnEntry(() => EntitySourceValue = EntitySource.Local)
                .SubstateOf(EntityState.Loading);
            
            entityStateMachine.Configure(EntityState.Creating)
                .OnEntry(() => EntitySourceValue = EntitySource.New)
                .SubstateOf(EntityState.Loading);

            #endregion

            entityStateMachine.Configure(EntityState.Set)
                .Permit(EntityTrigger.ChangeValues, EntityState.Changed)
                .Permit(EntityTrigger.Close, EntityState.NotSet);

            entityStateMachine.Configure(EntityState.Changed)
                .Permit(EntityTrigger.Save, EntityState.Set)
                .Permit(EntityTrigger.SaveAndClose, EntityState.NotSet)
                .Permit(EntityTrigger.Discard, EntityState.NotSet);

            return entityStateMachine;

            // TODO: define events
            // TODO: use state transition to hookup events
        }

        public enum EntitySource
        {
            NotSet,
            Server,
            Local,
            New
        }

        public enum EntityState
        {
            NotSet,
            Loading,
                LoadingFromServer,
                LoadingLocally,
                Creating,
            Set,
            Changed
        }

        public enum EntityTrigger
        {
            Load,
            LoadFail,
            Setted,
            ChangeValues,
            Save,
            SaveAndClose,
            Discard,
            Close
        }
    }


}
