## 0.5.3 - 2018-12-06
### Fixed
- RabbitMQ : An empty routing key was throwing an ArgumentNullException on DeclareBinding.

## 0.5.2 - 2018-11-21
### Added
- 2 new parameters on rabbitMq connection: friendlyName & requestedChannelMax

## 0.5.1 - 2018-09-25
### Fixed
- SharedConnection is disposable to properly close rabbitMq connection

## 0.4.2 - 2018-08-08
### Fixed
- RabbitMQ Consumer : consumer lock was static

## 0.4.1 - 2018-08-01
### Changed
- Samples.Startup

## 0.4.0 - 2018-07-31
### Changed
- .Net standard 2.0
### Added
- MerQureConfiguration
### Deleted
- RabbitMqConfigurationSection

## 0.3.1 - 2017-10-23
### Added
- Prefetch count support
- Publisher acknowledgement support
- Publishing without IMessage support
- Publishing many messages with transaction support

## 0.3.0 - 2017-03-08
### Changed
- HeaderProperties' values as object
### Added
- Message Priority Support
- DeadLetter Policy Support
- Exchange type support: direct, fanout, topic, headers

## 0.2.2 - 2017-01-11
### Changed
- IPublisher and IConsumer are IDisposable

## 0.2.1 - 2016-12-21
### Added
- Sonar ruleset

## 0.2.0 - 2016-12-21
### Removed
- ISubcriber: was ambiguous, replaced by specific methods
### Added
- IMessagingService.DeclareExchange
- IMessagingService.DeclareQueue
- IMessagingService.DeclareBinding
- IMessagingService.CancelBinding

## 0.1.6 - 2016-12-07
### Added
- IsConsuming: indicates when a consumer is registered
- StopConsuming: unregister a consumer

## 0.1.5 - 2016-12-02
### Changed
- HeaderProperties' values as string

## 0.1.4 - 2016-12-02
### Added
- logo

## 0.1.2 - 2016-11-22
### Changed
- IMessage interface has only methods

## 0.1.1 - 2016-11-21
### Added
- Nuget package definition

## 0.1.0 - 2016-11-14
### Added
- First version of the library
