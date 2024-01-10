using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class Verification
{
    public required string Id { get; set; }
    public required User UserInstance { get; set; }
    public int FourDigitVerificationCode { get; set; }
    public DateTime ExpirationDate { get; set; }
}
