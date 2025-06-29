Feature: Org and Space Info
  In order to manage resources
  As a client
  I want to retrieve organizations and spaces

  Scenario Outline: Retrieve information
    Given the endpoint for <kind> returns <json>
    When I request <kind>
    Then I receive <json>

    Examples:
      | kind  | json             |
      | orgs  | {"orgs":"o"}    |
      | spaces| {"spaces":"s"}  |
