using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Transfers
{
    /// <summary>
    /// Инкапсулирует информацию о получателе для отображения в списке
    /// </summary>
    public class RecipientListDto
    {
        public int RecipientId { get; set; }
        public string RecipientName { get; set; }
    }
}
