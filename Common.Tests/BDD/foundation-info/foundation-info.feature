Feature: Foundation Info
  In order to display environment details
  As a client
  I want to retrieve foundation information as raw JSON

  Scenario Outline: Retrieve foundation info
    Given the foundation endpoint returns <json>
    When I request the foundation info
    Then the API returns <json>

    Examples:
      | json              |
      | {"name":"tas"} |
