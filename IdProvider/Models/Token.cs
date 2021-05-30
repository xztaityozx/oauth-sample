using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdProvider.Models
{
    public record Token(int Id, int UserId, string Jwt);
}
