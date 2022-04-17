using System.Collections.Generic;

namespace Telegram.Dto.SendMessage.ReplyMarkup.Abstracts
{
    public class KeyboardRaw<TButtons> where TButtons : Button
    {
        public List<TButtons> ButtonsList { get; set; } = new();

        public void AddButton(TButtons inlineKeyboardButton)
        {
            ButtonsList.Add(inlineKeyboardButton);
        }
        
        public void AddButtons(IEnumerable<TButtons> inlineKeyboardButtons)
        {
            ButtonsList.AddRange(inlineKeyboardButtons);
        }
    }
}