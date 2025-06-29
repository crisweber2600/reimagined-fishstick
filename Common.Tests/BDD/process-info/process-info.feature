Feature: Process Info
  In order to monitor application processes
  As a client
  I want to retrieve processes for an app as raw JSON

  Scenario Outline: Retrieve processes
    Given the process endpoint returns <json>
    When I request processes for app <appId>
    Then the process API returns <json>

    Examples:
      | appId | json           |
      | app1  | {"procs":"p"} |
