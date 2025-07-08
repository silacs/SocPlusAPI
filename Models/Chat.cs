#pragma warning disable CS8618
namespace SocPlus.Models; 
public class Chat {
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsGroup { get; set; } = false;
    public string Name { get; set; }
}
