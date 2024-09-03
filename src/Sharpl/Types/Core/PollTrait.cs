using System.Threading.Channels;

namespace Sharpl.Types.Core;

public interface PollTrait {
    Task<bool> Poll(Value target, CancellationToken ct);
}