using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class MasterData
{
    public int Id { get; set; }
    public MasterDataType Type { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
