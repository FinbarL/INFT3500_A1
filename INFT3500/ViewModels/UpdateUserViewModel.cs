using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace INFT3500.ViewModels;

public class UpdateUserViewModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsStaff { get; set; }
}