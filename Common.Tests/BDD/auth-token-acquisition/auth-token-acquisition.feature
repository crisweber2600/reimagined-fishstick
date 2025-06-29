Feature: Auth Token Acquisition
  In order to interact with TAS APIs
  As a client
  I want to obtain a bearer token from username and password

  Scenario Outline: Acquire bearer token
    Given the auth endpoint returns <access_token> and <refresh_token>
    When I request a token using <username> and <password>
    Then the AuthenticationService returns <access_token> and <refresh_token>

    Examples:
      | username | password | access_token | refresh_token |
      | user1    | pass1    | abc          | xyz           |
