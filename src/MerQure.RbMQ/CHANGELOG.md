## 0.4.0 - 2017-10-23
### Added
- Prefetch count support
- Publisher acknowledgement support
- Ack and Nack support with delivery tag
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
- Publisher and Consumer close explicitly the channel on Dispose()

## 0.2.1 - 2016-12-21
### Added
- Sonar ruleset

## 0.2.0 - 2016-12-21
### Changed
- MerQure 0.2.0 impacts on IMessagingService implementation

## 0.1.9 - 2016-12-07
### Added
- rabbitMQ.config file, declare connectionString and others configuration parameters

## 0.1.8 - 2016-12-07
### Fixed
- use a default connectionString (localhost/guest) when not configured

## 0.1.7 - 2016-12-07
### Added
- IsConsuming: indicates when a consumer is registered
- StopConsuming: unregister a consumer

## 0.1.6 - 2016-12-05
### Fixed
- HeaderProperties' values as string

## 0.1.5 - 2016-12-02
### Changed
- HeaderProperties' values as string

## 0.1.4 - 2016-12-02
### Added
- logo

## 0.1.3 - 2016-12-01
### Changed
- Connection loaded from the app.config file: amqp URI defined in the connectionString named 'RabbitMQ'. See 'App.config.model'.

## 0.1.1 - 2016-11-21
### Added
- Nuget package definition

## 0.1.0 - 2016-11-21
### Added
- First version of the library
