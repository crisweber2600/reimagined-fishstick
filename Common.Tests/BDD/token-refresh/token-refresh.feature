Feature: Token Refresh
  In order to maintain access after token expiry
  As a client
  I want to refresh my bearer token using a refresh token

  Scenario Outline: Refresh expired token
    Given the refresh endpoint returns <access_token> and <new_refresh_token>
    When I refresh using <current_refresh_token>
    Then the TokenRefresher returns <access_token> and <new_refresh_token>

    Examples:
      | current_refresh_token | access_token | new_refresh_token |
      | xyz                   | newtoken     | newrefresh        |

