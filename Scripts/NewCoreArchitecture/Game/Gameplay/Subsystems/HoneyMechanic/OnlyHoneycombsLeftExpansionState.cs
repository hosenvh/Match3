using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{
    public class OnlyHoneycombsLeftExpansionState : HoneyExpansionSystemState
    {
        const int needed_player_moves = 2;
        UserMovementData userMovementData;
        int playersTotalMove;

        public OnlyHoneycombsLeftExpansionState(HoneyExpansionSystem system, CellStackBoard cellStackBoard) : base(system, cellStackBoard)
        {
            userMovementData = system.GetFrameData<UserMovementData>();
        }

        public override void OnEnter()
        {
            playersTotalMove = 0;
            // NOTE: If state is entered because of a move that move must not be considered in the logic.
            if (userMovementData.moves > 0)
                playersTotalMove -= 1;
        }

        public override void Update()
        {
            if(userMovementData.moves > 0)
                playersTotalMove += 1;

            if(playersTotalMove >= needed_player_moves)
            {
                system.StartGrowingAHoneyTile();
                playersTotalMove = 0;
            }
        }
    }
}