﻿using System;
using FluentAssertions;
using Ncqrs.Domain;
using Ncqrs.Eventing.Conversion;
using NUnit.Framework;

namespace Ncqrs.Tests.Eventing.Conversion
{
    public class EventConverterTests
    {
        public class FooEventV1 : DomainEvent
        {
            public string Name
            {
                get;
                set;
            }
        }

        public class FooEventV2 : DomainEvent
        {
            public string Name
            {
                get;
                set;
            }

            public string LastName
            {
                get;
                set;
            }

            public FooEventV2(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string name, string lastName)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                Name = name;
                LastName = lastName;
            }
        }

        public class FooEventV3 : DomainEvent
        {
            public string Name
            {
                get;
                set;
            }

            public string LastName
            {
                get;
                set;
            }

            public string Message
            {
                get;
                set;
            }

            public FooEventV3(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp, string name, string lastName, string message)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
                Name = name;
                LastName = lastName;
                Message = message;
            }
        }

        public class BarEventV1 : DomainEvent
        {
            public string Name
            {
                get;
                set;
            }
        }

        public class BarEventV2 : DomainEvent
        {
            public string FullName
            {
                get;
                set;
            }

            public BarEventV2(Guid eventIdentifier, Guid aggregateRootId, long eventSequence, DateTime eventTimeStamp)
                : base(eventIdentifier, aggregateRootId, eventSequence, eventTimeStamp)
            {
            }
        }

        [Test]
        public void Calling_convert_should_call_convert_for_the_register_converter()
        {
            var converter = new EventConverter();
            converter.AddConverter(
                (BarEventV1 e) =>
                new BarEventV2(e.EventIdentifier, e.AggregateRootId, e.EventSequence, e.EventTimeStamp));

            converter.Convert(new BarEventV1()).Should().BeOfType<BarEventV2>();
        }

        [Test]
        public void Calling_convert_should_call_convert_until_there_is_no_converter_anymore()
        {
            var converter = new EventConverter();
            converter.AddConverter
                (
                    (BarEventV1 e) =>
                    new BarEventV2(e.EventIdentifier, e.AggregateRootId, e.EventSequence, e.EventTimeStamp)
                ).AddConverter
                (
                    (FooEventV1 e) =>
                    new FooEventV2(e.EventIdentifier, e.AggregateRootId, e.EventSequence, e.EventTimeStamp, e.Name, "")
                ).AddConverter
                (
                    (FooEventV2 e) =>
                    new FooEventV3(e.EventIdentifier, e.AggregateRootId, e.EventSequence, e.EventTimeStamp, e.Name,
                                   e.LastName, "")
                );

            converter.Convert(new FooEventV1()).Should().BeOfType<FooEventV3>();
        }
    }
}