using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProjects.Models;
public class VerificationRequest
{
    public required string UserId { get; set; }
    public int FourDigitVerificationCode { get; set; }
}
