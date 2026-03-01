using Microsoft.Extensions.Configuration;
using Moq;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;
using Soat.Eleven.Kutcut.Users.Infra.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Services;

public class CacheServiceTests
{
    private readonly Mock<IDatabase> _databaseMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IConnectionMultiplexer> _connectionMultiplexerMock;
    private readonly Mock<IConfigurationSection> _connectionStringsSectionMock;
    private const string RedisConnectionString = "localhost:6379";
    private const int RedisExpireSeconds = 3600;

    public CacheServiceTests()
    {
        _databaseMock = new Mock<IDatabase>();
        _configurationMock = new Mock<IConfiguration>();
        _connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
        _connectionStringsSectionMock = new Mock<IConfigurationSection>();

        _connectionStringsSectionMock.Setup(s => s.Value).Returns(RedisConnectionString);
        _configurationMock
            .Setup(c => c.GetSection("ConnectionStrings"))
            .Returns(Mock.Of<IConfigurationSection>(s => s["Redis"] == RedisConnectionString));

        var redisExpireSection = new Mock<IConfigurationSection>();
        redisExpireSection.Setup(s => s.Value).Returns(RedisExpireSeconds.ToString());
        _configurationMock
            .Setup(c => c.GetSection("RedisExpire"))
            .Returns(redisExpireSection.Object);
        _configurationMock
            .Setup(c => c["RedisExpire"])
            .Returns(RedisExpireSeconds.ToString());
    }

    [Fact]
    public async Task GetAsync_BuscarChaveExistente_RetornaObjetoDeserializado()
    {
        var key = "user:test-key";
        var expectedUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var serializedUser = JsonSerializer.Serialize(expectedUser);

        _databaseMock
            .Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serializedUser));

        _connectionMultiplexerMock
            .Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_databaseMock.Object);

        _configurationMock
            .Setup(c => c.GetSection("ConnectionStrings").GetSection("Redis"))
            .Returns(_connectionStringsSectionMock.Object);

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        var result = await cacheService.GetAsync<User>(key);

        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Name, result.Name);
        Assert.Equal(expectedUser.Email, result.Email);
        _databaseMock.Verify(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_BuscarChaveInexistente_RetornaDefault()
    {
        var key = "user:non-existent-key";

        _databaseMock
            .Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        var result = await cacheService.GetAsync<User>(key);

        Assert.Null(result);
        _databaseMock.Verify(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_BuscarComCacheVazio_RetornaDefault()
    {
        var key = "user:empty-key";

        _databaseMock
            .Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(string.Empty));

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        var result = await cacheService.GetAsync<User>(key);

        Assert.Null(result);
        _databaseMock.Verify(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetAsync_SalvarObjetoNoCache_NaoLancaExcecao()
    {
        var key = "user:test-key";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _databaseMock
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        // Act & Assert - verificar que o m\u00e9todo completa sem lan\u00e7ar exce\u00e7\u00e3o
        await cacheService.SetAsync(key, user);

        // Se chegou aqui, o teste passou
        Assert.True(true);
    }

    [Fact]
    public async Task RemoveAsync_RemoverChaveExistente_ChamaKeyDeleteAsync()
    {
        var key = "user:test-key";

        _databaseMock
            .Setup(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        await cacheService.RemoveAsync(key);

        _databaseMock.Verify(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_RemoverChaveInexistente_ChamaKeyDeleteAsync()
    {
        var key = "user:non-existent-key";

        _databaseMock
            .Setup(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        await cacheService.RemoveAsync(key);

        _databaseMock.Verify(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetAsync_SalvarMultiplosObjetosComChavesDiferentes_NaoLancaExcecao()
    {
        var key1 = "user:key1";
        var key2 = "user:key2";
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User 1",
            Email = "user1@example.com",
            Password = "hash1",
            Status = StatusUser.Active
        };
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Name = "User 2",
            Email = "user2@example.com",
            Password = "hash2",
            Status = StatusUser.Active
        };

        _databaseMock
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var cacheService = new TestCacheService(_configurationMock.Object, _databaseMock.Object);

        // Act & Assert - verificar que os m\u00e9todos completam sem lan\u00e7ar exce\u00e7\u00e3o
        await cacheService.SetAsync(key1, user1);
        await cacheService.SetAsync(key2, user2);

        // Se chegou aqui, o teste passou
        Assert.True(true);
    }

    private class TestCacheService : CacheService
    {
        public TestCacheService(IConfiguration configuration, IDatabase database)
            : base(CreateTestConfiguration())
        {
            var field = typeof(CacheService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(this, database);
        }

        private static IConfiguration CreateTestConfiguration()
        {
            var testConfig = new Mock<IConfiguration>();

            var connectionStringsSection = new Mock<IConfigurationSection>();
            connectionStringsSection.Setup(s => s["Redis"]).Returns("localhost:6379,abortConnect=false");

            testConfig
                .Setup(c => c.GetSection("ConnectionStrings"))
                .Returns(connectionStringsSection.Object);

            var redisExpireSection = new Mock<IConfigurationSection>();
            redisExpireSection.Setup(s => s.Value).Returns("3600");

            testConfig
                .Setup(c => c.GetSection("RedisExpire"))
                .Returns(redisExpireSection.Object);

            testConfig
                .Setup(c => c["RedisExpire"])
                .Returns("3600");

            return testConfig.Object;
        }
    }
}
