using System;

namespace Telegram.Dto.SendMessage.ReplyMarkup.Abstracts
{
    public abstract class Keyboard<TButton> : ReplyMarkup where TButton : Button
    {
        protected virtual int MaxButtons { get; set; }
        private int _rowCounter;
        public virtual TButton[][] Buttons { get; set; } = Array.Empty<TButton[]>();

        protected void AddRaw(KeyboardRaw<TButton> keyboardRaw)
        {
            int keyboardColumnCount = keyboardRaw.ButtonsList.Count;
            CheckPossibleIncreaseArray(_rowCounter, keyboardColumnCount);
            ArrayExpand(keyboardColumnCount);
            
            for (int i = 0; i < keyboardRaw.ButtonsList.Count; i++)
            {
                Buttons[_rowCounter][i] = keyboardRaw.ButtonsList[i];
            }

            _rowCounter++;
        }

        private void CheckPossibleIncreaseArray(int rowCounter, int columnSize)
        {
            if (rowCounter + 1 >= MaxButtons)
            {
                throw new IndexOutOfRangeException($"{nameof(Keyboard<TButton>)} error:" +
                                                   "Reached the maximum raw limit for keyboards. " +
                                                   $"Maximum = {MaxButtons}");
            }

            if (columnSize >= MaxButtons)
            {
                throw new IndexOutOfRangeException($"{nameof(Keyboard<TButton>)} error:" +
                                                   "Reached the maximum column limit for keyboards. " +
                                                   $"Maximum = {MaxButtons}");
            }
        }

        private void ArrayExpand(int columnSize)
        {
            TButton[][] tempButtons = new TButton[_rowCounter + 1][];

            if (Buttons is null)
            {
                Buttons = new TButton[1][];
                Buttons[0] = new TButton[columnSize];
            }
            
            for (int i = 0; i < Buttons.Length; i++)
            {
                tempButtons[i] = Buttons[i];
            }

            tempButtons[_rowCounter] = new TButton[columnSize];
            Buttons = tempButtons;
        }
    }
}