using Iggy.Net.Enums;

namespace Iggy.Net.Options;

public interface IIgyOptions
{
    public string BaseAdress { get; set; } 
    public Protocol Protocol { get; set; }
    // public IEnumerable<HttpRequestHeaderContract>? Headers { get; set; }
    public int ReceiveBufferSize { get; set; }
    public int SendBufferSize { get; set; }
}