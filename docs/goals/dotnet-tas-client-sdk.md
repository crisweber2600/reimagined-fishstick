# dotnet-tas-client-sdk

Epic Goal  
> Build a **.NET 9** client SDK that authenticates to Tanzu Application Service (TAS) foundations via username + password → bearer-token flow (with automatic refresh) and exposes raw-JSON GET helpers for foundation, org, space, app/component, and process data—without pagination—using in-memory credential storage.

Status Table (auto-updated)


 - [x] **Feature 1: Authentication & Token Management**
    - [x] **Story 1.1: Auth Token Acquisition (`auth-token-acquisition.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/auth-token-acquisition/auth-token-acquisition.feature`.
        - [x] Implement `AuthenticationService.cs` → `GetBearerTokenAsync(string username, string password)`.
        - [x] Create `TokenModel.cs` for raw-JSON deserialization.
        - [x] Wire `AuthenticationService` into `TasClient.cs` constructor.
    - [x] **Story 1.2: Token Refresh Handling (`token-refresh.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/token-refresh/token-refresh.feature`.
        - [x] Implement `ITokenRefresher.cs` → `RefreshAsync(string refreshToken)`.
        - [x] Integrate automatic refresh into `HttpClientHandler` pipeline.
        - [x] Unit-test refresh-expiry edge cases in `TokenRefresherTests.cs`.
    - [x] Implement Authentication & Token Management

 - [x] **Feature 2: Client Initialization & Configuration**
    - [x] **Story 2.1: Initialize Client (`client-initialization.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/client-initialization/client-initialization.feature`.
        - [x] Implement `TasClientOptions.cs` (foundation URI, username, password).
        - [x] Build `TasClientBuilder.cs` fluent builder for options validation.
        - [x] Register `TasClient` + dependencies in DI (`ServiceCollectionExtensions.cs`).
    - [x] Implement Client Initialization & Configuration

 - [x] **Feature 3: Foundation / Org / Space Retrieval**
    - [x] **Story 3.1: Retrieve Foundation Info (`foundation-info.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/foundation-info/foundation-info.feature`.
        - [x] Implement `FoundationApi.cs` → `GetFoundationAsync()` (raw JSON).
        - [x] Expose `TasClient.GetFoundationAsync()`.
    - [x] **Story 3.2: Retrieve Org & Space Info (`org-space-info.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/org-space-info/org-space-info.feature`.
        - [x] Implement `OrgSpaceApi.cs` → `GetAllOrgsAsync()` and `GetSpacesForOrgAsync(orgId)`.
        - [x] Expose `TasClient.GetOrgsAsync()` and `TasClient.GetSpacesAsync(orgId)`.
        - [x] Add resilience policies for 429/5xx in `OrgSpaceApi.cs`.

- [ ] **Feature 4: Application & Process Retrieval**
    - [x] **Story 4.1: Retrieve App / Component Info (`app-component-info.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/app-component-info/app-component-info.feature`.
        - [x] Implement `AppApi.cs` → `GetAppsAsync(spaceId)` (raw JSON).
        - [x] Expose `TasClient.GetAppsAsync(spaceId)`.
        - [x] Add integration test `AppApiIntegrationTests.cs` for 200/404.
    - [x] **Story 4.2: Retrieve Process Info (`process-info.feature`)**
        - [x] Add BDD file `Common.Tests/BDD/process-info/process-info.feature`.
        - [x] Implement `ProcessApi.cs` → `GetProcessesAsync(appId)`.
        - [x] Expose `TasClient.GetProcessesAsync(appId)`.
        - [x] Validate token refresh on long-running calls in `ProcessApiTests.cs`.

References  
- Cloud Foundry v3 API Reference – all resources (apps, processes, orgs, spaces)  <https://v3-apidocs.cloudfoundry.org/>  
- UAA OAuth2 & Refresh-Token flow  <https://docs.cloudfoundry.org/api/uaa/>  
- Org/Space concepts in Tanzu Application Service  <https://techdocs.broadcom.com/us/en/vmware-tanzu/platform/tanzu-platform-for-cloud-foundry/6-0/tpcf/roles.html>  
- Planning TAS orgs and spaces  <https://techdocs.broadcom.com/us/en/vmware-tanzu/platform/tanzu-platform-for-cloud-foundry/6-0/tpcf/orgs-and-spaces.html>  
- Finding your Cloud Foundry API endpoint  <https://docs.cloudfoundry.org/running/cf-api-endpoint.html>  
- Attaching a TAS foundation as a data source (foundation topology)  <https://techdocs.broadcom.com/us/en/vmware-tanzu/platform/tanzu-platform/saas/tnz-platform/data-sources-attach-tas-foundation.html>  
- Authentication & enterprise SSO in TAS for VMs  <https://techdocs.broadcom.com/us/en/vmware-tanzu/platform/tanzu-platform-for-cloud-foundry/4-0/tpcf/auth-sso.html>  
- What’s new in .NET 9 – SDK & runtime overview  <https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview>  
- Official .NET 9 download page  <https://dotnet.microsoft.com/en-us/download/dotnet/9.0>

## Codex Tasks
- [x] Add BDD file `Common.Tests/BDD/client-initialization/client-initialization.feature`
- [x] Implement `TasClientOptions.cs` (foundation URI, username, password)
- [x] Build `TasClientBuilder.cs` fluent builder for options validation
- [x] Register `TasClient` + dependencies in DI (`ServiceCollectionExtensions.cs`)
- [x] Add BDD file `Common.Tests/BDD/foundation-info/foundation-info.feature`
- [x] Add step definitions for auth and token refresh features
- [x] Add BDD file `Common.Tests/BDD/org-space-info/org-space-info.feature`
- [x] Implement OrgSpaceApi and TasClient methods
- [x] Add BDD file `Common.Tests/BDD/app-component-info/app-component-info.feature`
- [x] Implement AppApi and TasClient method
- [x] Add integration test `AppApiIntegrationTests.cs`
- [ ] Add BDD file `Common.Tests/BDD/token-endpoint-discovery/token-endpoint-discovery.feature`
- [ ] Implement UAA endpoint discovery and Basic Auth in `AuthenticationService`
- [ ] Expand `TokenModel` with `token_type` and `expires_in`
- [ ] Implement thread-safe refresh in `TokenRefreshingHandler`
- [ ] Unit test token endpoint discovery and refresh logic
- [ ] Add BDD file `Common.Tests/BDD/foundation-info-pagination/foundation-info-pagination.feature`
- [ ] Auto-paginate `OrgSpaceApi` results and enhance retry policy
- [ ] Update `FoundationApi` to return usage summary JSON
- [ ] Unit test pagination and retry scenarios
- [ ] Add BDD file `Common.Tests/BDD/app-listing-correct/app-listing-correct.feature`
- [ ] Update `AppApi.GetAppsAsync` to filter by space GUID
- [ ] Handle 404 from `GetAppsAsync` gracefully
- [ ] Verify `ProcessApi.GetProcessesAsync` and add refresh expiry test
- [ ] Ensure next step is clear for Codex
- [ ] Start next task after merge
