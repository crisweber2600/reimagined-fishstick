Feature: Token Endpoint Discovery
  In order to authenticate with the correct UAA
  As a client
  I want the AuthenticationService to discover the token endpoint via /info

  Scenario Outline: Discover and use token endpoint
    Given the info endpoint returns <token_url>
    When I authenticate with <username> and <password>
    Then the token request is sent to <token_url>

    Examples:
      | username | password | token_url             |
      | user     | pass     | http://uaa.example.com |
