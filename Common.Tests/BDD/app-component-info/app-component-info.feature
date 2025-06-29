Feature: App Component Info
  In order to inspect applications
  As a client
  I want to retrieve apps for a space as raw JSON

  Scenario Outline: Retrieve apps
    Given the app endpoint returns <json>
    When I request apps for space <spaceId>
    Then the app API returns <json>

    Examples:
      | spaceId | json           |
      | space1  | {"apps":"a"} |
