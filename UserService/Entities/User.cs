using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string IdNo { get; set; }

        public string Email { get; set; }

        public string EngName { get; set; }

        public string Gender { get; set; }
    }
}
