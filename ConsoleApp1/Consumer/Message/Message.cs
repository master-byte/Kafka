using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Consumer
{
     public class Message 
    {
        public Message()
        {
            NickName = "9853691687";
            NumberCreated = DateTime.Now;
        }

        public string NickName { get; private set; }

        /// <summary>
        /// Дата создания номера
        /// </summary>
        public DateTime NumberCreated { get; private set; }

        /// <summary>
        /// Дата попадания сообщения в очередь
        /// </summary>
        public DateTime DateQueue { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public int Number { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
