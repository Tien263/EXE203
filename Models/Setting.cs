using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Setting
{
    public int SettingId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string? SettingValue { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
