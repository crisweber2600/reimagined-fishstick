using System.Threading;

namespace Common;

public interface ITokenCache
{
    TokenModel? Current { get; set; }
}

public class TokenCache : ITokenCache
{
    private readonly object _sync = new();
    private TokenModel? _token;
    public TokenModel? Current
    {
        get { lock(_sync) { return _token; } }
        set { lock(_sync) { _token = value; } }
    }
}
