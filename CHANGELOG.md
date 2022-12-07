## Changelog

All project changes.

Use [Keep a Changelog](https://keepachangelog.com/ru/1.0.0/) style for any change.

## [Unreleased]()

### Added

+ OrderController: Control trade copy by allow list. Only allowed user trades will be copied
+ EmailListener: Support multiple mail box accounts by `MailBoxStore`. Each mail box processed by `EmailListenerStateMachine`
+ EmailListener: Communication between `EmailListenerJob` and `CollectProcessCommand` processed with `System.Threading.Channels` by `EmailChannel` 

### Changed

+ Admin: Fixed orders mapping

### Removed

