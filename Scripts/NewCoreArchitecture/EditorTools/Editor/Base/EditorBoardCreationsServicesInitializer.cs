using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Factories;
using Match3.Main;


namespace Match3.EditorTools.Editor.Base
{
    public class EditorBoardCreationsServicesInitializer
    {
        public static void InitializeServices()
        {
            InitializeBoardCreationNeededServices();

            void InitializeBoardCreationNeededServices()
            {
                ServiceLocator.Replace<CellFactory>(new MainCellFactory());
                ServiceLocator.Replace<TileFactory>(new MainTileFactory());
                ServiceLocator.Replace<CellAttachmentFactory>(new MainCellAtttachmentFactory());
            }
        }
    }
}