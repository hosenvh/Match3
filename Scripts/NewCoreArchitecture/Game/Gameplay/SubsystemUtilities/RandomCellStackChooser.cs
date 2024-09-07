
using Match3.Game.Gameplay.Core;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay
{
    public class RandomCellStackChooser
    {
        CellStackBoard cellStackBoard;

        Predicate<CellStack> originalMultiValidator;
        HashSet<CellStack> chosenCellStacks = new HashSet<CellStack>();

        private int lastLinearIndex;
        private static bool isLeftMove = true;
  
        public RandomCellStackChooser(CellStackBoard cellStackBoard)
        {
            this.cellStackBoard = cellStackBoard;
            this.lastLinearIndex = cellStackBoard.PositionToLinearIndex(cellStackBoard.Width() - 1, cellStackBoard.Height() - 1);
        }

        public List<CellStack> Choose(int count, Predicate<CellStack> validator)
        {
            List<CellStack> cellStacks = new List<CellStack>();

            this.originalMultiValidator = validator;

            for (int i = 0; i < count; ++i)
            {
                var stack = ChooseOne(MultiValidator);
                if (stack != null)
                {
                    chosenCellStacks.Add(stack);
                    cellStacks.Add(stack);
                }
            }

            chosenCellStacks.Clear();

            return cellStacks;
        }

        bool MultiValidator(CellStack cellStack)
        {
            return chosenCellStacks.Contains(cellStack) == false && originalMultiValidator(cellStack);
        }

        public CellStack ChooseOne(Predicate<CellStack> validator)
        {
            int startPosition = UnityEngine.Random.Range(0, lastLinearIndex + 1);
            int  currentPosition = startPosition;
            CellStack current = null;
            bool isValid = false;
            
            do
            {
                current = cellStackBoard[cellStackBoard.LinearIndexToPosition(currentPosition)];
                isValid = validator(current);

                if (isLeftMove)
                {
                    currentPosition--;
                    if (currentPosition < 0)
                        currentPosition = lastLinearIndex;
                }
                else
                {
                    currentPosition++;
                    if (currentPosition > lastLinearIndex)
                        currentPosition = 0;
                }

            } while (currentPosition != startPosition && isValid == false);

            isLeftMove = !isLeftMove;
            if (isValid)
                return current;
            else
                return null;
        }
    }
}