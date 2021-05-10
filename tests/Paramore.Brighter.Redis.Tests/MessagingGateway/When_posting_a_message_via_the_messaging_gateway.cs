﻿using System;
using System.Linq;
using FluentAssertions;
using Paramore.Brighter.Redis.Tests.Fixtures;
using Xunit;

namespace Paramore.Brighter.Redis.Tests.MessagingGateway
{
    [Collection("Redis")]
    [Trait("Category", "Redis")]
    public class RedisMessageProducerSendTests : IClassFixture<RedisFixture>
    {
        private readonly RedisFixture _redisFixture;
        private readonly Message _message;

        public RedisMessageProducerSendTests(RedisFixture redisFixture)
        {
            const string topic = "test";
            _redisFixture = redisFixture;
            _message = new Message(
                new MessageHeader(Guid.NewGuid(), topic, MessageType.MT_COMMAND), 
                new MessageBody("test content")
                );
        }
        
        
        [Fact]
        public void When_posting_a_message_via_the_messaging_gateway()
        {
            _redisFixture.Consumer.Receive(1000); //Need to receive to subscribe to feed, before we send a message. This returns an empty message we discard
            _redisFixture.Producer.Send(_message);
            var sentMessage = _redisFixture.Consumer.Receive(1000).Single();
            var messageBody = sentMessage.Body.Value;
            _redisFixture.Consumer.Acknowledge(sentMessage);

            //_should_send_a_message_via_restms_with_the_matching_body
            messageBody.Should().Be(_message.Body.Value);
        }
    }
}
