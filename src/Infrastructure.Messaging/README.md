# StudentSystem.Infrastructure.Messaging library

This library contains helper classes for working with messaging within the Student Management System sample solution. It provides an abstraction over RabbitMQ for publishing events and handling commands.

## Features

- Base classes for `Command` and `Event` messages.
- Interfaces (`IMessagePublisher`, `IMessageHandler`) that abstract message broker functionality.
- RabbitMQ implementations for these interfaces.
- A helper class (`MessageSerializer`) for serializing messages to and from JSON.
