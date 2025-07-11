namespace SocPlus.Models; 
public class Result {
    public object? Value { get; set; }
    public bool Success { get; set; }
    public int ErrorCode { get; init; } = 400;
    public Error[] Errors { get; set; } = [];
    public Result(params Error[] errors) {
        Success = false;
        Errors = errors;
    }
    public Result(object value) {
        Success = true;
        Value = value;
    }
}
