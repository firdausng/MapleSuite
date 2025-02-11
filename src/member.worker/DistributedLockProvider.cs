using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace member.worker;

public class DistributedLockProvider
{
    private readonly RedLockFactory _redLockFactory;

    public DistributedLockProvider(IConnectionMultiplexer connectionMux)
    {
        _redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>
        {
            new RedLockMultiplexer(connectionMux)
        });
    }

    public async Task<IRedLock> AcquireLockAsync(string resource, TimeSpan expiryTime)
    {
        return await _redLockFactory.CreateLockAsync(
            resource: resource,
            expiryTime: expiryTime,
            waitTime: TimeSpan.FromSeconds(1),
            retryTime: TimeSpan.FromMilliseconds(100)
        );
    }
}