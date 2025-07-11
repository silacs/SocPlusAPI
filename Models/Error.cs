namespace SocPlus.Models; 
public class Error(string name, params string[] errors) {
    public string Name { get; set; } = name;
    public string[] Errors { get; set; } = errors;
}
