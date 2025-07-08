#pragma warning disable CS8618
namespace SocPlus.Models; 
public class Error {
    public string Name { get; set; }
    public string[] Errors { get; set; }
    public Error(string name, params string[] errors) {
        Name = name;
        Errors = errors;
    }
}
