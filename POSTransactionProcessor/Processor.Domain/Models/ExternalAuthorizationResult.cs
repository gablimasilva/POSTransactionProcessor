public class ExternalAuthorizationResult {
    public bool Approved { get; set; }
    public string AuthorizationCode { get; set; } = default!;
    public string Message { get; set; } = default!;
}
