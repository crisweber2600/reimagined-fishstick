Feature: Client Initialization
  In order to communicate with a TAS foundation
  As a developer
  I want to create a TasClient from configured credentials

  Scenario Outline: Initialize TasClient with valid options
    Given I configure TasClient with foundation URI <uri>, username <username>, password <password>
    When I build the TasClient
    Then the client is initialized successfully

    Examples:
      | uri             | username | password |
      | https://api.tas | user     | pass     |
