
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Cells
{
    public abstract class AbstractArtifactMainCellPresenter : Core.CellPresenter
    {
        public RectTransform rotationTransform;
        public RectTransform scaleTransform;

        protected override void InternalSetup()
        {
            var artifaceCell = cell as AbstractArtifactMainCell;

            var scale = new Vector3();
            var rotation = new Vector3();

            scale.z = 1;
            scale.x = artifaceCell.ArtifactSize().x;
            scale.y = artifaceCell.ArtifactSize().y;

            switch (artifaceCell.Direction())
            {
                case Direction.Right:
                    rotation.z = 90;
                    scaleTransform.pivot = new Vector2(1, 1);
                    break;
                case Direction.Down:
                    rotation.z = 0;
                    scaleTransform.pivot = new Vector2(0, 1);
                    break;
            }

            scaleTransform.localScale = scale;
            rotationTransform.localEulerAngles = rotation;
        }
    }
}