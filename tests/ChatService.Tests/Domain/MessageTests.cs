using ChatService.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ChatService.Tests.Domain;

public class MessageTests
{
    [Fact]
    public void CreateDirect_ShouldThrow_WhenContentExceeds4000Chars()
    {
        var act = () => Message.CreateDirect("a@a.com", "b@b.com", new string('x', 4001));
        act.Should().Throw<ArgumentException>().WithMessage("*4000*");
    }

    [Fact]
    public void SoftDelete_ShouldThrow_WhenNotSender()
    {
        var msg = Message.CreateDirect("sender@a.com", "recv@b.com", "Hello");
        var act = () => msg.SoftDelete("other@c.com");
        act.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void MarkAsRead_ShouldSetReadAt()
    {
        var msg = Message.CreateDirect("a@a.com", "b@b.com", "Hi");
        msg.ReadAt.Should().BeNull();
        msg.MarkAsRead();
        msg.ReadAt.Should().NotBeNull();
    }
}
