using System;

namespace Common;

public class TasClientOptions
{
    public Uri FoundationUri { get; set; } = new("https://localhost");
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// TASK: Validate required fields
    public void Validate()
    {
        if (FoundationUri == null)
            throw new InvalidOperationException("FoundationUri is required");
        if (FoundationUri.OriginalString.EndsWith("/"))
            throw new InvalidOperationException("FoundationUri must not have a trailing slash");
        if (string.IsNullOrWhiteSpace(Username))
            throw new InvalidOperationException("Username is required");
        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("Password is required");
    }
}
