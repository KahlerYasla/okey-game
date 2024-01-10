using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GenericResponse<T>
{
    public T? Data { get; set; }
    public bool Result { get; set; }
    public string? Message { get; set; }
}
