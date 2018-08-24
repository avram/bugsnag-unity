Feature: Handled Errors and Exceptions

Scenario Outline: Reporting a handled exception
  Given I configure the bugsnag notify endpoint with "<endpoint>"
  And I set environment variable "BUGSNAG_APIKEY" to "a35a2a72bd230ac0aa0f52715bbdc6aa"
  When I build a Unity application for "<platform>"
  And run the <platform> application
  Then I should receive a request
  And the request is a valid for the error reporting API
  And the "Bugsnag-API-Key" header equals "a35a2a72bd230ac0aa0f52715bbdc6aa"
  And the payload field "notifier.name" equals "Unity Bugsnag Notifier"
  And the payload field "events" is an array with 1 element
  And the exception "errorClass" equals "System.Exception"
  And the exception "message" equals "blorb"

Examples:
  | platform | endpoint   |
  | Android  | 10.0.2.2   |
  | MacOS    | localhost  |
