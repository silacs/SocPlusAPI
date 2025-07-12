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

    public Result(string name, bool notFound) {
        if (notFound) {
            Success = false;
            ErrorCode = 404;
            Errors = [new Error(name, $"{name} not found")];
        } else {
            try {
                throw new Exception(
                    "The result constructor with the signature (string name, bool notFound) is a helper method to create a 404 Error." +
                    " The notFound parameter is only there to differentiate from other constructors" +
                    " (which is why it cannot have a default value), and should always be set to true."
                );
            } catch (Exception e) {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
    }
}
